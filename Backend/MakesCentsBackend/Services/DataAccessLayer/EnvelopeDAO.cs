/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using Humanizer;
using MakesCentsBackend.Models;
using MySqlConnector;
using System.Reflection;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class EnvelopeDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public EnvelopeDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create an envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateEnvelopeAsync(CreateEnvelopeRequest envelope)
        {
            // Declare and initialize
            query = """
                INSERT INTO envelope (envelope_category_id, envelope_name, planned_amount, remaining_amount, is_sinking_fund, goal_amount, goal_end_date, transfer_envelope_id)
                VALUES (@EnvelopeCategoryId, @EnvelopeName, @PlannedAmount, @RemainingAmount, @IsSinkingFund, @GoalAmount, @GoalEndDate, @TransferEnvelopeId);
                SELECT LAST_INSERT_ID();
                """;
            int envelopeId;

            // Make sure the envelope category belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeCategoryAsync(envelope.EnvelopeCategoryId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Envelope category does not belong to the current user");
            }
            // Execute the request and get the new id for the envelope
            try
            {
                // Execute
                envelopeId = await _connection.ExecuteScalarAsync<int>(query, envelope);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_envelope_name'"))
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
            return new BaseIdResponse(201, "Envelope created successfully", envelopeId);
        }

        /// <summary>
        /// DAO method to update the remaining amount of an envelope by the amount
        /// </summary>
        /// <param name="dbTransaction"></param>
        /// <param name="userId"></param>
        /// <param name="envelopeId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<bool> UpdateEnvelopeRemainingAmountAsync(int? userId, int? envelopeId, decimal? amount, MySqlTransaction dbTransaction, MySqlConnection connection)
        {
            // Declare and initialize
            query = """
                UPDATE envelope
                SET remaining_amount = remaining_amount + @Amount
                WHERE envelope_id = @EnvelopeId;
                """;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeAsync(envelopeId, userId, dbTransaction))
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
                    EnvelopeId = envelopeId
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
        /// DAO method to get a single envelope
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetEnvelopeEntityResponse> GetEnvelopeAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetEnvelopeEntityResponse response = new GetEnvelopeEntityResponse();
            GetEnvelopeEntityModel responseEntity;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetEnvelopeEntityResponse(403, "Envelope does not belong to the current user");
            }
            // Set up the query to get the envelope
            query = """
                SELECT 
                    envelope.envelope_id AS EnvelopeId,
                    envelope.envelope_category_id AS EnvelopeCategoryId,
                    envelope.envelope_name AS EnvelopeName,
                    envelope.planned_amount AS PlannedAmount,
                    envelope.remaining_amount AS RemainingAmount,
                    envelope.is_sinking_fund AS IsSinkingFund,
                    envelope.goal_amount AS GoalAmount,
                    envelope.goal_end_date AS GoalEndDate,
                    envelope.transfer_envelope_id AS TransferEnvelopeId
                FROM envelope
                WHERE envelope.envelope_id = @EnvelopeId
                """;
            // Read the envelope
            responseEntity = await _connection.QuerySingleAsync<GetEnvelopeEntityModel>(query, new { EnvelopeId = request.EntityId });
            // Make sure the envelope was found
            if (responseEntity == null)
            {
                // Return the issue
                return new GetEnvelopeEntityResponse(404, "Envelope not found");
            }
            // Set up the query for reading the payment transactions
            query = """
                SELECT 
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionTypeId,
                    transaction.total_amount AS TotalAmount,
                    payment_transaction.merchant_source_name AS MerchantSourceName,
                    GROUP_CONCAT(DISTINCT envelope.envelope_name SEPARATOR ', ') AS EnvelopeNames
                FROM transaction_split
                INNER JOIN transaction ON transaction_split.transaction_id = transaction.transaction_id
                INNER JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                LEFT JOIN transaction_split AS ts2 ON transaction.transaction_id = ts2.transaction_id
                LEFT JOIN envelope ON ts2.envelope_id = envelope.envelope_id
                WHERE transaction_split.envelope_id = @EnvelopeId
                  AND transaction.deleted_at IS NULL
                  AND transaction.transaction_type_id = 2
                GROUP BY transaction.transaction_id
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the payment transactions
            responseEntity.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { EnvelopeId = request.EntityId })).ToList();
            // Set up the query to read the transfer transactions
            query = """
                SELECT 
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionTypeId,
                    transaction.total_amount AS TotalAmount,
                    transfer_transaction.transfer_from_account_id AS TransferFromAccountId,
                    transfer_transaction.transfer_to_account_id AS TransferToAccountId,
                    transfer_transaction.transfer_from_envelope_id AS TransferFromEnvelopeId,
                    transfer_transaction.transfer_to_envelope_id AS TransferToEnvelopeId
                FROM transaction_split
                INNER JOIN transaction ON transaction_split.transaction_id = transaction.transaction_id
                INNER JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                WHERE transaction_split.envelope_id = @EnvelopeId
                  AND transaction.deleted_at IS NULL
                  AND transaction.transaction_type_id = 3
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the transfer transactions
            responseEntity.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { EnvelopeId = request.EntityId })).ToList());
            // Set the response entity in the response
            response.EnvelopeEntity = responseEntity;
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Envelope found";
            // Return the response
            return response;
        }


        public async Task<GetEnvelopeEntityResponse> GetEnvelopeForUpdateAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetEnvelopeEntityResponse response = new GetEnvelopeEntityResponse();

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetEnvelopeEntityResponse(404, "Envelope not found");
            }
            // Set up the query to get the envelope
            query = """
                SELECT 
                    envelope.envelope_id AS EnvelopeId,
                    envelope.envelope_category_id AS EnvelopeCategoryId,
                    envelope.envelope_name AS EnvelopeName,
                    envelope.planned_amount AS PlannedAmount,
                    envelope.remaining_amount AS RemainingAmount,
                    envelope.is_sinking_fund AS IsSinkingFund,
                    envelope.goal_amount AS GoalAmount,
                    envelope.goal_end_date AS GoalEndDate,
                    envelope.transfer_envelope_id AS TransferEnvelopeId
                FROM envelope
                WHERE envelope.envelope_id = @EnvelopeId
                """;
            // Read the envelope
            response.EnvelopeEntity = await _connection.QuerySingleAsync<GetEnvelopeEntityModel>(query, new { EnvelopeId = request.EntityId });
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Envelope found";
            // Return the response
            return response;
        }


        public async Task<BaseIdResponse> UpdateEnvelopeAsync(UpdateEnvelopeRequest envelope)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeAsync(envelope.EnvelopeId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Envelope does not belong to the current user", envelope.EnvelopeId);
            }

            // Loop through each field to see if an update is necessary
            foreach (PropertyInfo property in typeof(UpdateEnvelopeRequest).GetProperties())
            {
                // Skip the id
                if (property.Name == "EnvelopeId" || property.Name == "UserId") continue;
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
                UPDATE envelope 
                SET {string.Join(", ", updates)}
                WHERE envelope_id = @EnvelopeId
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
                    if (ex.Message.Contains("'unique_envelope_name'"))
                    {
                        return new BaseIdResponse(400, "Envelope name already exists in this budget");
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
                return new BaseIdResponse(200, "Envelope updated successfully", envelope.EnvelopeId);
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


        public async Task<BaseResponse> DeleteEnvelopeAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM envelope 
                WHERE envelope_id = @EnvelopeId
                """;
            int rowsAffected;

            // Make sure the envelope belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Envelope does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { EnvelopeId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the user was not found
                return new BaseResponse(404, "Envelope not found");
            }
            // Return the success
            return new BaseResponse(200, "Envelope deleted successfully");
        }
    }
}
