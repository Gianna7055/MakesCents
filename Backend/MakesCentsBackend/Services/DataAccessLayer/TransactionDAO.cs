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
using System.Security.Principal;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class TransactionDAO
    {
        // Class level variables
        // string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        string query = "";

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public TransactionDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }


        public async Task<GetAllTransactionsEntityResponse> GetAllTransactionsAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllTransactionsEntityResponse response = new GetAllTransactionsEntityResponse();

            // Set up the query for reading the payment transactions
            query = """
                SELECT
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionType,
                    transaction.total_amount AS TotalAmount,
                    payment_transaction.merchant_source_name AS MerchantSourceName,
                    GROUP_CONCAT(DISTINCT envelope.envelope_name SEPARATOR ', ') AS EnvelopeNames
                FROM transaction
                INNER JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                LEFT JOIN transaction_split ON transaction.transaction_id = transaction_split.transaction_id
                LEFT JOIN envelope ON transaction_split.envelope_id = envelope.envelope_id
                WHERE transaction.budget_id = @BudgetId
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
            response.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { BudgetId = request.EntityId })).ToList();
            // Set up the query to read the transfer transactions
            query = """"
                SELECT
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionType,
                    transaction.total_amount AS TotalAmount,
                    transfer_transaction.transfer_transaction_type_id AS TransferTransactionType,
                    from_account.account_name AS TransferFromAccount,
                    to_account.account_name AS TransferToAccount,
                    from_envelope.envelope_name AS TransferFromEnvelope,
                    to_envelope.envelope_name AS TransferToEnvelope
                FROM transaction
                INNER JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                LEFT JOIN account AS from_account ON transfer_transaction.transfer_from_account_id = from_account.account_id
                LEFT JOIN account AS to_account ON transfer_transaction.transfer_to_account_id = to_account.account_id
                LEFT JOIN envelope AS from_envelope ON transfer_transaction.transfer_from_envelope_id = from_envelope.envelope_id
                LEFT JOIN envelope AS to_envelope ON transfer_transaction.transfer_to_envelope_id = to_envelope.envelope_id
                WHERE transaction.budget_id = @BudgetId
                  AND transaction.deleted_at IS NULL
                  AND transaction.transaction_type_id = 3
                ORDER BY transaction.transaction_date DESC;
                """";
            // Read the transfer transactions
            List<SummaryTransactionEntityModel> transferTransactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { BudgetId = request.EntityId })).ToList();
            response.Transactions.AddRange(transferTransactions);
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Transactions found";
            // Return the response
            return response;
        }


        public async Task<BaseIdResponse> UpdateTransactionAsync(UpdateTransactionRequest transaction, MySqlTransaction dbTransaction)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the transaction belongs to the user
            if (!await _authService.VerifyUserOwnsTransactionAsync(transaction.TransactionId, transaction.UserId))
            {
                // Return the issue
                return new BaseIdResponse(403, "Transaction does not belong to the current user", transaction.TransactionId);
            }

            // Loop through each file to see if an update is necessary
            foreach (PropertyInfo property in typeof(UpdateTransactionRequest).GetProperties())
            {
                // Skip the ids
                if (property.Name == "TransactionId" || property.Name == "UserId") continue;
                object? propertyValue = property.GetValue(transaction);

                // check if the property is IOptional and has a value
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
                UPDATE transaction 
                SET {string.Join(", ", updates)}
                WHERE transaction_id = @TransactionId
                """;

            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, transaction, dbTransaction);
            }
            catch (Exception ex)
            {
                // Roll the transaction back
                dbTransaction.Rollback();
                // Return the issue
                return new BaseIdResponse(500, $"{ex.Message}");
            }

            // Make sure the row was affected
            if (rowsAffected == 1)
            {
                return new BaseIdResponse(200, "Transaction updated successfully", transaction.TransactionId);
            }
            else if (rowsAffected == 0)
            {
                // Roll the transaction back
                dbTransaction.Rollback();
                return new BaseIdResponse(404, "Transaction not found");
            }
            else
            {
                // Roll the transaction back
                dbTransaction.Rollback();
                return new BaseIdResponse(400, "An error occurred");
            }
        }


        public async Task<BaseResponse> DeleteTransactionAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM transaction 
                WHERE transaction_id = @TransactionId
                """;
            int rowsAffected;

            // Make sure the transaction belongs to the user
            if (!await _authService.VerifyUserOwnsTransactionAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Transaction does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { TransactionId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the user was not found
                return new BaseResponse(404, "Transaction not found");
            }
            // Return the success
            return new BaseResponse(200, "Transaction deleted successfully");
        }
    }
}
