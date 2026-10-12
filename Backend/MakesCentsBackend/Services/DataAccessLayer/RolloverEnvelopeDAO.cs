/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using Humanizer;
using MakesCentsBackend.Models;
using MySqlConnector;
using System.Data;
using System.Reflection;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    /// <summary>
    /// Data access object for rollover envelopes.
    /// Rollover envelopes are split across two tables:
    /// - rollover_envelope: the persistent definition (name, category, default transfer target).
    ///   No dollar amounts live here since a rollover envelope isn't guaranteed to be active every month.
    /// - rollover_envelope_month: the amounts for one specific month. Absence of a row here means
    ///   the envelope wasn't activated that month.
    /// Most "get/update amount" operations need a budget_month_id in addition to the envelope id.
    /// </summary>
    public class RolloverEnvelopeDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public RolloverEnvelopeDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create the persistent definition for a rollover envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateRolloverEnvelopeAsync(CreateRolloverEnvelopeRequest envelope)
        {
            // Declare and initialize
            query = """
                INSERT INTO rollover_envelope (envelope_category_id, envelope_name, transfer_envelope_id)
                VALUES (@EnvelopeCategoryId, @EnvelopeName, @TransferEnvelopeId);
                SELECT LAST_INSERT_ID();
                """;
            int rolloverEnvelopeId;

            // Make sure the envelope category belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeCategoryAsync(envelope.EnvelopeCategoryId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Envelope category does not belong to the current user");
            }
            // Make sure the transfer target sinking fund envelope belongs to the user too
            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(envelope.TransferEnvelopeId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Transfer envelope does not belong to the current user");
            }
            // Execute the request and get the new id for the envelope
            try
            {
                // Execute
                rolloverEnvelopeId = await _connection.ExecuteScalarAsync<int>(query, envelope);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_rollover_envelope_name'"))
                    {
                        return new BaseIdResponse(400, "Envelope name already exists in this category");
                    }
                }
                return new BaseIdResponse(500, $"{ex.Number}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Return the issue
                return new BaseIdResponse(500, $"{ex.Message}");
            }

            // Return the new id
            return new BaseIdResponse(201, "Envelope created successfully", rolloverEnvelopeId);
        }

        /// <summary>
        /// DAO method to get (or create, if not yet activated) this month's instance of a rollover envelope.
        /// Called when a budget month is first being set up, or the first time a transaction
        /// split needs to point at this envelope for a given month. No longer takes a planned
        /// amount - that now lives on envelope_funding, not here - so a newly activated month
        /// just starts at its table default remaining_amount of 0.
        /// </summary>
        /// <param name="rolloverEnvelopeId"></param>
        /// <param name="budgetMonthId"></param>
        /// <param name="dbTransaction"></param>
        /// <returns>The rollover_envelope_month_id for this envelope + month, or null on failure</returns>
        public async Task<int?> GetOrActivateForMonthAsync(int rolloverEnvelopeId, int budgetMonthId, MySqlTransaction? dbTransaction = null)
        {
            // Declare and initialize
            int? existingId;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync();

            // Check for an existing month instance first
            query = """
                SELECT rollover_envelope_month_id
                FROM rollover_envelope_month
                WHERE rollover_envelope_id = @RolloverEnvelopeId
                AND budget_month_id = @BudgetMonthId
                """;
            existingId = await _connection.QuerySingleOrDefaultAsync<int?>(query, new { RolloverEnvelopeId = rolloverEnvelopeId, BudgetMonthId = budgetMonthId }, dbTransaction);
            if (existingId.HasValue)
            {
                return existingId;
            }

            // Not active yet this month - create it
            query = """
                INSERT INTO rollover_envelope_month (rollover_envelope_id, budget_month_id)
                VALUES (@RolloverEnvelopeId, @BudgetMonthId);
                SELECT LAST_INSERT_ID();
                """;
            try
            {
                return await _connection.ExecuteScalarAsync<int>(query, new { RolloverEnvelopeId = rolloverEnvelopeId, BudgetMonthId = budgetMonthId }, dbTransaction);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// DAO method to update the remaining amount of a specific month's rollover envelope instance
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rolloverEnvelopeMonthId"></param>
        /// <param name="amount"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRemainingAmountForMonthAsync(int? userId, int? rolloverEnvelopeMonthId, decimal? amount, MySqlTransaction dbTransaction, MySqlConnection connection)
        {
            // Declare and initialize
            query = """
                UPDATE rollover_envelope_month
                SET remaining_amount = remaining_amount + @Amount
                WHERE rollover_envelope_month_id = @RolloverEnvelopeMonthId;
                """;

            // Make sure the envelope month instance belongs to the user
            if (!await _authService.VerifyUserOwnsRolloverEnvelopeMonthAsync(rolloverEnvelopeMonthId, userId, dbTransaction))
            {
                return false;
            }
            // Execute the request
            try
            {
                // Execute
                await connection.ExecuteAsync(query, new
                {
                    Amount = amount,
                    RolloverEnvelopeMonthId = rolloverEnvelopeMonthId
                }, dbTransaction);
            }
            catch (Exception)
            {
                return false;
            }
            // Return true
            return true;
        }

        /// <summary>
        /// DAO method to get a rollover envelope for a specific month, including its transaction history.
        /// If the envelope hasn't been activated for that month, RemainingAmount comes back null.
        /// Planned funding lives on envelope_funding, not here.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetRolloverEnvelopeEntityResponse> GetRolloverEnvelopeForMonthAsync(GetRolloverEnvelopeMonthRequest request)
        {
            // Declare and initialize
            GetRolloverEnvelopeEntityResponse response = new GetRolloverEnvelopeEntityResponse();
            GetRolloverEnvelopeEntityModel responseEntity;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsRolloverEnvelopeAsync(request.RolloverEnvelopeId, request.UserId))
            {
                // Return the false result
                return new GetRolloverEnvelopeEntityResponse(403, "Envelope does not belong to the current user");
            }
            // Set up the query to get the envelope + this month's instance, if any
            query = """
                SELECT 
                    rollover_envelope.rollover_envelope_id AS RolloverEnvelopeId,
                    rollover_envelope.envelope_category_id AS EnvelopeCategoryId,
                    rollover_envelope.envelope_name AS EnvelopeName,
                    rollover_envelope.transfer_envelope_id AS TransferEnvelopeId,
                    rollover_envelope_month.rollover_envelope_month_id AS RolloverEnvelopeMonthId,
                    rollover_envelope_month.remaining_amount AS RemainingAmount
                FROM rollover_envelope
                LEFT JOIN rollover_envelope_month 
                    ON rollover_envelope_month.rollover_envelope_id = rollover_envelope.rollover_envelope_id
                    AND rollover_envelope_month.budget_month_id = @BudgetMonthId
                WHERE rollover_envelope.rollover_envelope_id = @RolloverEnvelopeId
                """;
            // Read the envelope
            responseEntity = await _connection.QuerySingleAsync<GetRolloverEnvelopeEntityModel>(query, new { RolloverEnvelopeId = request.RolloverEnvelopeId, BudgetMonthId = request.BudgetMonthId });
            // Make sure the envelope was found
            if (responseEntity == null)
            {
                // Return the issue
                return new GetRolloverEnvelopeEntityResponse(404, "Envelope not found");
            }
            // If there's no month instance, there's no transaction history to look up either
            if (!responseEntity.RolloverEnvelopeMonthId.HasValue)
            {
                response.RolloverEnvelope = responseEntity;
                response.HttpStatus = 200;
                response.Message = "Envelope found";
                return response;
            }
            // Set up the query for reading the payment transactions that touched this month's envelope
            query = """
                SELECT
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionType,
                    transaction.total_amount AS TotalAmount,
                    payment_transaction.merchant_source_name AS MerchantSourceName,
                    GROUP_CONCAT(DISTINCT COALESCE(sfe.envelope_name, re.envelope_name) SEPARATOR ', ') AS EnvelopeNames
                FROM transaction_split
                INNER JOIN transaction ON transaction_split.transaction_id = transaction.transaction_id
                INNER JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                LEFT JOIN transaction_split AS ts2 ON transaction.transaction_id = ts2.transaction_id
                LEFT JOIN sinking_fund_envelope AS sfe ON ts2.sinking_fund_envelope_id = sfe.sinking_fund_envelope_id
                LEFT JOIN rollover_envelope_month AS rem ON ts2.rollover_envelope_month_id = rem.rollover_envelope_month_id
                LEFT JOIN rollover_envelope AS re ON rem.rollover_envelope_id = re.rollover_envelope_id
                WHERE transaction_split.rollover_envelope_month_id = @RolloverEnvelopeMonthId
                AND transaction.deleted_at IS NULL
                AND transaction.transaction_type_id = 2
                GROUP BY
                    transaction.transaction_id,
                    transaction.transaction_date,
                    transaction.transaction_type_id,
                    transaction.total_amount,
                    payment_transaction.merchant_source_name
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the payment transactions
            responseEntity.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { RolloverEnvelopeMonthId = responseEntity.RolloverEnvelopeMonthId })).ToList();
            // Set up the query to read envelope-to-envelope transfers touching this month's envelope
            query = """
                SELECT
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionType,
                    ABS(to_split.amount) AS TotalAmount,
                    COALESCE(from_sfe.envelope_name, from_re.envelope_name) AS TransferFromEnvelope,
                    COALESCE(to_sfe.envelope_name, to_re.envelope_name) AS TransferToEnvelope
                FROM transaction
                INNER JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                INNER JOIN transaction_split AS from_split 
                    ON from_split.transaction_id = transaction.transaction_id AND from_split.amount < 0
                INNER JOIN transaction_split AS to_split 
                    ON to_split.transaction_id = transaction.transaction_id AND to_split.amount > 0
                LEFT JOIN sinking_fund_envelope AS from_sfe ON from_split.sinking_fund_envelope_id = from_sfe.sinking_fund_envelope_id
                LEFT JOIN rollover_envelope_month AS from_rem ON from_split.rollover_envelope_month_id = from_rem.rollover_envelope_month_id
                LEFT JOIN rollover_envelope AS from_re ON from_rem.rollover_envelope_id = from_re.rollover_envelope_id
                LEFT JOIN sinking_fund_envelope AS to_sfe ON to_split.sinking_fund_envelope_id = to_sfe.sinking_fund_envelope_id
                LEFT JOIN rollover_envelope_month AS to_rem ON to_split.rollover_envelope_month_id = to_rem.rollover_envelope_month_id
                LEFT JOIN rollover_envelope AS to_re ON to_rem.rollover_envelope_id = to_re.rollover_envelope_id
                WHERE (from_split.rollover_envelope_month_id = @RolloverEnvelopeMonthId OR to_split.rollover_envelope_month_id = @RolloverEnvelopeMonthId)
                AND transaction.deleted_at IS NULL
                AND transaction.transaction_type_id = 3
                AND transfer_transaction.transfer_transaction_type_id = 3
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the envelope transfer transactions
            responseEntity.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { RolloverEnvelopeMonthId = responseEntity.RolloverEnvelopeMonthId })).ToList());
            // Set the response entity in the response
            response.RolloverEnvelope = responseEntity;
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Envelope found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to update the persistent definition fields of a rollover envelope
        /// (name, category, default transfer target). Does not touch any month's amounts.
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateRolloverEnvelopeAsync(UpdateRolloverEnvelopeRequest envelope)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsRolloverEnvelopeAsync(envelope.RolloverEnvelopeId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Envelope does not belong to the current user", envelope.RolloverEnvelopeId);
            }
            // If a new transfer target was given, make sure that sinking fund envelope belongs to the user too
            if (envelope.TransferEnvelopeId.HasValue && !await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(envelope.TransferEnvelopeId.Value, envelope.UserId))
            {
                return new BaseIdResponse(403, "Transfer envelope does not belong to the current user");
            }

            // Loop through each field to see if an update is necessary
            foreach (PropertyInfo property in typeof(UpdateRolloverEnvelopeRequest).GetProperties())
            {
                // Skip the ids
                if (property.Name == "RolloverEnvelopeId" || property.Name == "UserId") continue;
                object? propertyValue = property.GetValue(envelope);

                // Check if the property is IOptional and has a value
                if (propertyValue is IOptional optional && optional.HasValue)
                {
                    // Add the Optional<T> to the query
                    updates.Add($"{property.Name.Underscore()} = @{property.Name}");
                }
                // Check if the property is null
                else if (propertyValue != null)
                {
                    // Add the property to the query
                    updates.Add($"{property.Name.Underscore()} = @{property.Name}");
                }
            }

            // If no fields to update, return early
            if (updates.Count == 0)
            {
                return new BaseIdResponse(400, "No fields to update");
            }
            // Assemble the query
            query = $"""
                UPDATE rollover_envelope 
                SET {string.Join(", ", updates)}
                WHERE rollover_envelope_id = @RolloverEnvelopeId
                """;
            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, envelope);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_rollover_envelope_name'"))
                    {
                        return new BaseIdResponse(400, "Envelope name already exists in this category");
                    }
                }
                return new BaseIdResponse(500, $"{ex.Number}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Return the issue
                return new BaseIdResponse(500, $"{ex.Message}");
            }

            // Make sure the row was affected
            if (rowsAffected == 1)
            {
                return new BaseIdResponse(200, "Envelope updated successfully", envelope.RolloverEnvelopeId);
            }
            else if (rowsAffected == 0)
            {
                return new BaseIdResponse(404, "Envelope not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
            }
        }

        /// <summary>
        /// DAO method to delete a rollover envelope's persistent definition.
        /// Cascades to remove every month's instance of it as well (ON DELETE CASCADE).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteRolloverEnvelopeAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM rollover_envelope 
                WHERE rollover_envelope_id = @RolloverEnvelopeId
                """;
            int rowsAffected;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsRolloverEnvelopeAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Envelope does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { RolloverEnvelopeId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the envelope was not found
                return new BaseResponse(404, "Envelope not found");
            }
            // Return the success
            return new BaseResponse(200, "Envelope deleted successfully");
        }
    }
}