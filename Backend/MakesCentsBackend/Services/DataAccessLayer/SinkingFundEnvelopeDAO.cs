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
    /// Data access object for sinking fund envelopes.
    /// Sinking fund envelopes are persistent - one row per envelope, with a live
    /// remaining_amount that carries forward month to month (no monthly instance table,
    /// unlike rollover envelopes).
    /// </summary>
    public class SinkingFundEnvelopeDAO
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
        public SinkingFundEnvelopeDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create a sinking fund envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateSinkingFundEnvelopeAsync(CreateSinkingFundEnvelopeRequest envelope)
        {
            // Declare and initialize
            query = """
                INSERT INTO sinking_fund_envelope (envelope_category_id, envelope_name, remaining_amount, goal_amount, goal_end_date)
                VALUES (@EnvelopeCategoryId, @EnvelopeName, @RemainingAmount, @GoalAmount, @GoalEndDate);
                SELECT LAST_INSERT_ID();
                """;
            int sinkingFundEnvelopeId;

            // Make sure the envelope category belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeCategoryAsync(envelope.EnvelopeCategoryId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Envelope category does not belong to the current user");
            }
            // Execute the request and get the new id for the envelope
            try
            {
                // Execute
                sinkingFundEnvelopeId = await _connection.ExecuteScalarAsync<int>(query, envelope);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_sinking_envelope_name'"))
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
            return new BaseIdResponse(201, "Envelope created successfully", sinkingFundEnvelopeId);
        }

        /// <summary>
        /// DAO method to update the remaining amount of a sinking fund envelope by the amount
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sinkingFundEnvelopeId"></param>
        /// <param name="amount"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRemainingAmountAsync(int? userId, int? sinkingFundEnvelopeId, decimal? amount, MySqlTransaction dbTransaction, MySqlConnection connection)
        {
            // Declare and initialize
            query = """
                UPDATE sinking_fund_envelope
                SET remaining_amount = remaining_amount + @Amount
                WHERE sinking_fund_envelope_id = @SinkingFundEnvelopeId;
                """;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(sinkingFundEnvelopeId, userId, dbTransaction))
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
                    SinkingFundEnvelopeId = sinkingFundEnvelopeId
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
        /// DAO method to get a single sinking fund envelope, including its transaction history
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetSinkingFundEnvelopeEntityResponse> GetSinkingFundEnvelopeAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetSinkingFundEnvelopeEntityResponse response = new GetSinkingFundEnvelopeEntityResponse();
            GetSinkingFundEnvelopeEntityModel responseEntity;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetSinkingFundEnvelopeEntityResponse(403, "Envelope does not belong to the current user");
            }
            // Set up the query to get the envelope
            query = """
                SELECT 
                    sinking_fund_envelope.sinking_fund_envelope_id AS SinkingFundEnvelopeId,
                    sinking_fund_envelope.envelope_category_id AS EnvelopeCategoryId,
                    sinking_fund_envelope.envelope_name AS EnvelopeName,
                    sinking_fund_envelope.remaining_amount AS RemainingAmount,
                    sinking_fund_envelope.goal_amount AS GoalAmount,
                    sinking_fund_envelope.goal_end_date AS GoalEndDate
                FROM sinking_fund_envelope
                WHERE sinking_fund_envelope.sinking_fund_envelope_id = @SinkingFundEnvelopeId
                """;
            // Read the envelope
            responseEntity = await _connection.QuerySingleAsync<GetSinkingFundEnvelopeEntityModel>(query, new { SinkingFundEnvelopeId = request.EntityId });
            // Make sure the envelope was found
            if (responseEntity == null)
            {
                // Return the issue
                return new GetSinkingFundEnvelopeEntityResponse(404, "Envelope not found");
            }
            // Set up the query for reading the payment transactions that touched this envelope
            // (self-join ts2 pulls every envelope name on the transaction, same pattern as before the schema split)
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
                WHERE transaction_split.sinking_fund_envelope_id = @SinkingFundEnvelopeId
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
            responseEntity.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { SinkingFundEnvelopeId = request.EntityId })).ToList();
            // Set up the query to read envelope-to-envelope transfers touching this envelope.
            // v3 no longer stores these on transfer_transaction - they're two opposite-signed
            // transaction_split rows (negative = from, positive = to) tied to a transfer_transaction
            // marker row of type Envelope.
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
                WHERE (from_split.sinking_fund_envelope_id = @SinkingFundEnvelopeId OR to_split.sinking_fund_envelope_id = @SinkingFundEnvelopeId)
                AND transaction.deleted_at IS NULL
                AND transaction.transaction_type_id = 3
                AND transfer_transaction.transfer_transaction_type_id = 3
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the envelope transfer transactions
            responseEntity.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { SinkingFundEnvelopeId = request.EntityId })).ToList());
            // Set the response entity in the response
            response.SinkingFundEnvelope = responseEntity;
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Envelope found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to get a single sinking fund envelope without its transaction history (for update flows)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetSinkingFundEnvelopeEntityResponse> GetSinkingFundEnvelopeForUpdateAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetSinkingFundEnvelopeEntityResponse response = new GetSinkingFundEnvelopeEntityResponse();

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetSinkingFundEnvelopeEntityResponse(404, "Envelope not found");
            }
            // Set up the query to get the envelope
            query = """
                SELECT 
                    sinking_fund_envelope.sinking_fund_envelope_id AS SinkingFundEnvelopeId,
                    sinking_fund_envelope.envelope_category_id AS EnvelopeCategoryId,
                    sinking_fund_envelope.envelope_name AS EnvelopeName,
                    sinking_fund_envelope.remaining_amount AS RemainingAmount,
                    sinking_fund_envelope.goal_amount AS GoalAmount,
                    sinking_fund_envelope.goal_end_date AS GoalEndDate
                FROM sinking_fund_envelope
                WHERE sinking_fund_envelope.sinking_fund_envelope_id = @SinkingFundEnvelopeId
                """;
            // Read the envelope
            response.SinkingFundEnvelope = await _connection.QuerySingleAsync<GetSinkingFundEnvelopeEntityModel>(query, new { SinkingFundEnvelopeId = request.EntityId });
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Envelope found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to update a sinking fund envelope based on given fields
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateSinkingFundEnvelopeAsync(UpdateSinkingFundEnvelopeRequest envelope)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(envelope.SinkingFundEnvelopeId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Envelope does not belong to the current user", envelope.SinkingFundEnvelopeId);
            }

            // Loop through each field to see if an update is necessary
            foreach (PropertyInfo property in typeof(UpdateSinkingFundEnvelopeRequest).GetProperties())
            {
                // Skip the ids
                if (property.Name == "SinkingFundEnvelopeId" || property.Name == "UserId") continue;
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
                UPDATE sinking_fund_envelope 
                SET {string.Join(", ", updates)}
                WHERE sinking_fund_envelope_id = @SinkingFundEnvelopeId
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
                    if (ex.Message.Contains("'unique_sinking_envelope_name'"))
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
                return new BaseIdResponse(200, "Envelope updated successfully", envelope.SinkingFundEnvelopeId);
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
        /// DAO method to get the rollover envelopes that use this sinking fund envelope as their
        /// transfer target - these have to be pointed at a different sinking fund before this
        /// one can be deleted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetSinkingFundDependentsResponse> GetRolloverDependentsAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetSinkingFundDependentsResponse response = new GetSinkingFundDependentsResponse();

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(request.EntityId, request.UserId))
            {
                return new GetSinkingFundDependentsResponse(403, "Envelope does not belong to the current user");
            }
            // Set up the query to get the rollover envelopes sweeping into this envelope
            query = """
                SELECT 
                    rollover_envelope.rollover_envelope_id AS RolloverEnvelopeId,
                    rollover_envelope.envelope_name AS EnvelopeName
                FROM rollover_envelope
                WHERE rollover_envelope.transfer_envelope_id = @SinkingFundEnvelopeId
                ORDER BY rollover_envelope.envelope_name;
                """;
            // Execute the query
            response.RolloverEnvelopes = (await _connection.QueryAsync<RolloverEnvelopeDependentModel>(query, new { SinkingFundEnvelopeId = request.EntityId })).ToList();

            // Set the status and message for the response
            response.HttpStatus = 200;
            response.Message = "Dependent rollover envelopes found";
            return response;
        }

        /// <summary>
        /// DAO method to delete a sinking fund envelope
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteSinkingFundEnvelopeAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM sinking_fund_envelope 
                WHERE sinking_fund_envelope_id = @SinkingFundEnvelopeId
                """;
            int rowsAffected;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Envelope does not belong to the current user");
            }
            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, new { SinkingFundEnvelopeId = request.EntityId });
            }
            catch (MySqlException ex)
            {
                // 1451 = row is still referenced by another table
                if (ex.Number == 1451)
                {
                    return new BaseResponse(400, "This envelope is still in use (transactions, funding rows, or a rollover envelope's transfer target) and can't be deleted");
                }
                return new BaseResponse(500, $"{ex.Number}: {ex.Message}");
            }

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