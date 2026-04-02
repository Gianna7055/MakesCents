/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using Humanizer;
using MakesCentsBackend.Models;
using MakesCentsBackend.Models.Enums;
using MySqlConnector;
using System.Data;
using System.Reflection;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class TransferTransactionDAO
    {
        // Class level variables
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        private readonly AccountDAO _accountDAO;
        private readonly EnvelopeDAO _envelopeDAO;
        private readonly TransactionDAO _transactionDAO;
        string query = "";

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public TransferTransactionDAO(MySqlConnection connection, AuthorizationService authService, AccountDAO accountDAO, EnvelopeDAO envelopeDAO, TransactionDAO transactionDAO)
        {
            _connection = connection;
            _authService = authService;
            _accountDAO = accountDAO;
            _envelopeDAO = envelopeDAO;
            _transactionDAO = transactionDAO;
        }


        public async Task<CreateTransferTransactionResponse> CreateTransferTransactionAsync(CreateTransferTransactionRequest transferTransaction)
        {
            /*
             * Query 1: Create Transaction
             * Query 2: Create Transfer Transaction
             * Query 3: Update from Envelope/Account
             * Query 4: Update to Envelope/Account
             */
            // Declare and initialize
            CreateTransferTransactionResponse response = new CreateTransferTransactionResponse();
            int transactionId;
            int transferTransactionId;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
            {
                // Open the connection
                await _connection.OpenAsync();
            }
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                try
                {
                    // Make sure the budget belongs to the user
                    if (!await _authService.VerifyUserOwnsBudgetAsync(transferTransaction.BudgetId, transferTransaction.UserId, dbTransaction))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new CreateTransferTransactionResponse(403, "Budget does not belong to the current user");
                    }
                    // Set up the query for adding a paycheck
                    query = """
                        INSERT INTO transaction (budget_id, transaction_date, total_amount, is_reconciled, notes, transaction_type_id)
                        VALUES (@BudgetId, @TransactionDate, @TotalAmount, false, @Notes, 3);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Execute the query and get the transaction id
                    transactionId = await _connection.QuerySingleOrDefaultAsync<int>(query, transferTransaction, dbTransaction);
                    // Set the transaction id in the transfer transaction
                    transferTransaction.TransactionId = transactionId;
                    // Set the transaction id in the response
                    response.Id = transactionId;

                    // Check if the transfer transaction if is for accounts or envelopes
                    if (transferTransaction.TransferTransactionType == TransferTransactionType.Account)
                    {
                        // Makes sure both accounts belong to the user
                        if (!await _authService.VerifyUserOwnsAccountAsync(transferTransaction.TransferFromId, transferTransaction.UserId, dbTransaction) || !await _authService.VerifyUserOwnsAccountAsync(transferTransaction.TransferToId, transferTransaction.UserId, dbTransaction))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new CreateTransferTransactionResponse(403, "One or both accounts do not belong to the current user");
                        }
                        // Set up the query for the transfer transaction
                        query = """
                            INSERT INTO transfer_transaction (transaction_id, transfer_from_account_id, transfer_to_account_id, transfer_transaction_type_id)
                            VALUES (@TransactionId, @TransferFromId, @TransferToId, 2);
                            SELECT LAST_INSERT_ID();
                        """;
                        // Execute the query
                        transferTransactionId = await _connection.QuerySingleAsync<int>(query, transferTransaction, dbTransaction);
                        // Set the transfer transaction id in the response
                        response.TransferTransactionId = transferTransactionId;

                        // Update the to and from accounts
                        if (!await _accountDAO.UpdateAccountBalanceAsync(transferTransaction.UserId, transferTransaction.TransferFromId, -transferTransaction.TotalAmount, dbTransaction, _connection) || !await _accountDAO.UpdateAccountBalanceAsync(transferTransaction.UserId, transferTransaction.TransferToId, transferTransaction.TotalAmount, dbTransaction, _connection))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new CreateTransferTransactionResponse(400, "There was an issue moving the amount between accounts");
                        }
                    }
                    else if (transferTransaction.TransferTransactionType == TransferTransactionType.Envelope)
                    {
                        // Makes sure both envelope belong to the user
                        if (!await _authService.VerifyUserOwnsEnvelopeAsync(transferTransaction.TransferFromId, transferTransaction.UserId, dbTransaction) || !await _authService.VerifyUserOwnsEnvelopeAsync(transferTransaction.TransferToId, transferTransaction.UserId, dbTransaction))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new CreateTransferTransactionResponse(403, "One or both envelopes do not belong to the current user");
                        }
                        // Set up the query for the transfer transaction
                        query = """
                            INSERT INTO transfer_transaction (transaction_id, transfer_from_envelope_id, transfer_to_envelope_id, transfer_transaction_type_id)
                            VALUES (@TransactionId, @TransferFromId, @TransferToId, 3);
                            SELECT LAST_INSERT_ID();
                        """;
                        // Execute the query
                        transferTransactionId = await _connection.QuerySingleAsync<int>(query, transferTransaction, dbTransaction);
                        // Set the transfer transaction id in the response
                        response.TransferTransactionId = transferTransactionId;

                        // Update the to and from envelopes
                        if (!await _envelopeDAO.UpdateEnvelopeRemainingAmountAsync(transferTransaction.UserId, transferTransaction.TransferFromId, -transferTransaction.TotalAmount, dbTransaction, _connection) || !await _envelopeDAO.UpdateEnvelopeRemainingAmountAsync(transferTransaction.UserId, transferTransaction.TransferToId, transferTransaction.TotalAmount, dbTransaction, _connection))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new CreateTransferTransactionResponse(400, "There was an issue moving the amount between envelopes");
                        }
                    }
                    else
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new CreateTransferTransactionResponse(403, "There was an issue creating your transfer transaction");
                    }
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new CreateTransferTransactionResponse(500, $"{ex.Message}");
                }
                // Commit the transaction
                dbTransaction.Commit(); 
            }
            // Set the status and the message
            response.HttpStatus = 201;
            response.Message = "Transfer transaction created successfully";
            // Return the result
            return response; 
        }


        public async Task<GetTransferTransactionEntityResponse> GetTransferTransactionAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetTransferTransactionEntityResponse response = new GetTransferTransactionEntityResponse();

            // Make sure the user owns the transaction
            if (!await _authService.VerifyUserOwnsTransactionAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetTransferTransactionEntityResponse(403, "Transfer transaction does not belong to the current user");
            }
            // Set up the query to get the transfer transaction
            query = """
                SELECT 
                    transaction.transaction_id AS TransactionId,
                    transaction.budget_id AS BudgetId,
                    transaction.transaction_date AS Date,
                    transaction.total_amount AS TotalAmount,
                    transaction.is_reconciled AS IsReconciled,
                    transaction.notes AS Notes,
                    transaction.transaction_type_id AS TransactionTypeId,
                    transfer_transaction.transfer_transaction_id AS TransferTransactionId,
                    transfer_transaction.transfer_from_account_id AS TransferFromAccountId,
                    transfer_transaction.transfer_to_account_id AS TransferToAccountId,
                    transfer_transaction.transfer_from_envelope_id AS TransferFromEnvelopeId,
                    transfer_transaction.transfer_to_envelope_id AS TransferToEnvelopeId,
                    transfer_transaction.transfer_transaction_type_id AS TransferTransactionTypeId
                FROM transaction
                INNER JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                WHERE transaction.transaction_id = @TransactionId
                  AND transaction.deleted_at IS NULL
                """;
            // Execute the query to get the entity model
            response.TransferTransaction = await _connection.QuerySingleAsync<GetTransferTransactionEntityModel>(query, new { TransactionId = request.EntityId });

            // Make sure the payment transaction was found
            if (response.TransferTransaction == null)
            {
                // Return the issue
                return new GetTransferTransactionEntityResponse(404, "Transfer transaction not found");
            }

            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Transfer transaction found";
            // Return the response
            return response;
        }


        public async Task<BaseIdResponse> UpdateTransferTransactionAsync(UpdateTransferTransactionRequest transferTransaction)
        {
            // Declare and initialize
            BaseIdResponse transactionResponse;
            int rowsAffected;
            List<string> updates = new List<string>();

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Setting up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Call the transaction update method
                transactionResponse = await _transactionDAO.UpdateTransactionAsync(transferTransaction, dbTransaction);

                // Check the transaction response
                if (transactionResponse.HttpStatus != 200 || transactionResponse.Message != "No fields to update")
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return transactionResponse;
                }

                // Fetch the current transfer transaction
                query = """
                    SELECT 
                        transfer_transaction.transfer_transaction_type_id AS TransferTransactionTypeId,
                        transfer_transaction.transfer_from_account_id AS TransferFromAccountId,
                        transfer_transaction.transfer_to_account_id AS TransferToAccountId,
                        transfer_transaction.transfer_from_envelope_id AS TransferFromEnvelopeId,
                        transfer_transaction.transfer_to_envelope_id AS TransferToEnvelopeId
                    FROM transfer_transaction
                    WHERE transfer_transaction_id = @TransferTransactionId
                    """;
                GetTransferTransactionEntityModel? current = await _connection.QuerySingleOrDefaultAsync<GetTransferTransactionEntityModel>(query, new { transferTransaction.TransferTransactionId }, dbTransaction);

                if (current == null)
                {
                    dbTransaction.Rollback();
                    return new BaseIdResponse(404, "Transfer transaction not found");
                }

                // Get the correct transfer transaction type
                TransferTransactionType? currentType = transferTransaction.TransferTransactionType != null ? transferTransaction.TransferTransactionType : current.TransferTransactionType;

                // Check the transfer transaction type
                if (currentType == TransferTransactionType.Account)
                {
                    int? currentFromId = transferTransaction.TransferFromId != null ? transferTransaction.TransferFromId : current.TransferFromAccountId;
                    int? currentToId = transferTransaction.TransferToId != null ? transferTransaction.TransferToId : current.TransferToAccountId;

                    // Makes sure both accounts belong to the user
                    if (!await _authService.VerifyUserOwnsAccountAsync(currentFromId, transferTransaction.UserId, dbTransaction) || !await _authService.VerifyUserOwnsAccountAsync(currentToId, transferTransaction.UserId, dbTransaction))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new BaseIdResponse(403, "One or both accounts do not belong to the current user");
                    }
                }
                else // Current type is envelope
                {
                    int? currentFromId = transferTransaction.TransferFromId != null ? transferTransaction.TransferFromId : current.TransferFromEnvelopeId;
                    int? currentToId = transferTransaction.TransferToId != null ? transferTransaction.TransferToId : current.TransferToEnvelopeId;

                    // Makes sure both envelope belong to the user
                    if (!await _authService.VerifyUserOwnsEnvelopeAsync(currentFromId, transferTransaction.UserId, dbTransaction) || !await _authService.VerifyUserOwnsEnvelopeAsync(currentToId, transferTransaction.UserId, dbTransaction))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new BaseIdResponse(403, "One or both envelopes do not belong to the current user");
                    }
                }


                // Loop through each field to see if an update is necessary
                foreach (PropertyInfo property in typeof(UpdateTransferTransactionRequest).GetProperties())
                {
                    // Skip base table properties
                    if (typeof(UpdateTransactionRequest).GetProperty(property.Name) != null) continue;
                    // Skip the id
                    if (property.Name == "TransferTransactionId") continue;
                    object? propertyValue = property.GetValue(transferTransaction);

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

                // If there are no fields to update, return early
                if (updates.Count == 0)
                {
                    return new BaseIdResponse(400, "No fields to update");
                }
                // Assemble the query
                query = $"""
                    UPDATE transfer_transaction 
                    SET {string.Join(", ", updates)}
                    WHERE transfer_transaction_id = @TransferTransactionId
                    """;

                try
                {
                    // Execute the query
                    rowsAffected = await _connection.ExecuteAsync(query, transferTransaction, dbTransaction);
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new BaseIdResponse(500, $"{ex.Message}");
                }

                if (rowsAffected == 0)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new BaseIdResponse(404, "Payment transaction not found");
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Return the result
            return new BaseIdResponse(201, "Transfer transaction updated successfully");
        }
    }
}
