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
    public class PaymentTransactionDAO
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
        public PaymentTransactionDAO(MySqlConnection connection, AuthorizationService authService, AccountDAO accountDAO, EnvelopeDAO envelopeDAO, TransactionDAO transactionDAO)
        {
            _connection = connection;
            _authService = authService;
            _accountDAO = accountDAO;
            _envelopeDAO = envelopeDAO;
            _transactionDAO = transactionDAO;
        }

        /// <summary>
        /// DAO method to add a new payment transaction
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <returns></returns>
        public async Task<CreatePaymentTransactionResponse> CreatePaymentTransactionAsync(CreatePaymentTransactionRequest paymentTransaction)
        {
            /*
             * Query 1: Create Transaction
             * Query 2: Create Payment Transaction
             * Query 3: Update account balance
             * Query 4: Add transaction split
             * Query 5: Update envelope remaining amount
             */
            // Declare and initialize
            CreatePaymentTransactionResponse response = new CreatePaymentTransactionResponse();
            int transactionId;
            int paymentTransactionId;
            int transactionSplitId;

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
                    if (!await _authService.VerifyUserOwnsBudgetAsync(paymentTransaction.BudgetId, paymentTransaction.UserId, dbTransaction))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new CreatePaymentTransactionResponse(403, "Budget does not belong to the current user");
                    }
                    // Set up the query for adding a paycheck
                    query = """
                        INSERT INTO transaction (budget_id, transaction_date, total_amount, is_reconciled, notes, transaction_type_id)
                        VALUES (@BudgetId, @TransactionDate, @TotalAmount, false, @Notes, 2);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Execute the query and get the transaction id
                    transactionId = await _connection.QuerySingleAsync<int>(query, paymentTransaction, dbTransaction);
                    // Set the transaction id in the payment transaction
                    paymentTransaction.TransactionId = transactionId;
                    // Set the transaction id in the response
                    response.Id = transactionId;

                    // Set up the query for adding the payment transaction
                    query = """
                        INSERT INTO payment_transaction (transaction_id, account_id, payment_transaction_type_id, merchant_source_name, check_number)
                        VALUES (@TransactionId, @AccountId, @PaymentTransactionType, @MerchantSourceName, @CheckNumber);
                        SELECT LAST_INSERT_ID();
                        """;
                    paymentTransactionId = await _connection.QuerySingleAsync<int>(query, paymentTransaction, dbTransaction);
                    // Set the payment transaction id in the response
                    response.PaymentTransactionId = paymentTransactionId;
                    // Update the balance of the account
                    if (!await _accountDAO.UpdateAccountBalanceAsync(paymentTransaction.UserId, paymentTransaction.AccountId, paymentTransaction.TotalAmount, dbTransaction, _connection))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new CreatePaymentTransactionResponse(500, "There was an issue updating the account balance");
                    }
                    // Loop through the transaction splits
                    foreach (CreateTransactionSplitRequest split in paymentTransaction.TransactionSplits)
                    {
                        // Set the transaction id in the split model
                        split.TransactionId = transactionId;
                        // Set up the query for adding a transaction split
                        query = """
                            INSERT INTO transaction_split (transaction_id, envelope_id, amount)
                            VALUES (@TransactionId, @EnvelopeId, @Amount);
                            SELECT LAST_INSERT_ID();
                            """;
                        // Execute the query
                        transactionSplitId = await _connection.QuerySingleAsync<int>(query, split, dbTransaction);
                        // Update the remaining amount of the envelope
                        if (!await _envelopeDAO.UpdateEnvelopeRemainingAmountAsync(paymentTransaction.UserId, split.EnvelopeId, split.Amount, dbTransaction, _connection))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new CreatePaymentTransactionResponse(500, "There was an issue updating the remaining envelope amount");
                        }
                        // Add the new id to the response split id list
                        response.TransactionSplitIds.Add(transactionSplitId);
                    }
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new CreatePaymentTransactionResponse(500, $"{ex.Message}");
                }
            }
            // Set the status and the message
            response.HttpStatus = 201;
            response.Message = "Payment transaction created successfully";
            // Return the result
            return response;
        }

        public async Task<GetPaymentTransactionResponse> GetPaymentTransactionAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetPaymentTransactionResponse response = new GetPaymentTransactionResponse();
            GetPaymentTransactionDTOModel responseDTO;
            
            // Make sure the user owns the transaction
            if (!await _authService.VerifyUserOwnsTransactionAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetPaymentTransactionResponse(403, "Payment transaction does not belong to the current user");
            }
            // Set up the query for getting the payment transaction
            query = """
                SELECT 
                    transaction.transaction_id AS TransactionId,
                    transaction.budget_id AS BudgetId,
                    transaction.transaction_date AS Date,
                    transaction.total_amount AS TotalAmount,
                    transaction.is_reconciled AS IsReconciled,
                    transaction.notes AS Notes,
                    transaction.transaction_type_id AS TransactionTypeId,
                    payment_transaction.payment_transaction_id AS PaymentTransactionId,
                    payment_transaction.account_id AS AccountId,
                    payment_transaction.payment_transaction_type_id AS PaymentTransactionTypeId,
                    payment_transaction.merchant_source_name AS MerchantSourceName,
                    payment_transaction.check_number AS CheckNumber
                FROM transaction
                INNER JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                WHERE payment_transaction.payment_transaction_id = @PaymentTransactionId
                  AND transaction.deleted_at IS NULL
                """;
            // Get the transaction
            responseDTO = await _connection.QueryFirstAsync<GetPaymentTransactionDTOModel>(query, new { PaymentTransactionId = request.EntityId });

            // Make sure the payment transaction was found
            if (responseDTO == null)
            {
                // Return the issue
                return new GetPaymentTransactionResponse(404, "Payment transaction not found");
            }

            // Set up the query to get the list of transaction splits
            query = """
                SELECT 
                    transaction_split.transaction_split_id AS TransactionSplitId,
                    transaction_split.transaction_id AS TransactionId,
                    transaction_split.envelope_id AS EnvelopeId,
                    transaction_split.amount AS Amount
                FROM transaction_split
                WHERE transaction_split.transaction_id = @PaymentTransactionId
                """;
            // Execute the query to get the list of splits
            responseDTO.TransactionSplits = (await _connection.QueryAsync<GetTransactionSplitDTOModel>(query, new { PaymentTransactionId = request.EntityId })).ToList();

            // Set the response DTO in the response
            response.PaymentTransaction = responseDTO;
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Payment transaction found";
            // Return the response
            return response;
        }


        public async Task<UpdatePaymentTransactionResponse> UpdatePaymentTransactionAsync(UpdatePaymentTransactionRequest paymentTransaction)
        {
            // Declare and initialize
            UpdatePaymentTransactionResponse response = new UpdatePaymentTransactionResponse();
            List<string> updates = new List<string>();
            List<int> existingSplitIds;
            List<int> incomingSplitIds;
            List<int> splitsToDelete;
            int rowsAffected;
            int transactionSplitId;
            BaseIdResponse transactionResponse;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Setting up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Call the transaction update method
                transactionResponse = await _transactionDAO.UpdateTransactionAsync(paymentTransaction, dbTransaction);

                // Check the transaction response
                if (transactionResponse.HttpStatus != 200 || transactionResponse.Message != "No fields to update")
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new UpdatePaymentTransactionResponse(transactionResponse.HttpStatus, transactionResponse.Message);
                }

                // Loop through each field to see if an update is necessary
                foreach (PropertyInfo property in typeof(UpdatePaymentTransactionRequest).GetProperties())
                {
                    // Skip base table properties
                    if (typeof(UpdateTransactionRequest).GetProperty(property.Name) != null) continue;
                    // Skip the id
                    if (property.Name == "PaymentTransactionId") continue;
                    object? propertyValue = property.GetValue(paymentTransaction);

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
                    return new UpdatePaymentTransactionResponse(400, "No fields to update");
                }
                // Assemble the query
                query = $"""
                    UPDATE payment_transaction 
                    SET {string.Join(", ", updates)}
                    WHERE payment_transaction_id = @PaymentTransactionId
                    """;

                try
                {
                    // Execute the query
                    rowsAffected = await _connection.ExecuteAsync(query, paymentTransaction, dbTransaction);
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new UpdatePaymentTransactionResponse(500, $"{ex.Message}");
                }

                if (rowsAffected == 0)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new UpdatePaymentTransactionResponse(404, "Payment transaction not found");
                }

                // Update the transaction splits table
                if (paymentTransaction.TransactionSplits != null)
                {
                    // Get the existing splits from the database
                    query = """
                        SELECT transaction_split_id AS TransactionSplitId 
                        FROM transaction_split 
                        WHERE transaction_id = @TransactionId
                        """;
                    try
                    {
                        // Return the ids to an int list
                        existingSplitIds = (await _connection.QueryAsync<int>(query, new { TransactionId = paymentTransaction.TransactionId }, dbTransaction)).ToList();
                    }
                    catch (Exception ex)
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        // Return the issue
                        return new UpdatePaymentTransactionResponse(500, $"{ex.Message}");
                    }
                    // Get the incoming slit ids
                    incomingSplitIds = paymentTransaction.TransactionSplits.Where(s => s.TransactionSplitId != null).Select(s => s.TransactionSplitId!.Value).ToList();

                    // Get the splits to delete by comparing the existing and incoming splits
                    splitsToDelete = existingSplitIds.Except(incomingSplitIds).ToList();
                    // Check if there are splits to delete
                    if (splitsToDelete.Count > 0)
                    {
                        query = """
                            DELETE FROM transaction_split 
                            WHERE transaction_split_id 
                            IN @SplitIds
                            """;
                        await _connection.QueryAsync(query, new { SplitIds = splitsToDelete }, dbTransaction);
                    }

                    // Loop through the splits to insert and update
                    foreach (UpdateTransactionSplitRequest split in paymentTransaction.TransactionSplits)
                    {
                        // Check if the transaction split id is null
                        if (split.TransactionSplitId.HasValue)
                        {
                            // Update the existing split
                            query = """
                                UPDATE transaction_split 
                                SET envelope_id = @EnvelopeId, 
                                    amount = @Amount
                                WHERE paycheck_split_id = @PaycheckSplitId
                                """;
                            try
                            {
                                // Return the ids to an int list
                                existingSplitIds = (await _connection.QueryAsync<int>(query, split, dbTransaction)).ToList();
                            }
                            catch (Exception ex)
                            {
                                // Roll the transaction back
                                dbTransaction.Rollback();
                                // Return the issue
                                return new UpdatePaymentTransactionResponse(500, $"{ex.Message}");
                            }
                        }
                        else
                        {
                            // Set the transaction id
                            split.TransactionId = paymentTransaction.TransactionId;
                            // Add a new split
                            query = """
                                INSERT INTO transaction_split (transaction_id, envelope_id, amount)
                                VALUES (@TransactionId, @EnvelopeId, @Amount);
                                SELECT LAST_INSERT_ID();
                            """;
                            // Execute the query
                            transactionSplitId = await _connection.QuerySingleAsync<int>(query, split, dbTransaction);
                            // Add the new id to the response split id list
                            response.TransactionSplitIds.Add(transactionSplitId);
                        }
                    }
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Set the status and the message
            response.HttpStatus = 201;
            response.Message = "Payment transaction updated successfully";
            // Return the result
            return response;
        }
    }
}
