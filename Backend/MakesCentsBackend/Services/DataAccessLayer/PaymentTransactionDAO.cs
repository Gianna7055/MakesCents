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
        private readonly SinkingFundEnvelopeDAO _sinkingFundEnvelopeDAO;
        private readonly RolloverEnvelopeDAO _rolloverEnvelopeDAO;
        private readonly TransactionDAO _transactionDAO;
        string query = "";

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public PaymentTransactionDAO(MySqlConnection connection, AuthorizationService authService, AccountDAO accountDAO, SinkingFundEnvelopeDAO sinkingFundEnvelopeDAO, RolloverEnvelopeDAO rolloverEnvelopeDAO, TransactionDAO transactionDAO)
        {
            _connection = connection;
            _authService = authService;
            _accountDAO = accountDAO;
            _sinkingFundEnvelopeDAO = sinkingFundEnvelopeDAO;
            _rolloverEnvelopeDAO = rolloverEnvelopeDAO;
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
             * Query 4 (per split): Add transaction split (resolving rollover's month instance first if needed)
             * Query 5 (per split): Update envelope remaining amount
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
                    // Make sure this budget month belongs to the user
                    if (!await _authService.VerifyUserOwnsBudgetMonthAsync(paymentTransaction.BudgetMonthId, paymentTransaction.UserId, dbTransaction))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new CreatePaymentTransactionResponse(403, "Budget does not belong to the current user");
                    }
                    // Set up the query for adding the transaction
                    query = """
                        INSERT INTO transaction (budget_month_id, transaction_date, total_amount, is_reconciled, notes, transaction_type_id)
                        VALUES (@BudgetMonthId, @TransactionDate, @TotalAmount, false, @Notes, 2);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Execute the query
                    transactionId = await _connection.QuerySingleAsync<int>(query, paymentTransaction, dbTransaction);

                    // Make sure the account belongs to the user
                    if (!await _authService.VerifyUserOwnsAccountAsync(paymentTransaction.AccountId, paymentTransaction.UserId, dbTransaction))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new CreatePaymentTransactionResponse(403, "Account does not belong to the current user");
                    }
                    // Set up the query for adding the payment transaction
                    query = """
                        INSERT INTO payment_transaction (transaction_id, account_id, payment_transaction_type_id, merchant_source_name, check_number)
                        VALUES (@TransactionId, @AccountId, @PaymentTransactionType, @MerchantSourceName, @CheckNumber);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Execute the query
                    paymentTransactionId = await _connection.QuerySingleAsync<int>(query, new
                    {
                        TransactionId = transactionId,
                        paymentTransaction.AccountId,
                        paymentTransaction.PaymentTransactionType,
                        paymentTransaction.MerchantSourceName,
                        paymentTransaction.CheckNumber
                    }, dbTransaction);
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
                        int? sinkingFundEnvelopeId = null;
                        int? rolloverEnvelopeMonthId = null;

                        if (split.IsSinkingFund)
                        {
                            // Makes sure the split's envelope belongs to the user
                            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(split.EnvelopeId, paymentTransaction.UserId, dbTransaction))
                            {
                                // Roll the transaction back
                                dbTransaction.Rollback();
                                return new CreatePaymentTransactionResponse(403, "Envelope for split does not belong to the current user");
                            }
                            sinkingFundEnvelopeId = split.EnvelopeId;
                        }
                        else
                        {
                            // Makes sure the split's envelope belongs to the user
                            if (!await _authService.VerifyUserOwnsRolloverEnvelopeAsync(split.EnvelopeId, paymentTransaction.UserId, dbTransaction))
                            {
                                // Roll the transaction back
                                dbTransaction.Rollback();
                                return new CreatePaymentTransactionResponse(403, "Envelope for split does not belong to the current user");
                            }
                            // Resolve (or activate) this month's instance of the rollover envelope
                            rolloverEnvelopeMonthId = await _rolloverEnvelopeDAO.GetOrActivateForMonthAsync(split.EnvelopeId, paymentTransaction.BudgetMonthId, dbTransaction);
                            if (rolloverEnvelopeMonthId == null)
                            {
                                // Roll the transaction back
                                dbTransaction.Rollback();
                                return new CreatePaymentTransactionResponse(500, "There was an issue activating the rollover envelope for this month");
                            }
                        }
                        // Set up the query for adding a transaction split
                        query = """
                            INSERT INTO transaction_split (transaction_id, sinking_fund_envelope_id, rollover_envelope_month_id, amount)
                            VALUES (@TransactionId, @SinkingFundEnvelopeId, @RolloverEnvelopeMonthId, @Amount);
                            SELECT LAST_INSERT_ID();
                            """;
                        // Execute the query
                        transactionSplitId = await _connection.QuerySingleAsync<int>(query, new
                        {
                            split.TransactionId,
                            SinkingFundEnvelopeId = sinkingFundEnvelopeId,
                            RolloverEnvelopeMonthId = rolloverEnvelopeMonthId,
                            split.Amount
                        }, dbTransaction);
                        // Update the remaining amount of the envelope
                        bool remainingAmountUpdated = split.IsSinkingFund
                            ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(paymentTransaction.UserId, sinkingFundEnvelopeId, split.Amount, dbTransaction, _connection)
                            : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(paymentTransaction.UserId, rolloverEnvelopeMonthId, split.Amount, dbTransaction, _connection);
                        if (!remainingAmountUpdated)
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
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Set the status and the message
            response.HttpStatus = 201;
            response.Message = "Payment transaction created successfully";
            response.Id = transactionId;
            // Return the result
            return response;
        }

        /// <summary>
        /// DAO method to get a payment transaction and its splits
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetPaymentTransactionResponse> GetPaymentTransactionAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetPaymentTransactionResponse response = new GetPaymentTransactionResponse();
            GetPaymentTransactionDTOModel? responseDTO;

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
                    transaction.budget_month_id AS BudgetMonthId,
                    transaction.transaction_date AS TransactionDate,
                    transaction.total_amount AS TotalAmount,
                    transaction.is_reconciled AS IsReconciled,
                    transaction.notes AS Notes,
                    transaction.transaction_type_id AS TransactionType,
                    payment_transaction.payment_transaction_id AS PaymentTransactionId,
                    payment_transaction.account_id AS AccountId,
                    payment_transaction.payment_transaction_type_id AS PaymentTransactionType,
                    payment_transaction.merchant_source_name AS MerchantSourceName,
                    payment_transaction.check_number AS CheckNumber
                FROM transaction
                INNER JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                WHERE transaction.transaction_id = @TransactionId
                  AND transaction.deleted_at IS NULL
                """;
            // Get the transaction
            responseDTO = await _connection.QueryFirstAsync<GetPaymentTransactionDTOModel>(query, new { TransactionId = request.EntityId });

            // Make sure the payment transaction was found
            if (responseDTO == null)
            {
                // Return the issue
                return new GetPaymentTransactionResponse(404, "Payment transaction not found");
            }

            // Set up the query to get the list of transaction splits. EnvelopeId is resolved back
            // to whichever table it came from - sinking_fund_envelope_id directly, or the persistent
            // rollover_envelope_id via the month instance - with IsSinkingFund saying which.
            query = """
                SELECT 
                    transaction_split.transaction_split_id AS TransactionSplitId,
                    transaction_split.transaction_id AS TransactionId,
                    COALESCE(transaction_split.sinking_fund_envelope_id, rollover_envelope_month.rollover_envelope_id) AS EnvelopeId,
                    (transaction_split.sinking_fund_envelope_id IS NOT NULL) AS IsSinkingFund,
                    transaction_split.amount AS Amount
                FROM transaction_split
                LEFT JOIN rollover_envelope_month ON transaction_split.rollover_envelope_month_id = rollover_envelope_month.rollover_envelope_month_id
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

        /// <summary>
        /// DAO method to update a payment transaction and its splits.
        /// Reverses every old effect (the old account balance change, and each old split's effect
        /// on its envelope's remaining amount) before applying the new ones, so edited amounts and
        /// re-assigned envelopes/accounts don't leave balances double-counted or stale.
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <returns></returns>
        public async Task<UpdatePaymentTransactionResponse> UpdatePaymentTransactionAsync(UpdatePaymentTransactionRequest paymentTransaction)
        {
            // Declare and initialize
            UpdatePaymentTransactionResponse response = new UpdatePaymentTransactionResponse();
            List<string> updates = new List<string>();
            List<TransactionSplitSnapshotModel> existingSplits;
            List<int> incomingSplitIds;
            List<int> splitsToDelete;
            Dictionary<int, TransactionSplitSnapshotModel> existingSplitLookup;
            int rowsAffected;
            int transactionSplitId;
            BaseIdResponse transactionResponse;
            PaymentTransactionSnapshotModel? oldSnapshot;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Setting up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Snapshot the transaction's current account/amount/month before anything changes,
                // so the old effect on the account balance can be reversed below
                query = """
                    SELECT 
                        transaction.total_amount AS TotalAmount,
                        transaction.budget_month_id AS BudgetMonthId,
                        transaction.deleted_at IS NOT NULL AS IsDeleted,
                        payment_transaction.account_id AS AccountId
                    FROM transaction
                    INNER JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                    WHERE transaction.transaction_id = @TransactionId
                    """;
                oldSnapshot = await _connection.QuerySingleOrDefaultAsync<PaymentTransactionSnapshotModel>(query, new { paymentTransaction.TransactionId }, dbTransaction);
                if (oldSnapshot == null)
                {
                    dbTransaction.Rollback();
                    return new UpdatePaymentTransactionResponse(404, "Payment transaction not found");
                }

                // Make sure the transaction isn't soft-deleted. Its effect on envelopes and accounts
                // has been reversed, so editing it now would reverse and apply effects that aren't there.
                if (oldSnapshot.IsDeleted)
                {
                    dbTransaction.Rollback();
                    return new UpdatePaymentTransactionResponse(400, "Restore the transaction before editing it");
                }

                // Call the transaction update method
                transactionResponse = await _transactionDAO.UpdateTransactionAsync(paymentTransaction, dbTransaction);

                // Check the transaction response
                if (transactionResponse.HttpStatus < 200 || transactionResponse.HttpStatus >= 300)
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
                    // Skip the id and the splits
                    if (property.Name == "PaymentTransactionId" || property.Name == "TransactionSplits") continue;
                    object? propertyValue = property.GetValue(paymentTransaction);

                    // Check if the property is IOptional and has a value
                    if (propertyValue is IOptional optional && optional.HasValue)
                    {
                        updates.Add($"{property.Name.Underscore()} = @{property.Name}");
                    }
                    // Check if the property is null
                    else if (propertyValue != null)
                    {
                        updates.Add($"{property.Name.Underscore()} = @{property.Name}");
                    }
                }
                // If there are fields to update on payment_transaction, do so
                if (updates.Count != 0)
                {
                    // Assemble the query
                    query = $"""
                        UPDATE payment_transaction 
                        SET {string.Join(", ", updates)}
                        WHERE transaction_id = @TransactionId
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
                }

                // Reverse the old account balance effect, then apply the new one - covers the
                // amount changing, the account changing, or both
                int newAccountId = paymentTransaction.AccountId ?? oldSnapshot.AccountId;
                decimal newTotalAmount = paymentTransaction.TotalAmount ?? oldSnapshot.TotalAmount;
                if (!await _accountDAO.UpdateAccountBalanceAsync(paymentTransaction.UserId, oldSnapshot.AccountId, -oldSnapshot.TotalAmount, dbTransaction, _connection) ||
                    !await _accountDAO.UpdateAccountBalanceAsync(paymentTransaction.UserId, newAccountId, newTotalAmount, dbTransaction, _connection))
                {
                    dbTransaction.Rollback();
                    return new UpdatePaymentTransactionResponse(500, "There was an issue updating the account balance");
                }

                // Update the transaction splits table
                if (paymentTransaction.TransactionSplits != null)
                {
                    // Get a full snapshot of the existing splits (not just their ids), so each
                    // one's old amount can be reversed off its old envelope below
                    query = """
                        SELECT 
                            transaction_split_id AS TransactionSplitId,
                            sinking_fund_envelope_id AS SinkingFundEnvelopeId,
                            rollover_envelope_month_id AS RolloverEnvelopeMonthId,
                            amount AS Amount
                        FROM transaction_split 
                        WHERE transaction_id = @TransactionId
                        """;
                    try
                    {
                        existingSplits = (await _connection.QueryAsync<TransactionSplitSnapshotModel>(query, new { TransactionId = paymentTransaction.TransactionId }, dbTransaction)).ToList();
                    }
                    catch (Exception ex)
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        // Return the issue
                        return new UpdatePaymentTransactionResponse(500, $"{ex.Message}");
                    }
                    existingSplitLookup = existingSplits.ToDictionary(s => s.TransactionSplitId);

                    // Get the incoming split ids
                    incomingSplitIds = paymentTransaction.TransactionSplits.Where(s => s.TransactionSplitId != null).Select(s => s.TransactionSplitId!.Value).ToList();

                    // Get the splits to delete by comparing the existing and incoming splits
                    splitsToDelete = existingSplits.Select(s => s.TransactionSplitId).Except(incomingSplitIds).ToList();
                    // Check if there are splits to delete
                    if (splitsToDelete.Count > 0)
                    {
                        // Reverse each deleted split's old effect before deleting it
                        foreach (int deletedSplitId in splitsToDelete)
                        {
                            TransactionSplitSnapshotModel deleted = existingSplitLookup[deletedSplitId];
                            bool reversed = deleted.SinkingFundEnvelopeId.HasValue
                                ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(paymentTransaction.UserId, deleted.SinkingFundEnvelopeId, -deleted.Amount, dbTransaction, _connection)
                                : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(paymentTransaction.UserId, deleted.RolloverEnvelopeMonthId, -deleted.Amount, dbTransaction, _connection);
                            if (!reversed)
                            {
                                dbTransaction.Rollback();
                                return new UpdatePaymentTransactionResponse(500, "There was an issue reversing a removed split's envelope amount");
                            }
                        }
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
                        int? sinkingFundEnvelopeId = null;
                        int? rolloverEnvelopeMonthId = null;

                        if (split.IsSinkingFund == true)
                        {
                            // Makes sure the split's envelope belongs to the user
                            if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(split.EnvelopeId, paymentTransaction.UserId, dbTransaction))
                            {
                                dbTransaction.Rollback();
                                return new UpdatePaymentTransactionResponse(403, "Envelope for split does not belong to the current user");
                            }
                            sinkingFundEnvelopeId = split.EnvelopeId;
                        }
                        else if (split.IsSinkingFund == false)
                        {
                            // Makes sure the split's envelope belongs to the user
                            if (!await _authService.VerifyUserOwnsRolloverEnvelopeAsync(split.EnvelopeId, paymentTransaction.UserId, dbTransaction))
                            {
                                dbTransaction.Rollback();
                                return new UpdatePaymentTransactionResponse(403, "Envelope for split does not belong to the current user");
                            }
                            rolloverEnvelopeMonthId = await _rolloverEnvelopeDAO.GetOrActivateForMonthAsync(split.EnvelopeId!.Value, oldSnapshot.BudgetMonthId, dbTransaction);
                            if (rolloverEnvelopeMonthId == null)
                            {
                                dbTransaction.Rollback();
                                return new UpdatePaymentTransactionResponse(500, "There was an issue activating the rollover envelope for this month");
                            }
                        }
                        // Check if the transaction split id is null
                        if (split.TransactionSplitId.HasValue)
                        {
                            // This split already existed - reverse its old effect before applying the new one
                            TransactionSplitSnapshotModel oldSplit = existingSplitLookup[split.TransactionSplitId.Value];
                            bool oldReversed = oldSplit.SinkingFundEnvelopeId.HasValue
                                ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(paymentTransaction.UserId, oldSplit.SinkingFundEnvelopeId, -oldSplit.Amount, dbTransaction, _connection)
                                : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(paymentTransaction.UserId, oldSplit.RolloverEnvelopeMonthId, -oldSplit.Amount, dbTransaction, _connection);
                            if (!oldReversed)
                            {
                                dbTransaction.Rollback();
                                return new UpdatePaymentTransactionResponse(500, "There was an issue reversing a split's old envelope amount");
                            }
                            // If the split didn't say which envelope type it's changing to, keep the existing one
                            if (split.IsSinkingFund == null)
                            {
                                sinkingFundEnvelopeId = oldSplit.SinkingFundEnvelopeId;
                                rolloverEnvelopeMonthId = oldSplit.RolloverEnvelopeMonthId;
                            }
                            decimal newAmount = split.Amount ?? oldSplit.Amount;

                            // Update the existing split
                            query = """
                                UPDATE transaction_split 
                                SET sinking_fund_envelope_id = @SinkingFundEnvelopeId,
                                    rollover_envelope_month_id = @RolloverEnvelopeMonthId,
                                    amount = @Amount
                                WHERE transaction_split_id = @TransactionSplitId
                                """;
                            try
                            {
                                await _connection.ExecuteAsync(query, new
                                {
                                    SinkingFundEnvelopeId = sinkingFundEnvelopeId,
                                    RolloverEnvelopeMonthId = rolloverEnvelopeMonthId,
                                    Amount = newAmount,
                                    split.TransactionSplitId
                                }, dbTransaction);
                            }
                            catch (Exception ex)
                            {
                                // Roll the transaction back
                                dbTransaction.Rollback();
                                // Return the issue
                                return new UpdatePaymentTransactionResponse(500, $"{ex.Message}");
                            }
                            // Apply the new effect
                            bool newApplied = sinkingFundEnvelopeId.HasValue
                                ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(paymentTransaction.UserId, sinkingFundEnvelopeId, newAmount, dbTransaction, _connection)
                                : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(paymentTransaction.UserId, rolloverEnvelopeMonthId, newAmount, dbTransaction, _connection);
                            if (!newApplied)
                            {
                                dbTransaction.Rollback();
                                return new UpdatePaymentTransactionResponse(500, "There was an issue applying a split's new envelope amount");
                            }
                        }
                        else
                        {
                            // Brand new split - nothing to reverse, just apply its effect
                            decimal newAmount = split.Amount ?? 0m;
                            // Set the transaction id
                            split.TransactionId = paymentTransaction.TransactionId;
                            // Add a new split
                            query = """
                                INSERT INTO transaction_split (transaction_id, sinking_fund_envelope_id, rollover_envelope_month_id, amount)
                                VALUES (@TransactionId, @SinkingFundEnvelopeId, @RolloverEnvelopeMonthId, @Amount);
                                SELECT LAST_INSERT_ID();
                            """;
                            // Execute the query
                            transactionSplitId = await _connection.QuerySingleAsync<int>(query, new
                            {
                                split.TransactionId,
                                SinkingFundEnvelopeId = sinkingFundEnvelopeId,
                                RolloverEnvelopeMonthId = rolloverEnvelopeMonthId,
                                Amount = newAmount
                            }, dbTransaction);
                            bool newApplied = sinkingFundEnvelopeId.HasValue
                                ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(paymentTransaction.UserId, sinkingFundEnvelopeId, newAmount, dbTransaction, _connection)
                                : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(paymentTransaction.UserId, rolloverEnvelopeMonthId, newAmount, dbTransaction, _connection);
                            if (!newApplied)
                            {
                                dbTransaction.Rollback();
                                return new UpdatePaymentTransactionResponse(500, "There was an issue applying a new split's envelope amount");
                            }
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