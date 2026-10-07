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
    public class TransactionDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        private readonly AccountDAO _accountDAO;
        private readonly SinkingFundEnvelopeDAO _sinkingFundEnvelopeDAO;
        private readonly RolloverEnvelopeDAO _rolloverEnvelopeDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables. The account and envelope DAOs are
        /// needed so delete/restore can reverse or re-apply a transaction's balance effects.
        /// </summary>
        public TransactionDAO(MySqlConnection connection, AuthorizationService authService, AccountDAO accountDAO, SinkingFundEnvelopeDAO sinkingFundEnvelopeDAO, RolloverEnvelopeDAO rolloverEnvelopeDAO)
        {
            _connection = connection;
            _authService = authService;
            _accountDAO = accountDAO;
            _sinkingFundEnvelopeDAO = sinkingFundEnvelopeDAO;
            _rolloverEnvelopeDAO = rolloverEnvelopeDAO;
        }

        /// <summary>
        /// DAO method to get every transaction (payment, account transfer, and envelope transfer)
        /// for a single budget month.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetAllTransactionsEntityResponse> GetAllTransactionsAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllTransactionsEntityResponse response = new GetAllTransactionsEntityResponse();

            // Make sure this budget month belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetMonthAsync(request.EntityId, request.UserId))
            {
                return new GetAllTransactionsEntityResponse(403, "Budget does not belong to the current user");
            }

            // Set up the query to read the payment transactions.
            // Self-join transaction_split, then LEFT JOIN both envelope tables off of it, so
            // GROUP_CONCAT picks up every envelope name touched by the transaction regardless of
            // which table each split's envelope came from.
            query = """
                SELECT
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionType,
                    transaction.total_amount AS TotalAmount,
                    payment_transaction.merchant_source_name AS MerchantSourceName,
                    GROUP_CONCAT(DISTINCT COALESCE(sfe.envelope_name, re.envelope_name) SEPARATOR ', ') AS EnvelopeNames
                FROM transaction
                INNER JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                LEFT JOIN transaction_split ON transaction.transaction_id = transaction_split.transaction_id
                LEFT JOIN sinking_fund_envelope AS sfe ON transaction_split.sinking_fund_envelope_id = sfe.sinking_fund_envelope_id
                LEFT JOIN rollover_envelope_month AS rem ON transaction_split.rollover_envelope_month_id = rem.rollover_envelope_month_id
                LEFT JOIN rollover_envelope AS re ON rem.rollover_envelope_id = re.rollover_envelope_id
                WHERE transaction.budget_month_id = @BudgetMonthId
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
            response.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { BudgetMonthId = request.EntityId })).ToList();

            // Set up the query to read the account transfer transactions
            query = """
                SELECT
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionType,
                    transaction.total_amount AS TotalAmount,
                    transfer_transaction.transfer_transaction_type_id AS TransferTransactionType,
                    from_account.account_name AS TransferFromAccount,
                    to_account.account_name AS TransferToAccount
                FROM transaction
                INNER JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                LEFT JOIN account AS from_account ON transfer_transaction.transfer_from_account_id = from_account.account_id
                LEFT JOIN account AS to_account ON transfer_transaction.transfer_to_account_id = to_account.account_id
                WHERE transaction.budget_month_id = @BudgetMonthId
                  AND transaction.deleted_at IS NULL
                  AND transaction.transaction_type_id = 3
                  AND transfer_transaction.transfer_transaction_type_id = 2
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the account transfer transactions
            response.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { BudgetMonthId = request.EntityId })).ToList());

            // Set up the query to read the envelope transfer transactions. These are identified by
            // pairing the negative-amount split with the positive-amount split on the same
            // transaction, rather than by columns on transfer_transaction (which no longer has any
            // envelope columns - just a type-3 marker row with null account columns).
            query = """
                SELECT
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionType,
                    transfer_transaction.transfer_transaction_type_id AS TransferTransactionType,
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
                WHERE transaction.budget_month_id = @BudgetMonthId
                  AND transaction.deleted_at IS NULL
                  AND transaction.transaction_type_id = 3
                  AND transfer_transaction.transfer_transaction_type_id = 3
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the envelope transfer transactions
            response.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { BudgetMonthId = request.EntityId })).ToList());

            // Set the status and message for the transactions response
            response.HttpStatus = 200;
            response.Message = "Transactions found";
            // Return the response
            return response;
        }

        /// <summary>
        /// Shared method (called by PaymentTransactionDAO/TransferTransactionDAO) to update the
        /// base transaction fields common to every transaction type. Unaffected by the schema split -
        /// transaction_date, total_amount, and notes are all still plain columns on transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateTransactionAsync(UpdateTransactionRequest transaction, MySqlTransaction dbTransaction)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the transaction belongs to the user
            if (!await _authService.VerifyUserOwnsTransactionAsync(transaction.TransactionId, transaction.UserId, dbTransaction))
            {
                // Return the issue
                return new BaseIdResponse(403, "Transaction does not belong to the current user", transaction.TransactionId);
            }

            // Loop through each field to see if an update is necessary
            foreach (PropertyInfo property in typeof(UpdateTransactionRequest).GetProperties())
            {
                // Skip the ids
                if (property.Name == "TransactionId" || property.Name == "UserId") continue;
                object? propertyValue = property.GetValue(transaction);

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
                return new BaseIdResponse(400, "No fields to update", transaction.TransactionId);
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
                return new BaseIdResponse(404, "Transaction not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
            }
        }

        /// <summary>
        /// Reads what a transaction currently does to account balances and whether it is
        /// soft-deleted. The LEFT JOINs mean only the columns for the transaction's own subtype
        /// come back non-null (payment account, or transfer from/to accounts).
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="dbTransaction"></param>
        /// <returns>The snapshot, or null if the transaction does not exist</returns>
        private async Task<TransactionEffectSnapshotModel?> GetEffectSnapshotAsync(int transactionId, MySqlTransaction dbTransaction)
        {
            // Set up the query. "deleted_at IS NOT NULL" gives a simple true/false for IsDeleted
            query = """
                SELECT 
                    transaction.total_amount AS TotalAmount,
                    transaction.deleted_at IS NOT NULL AS IsDeleted,
                    payment_transaction.account_id AS PaymentAccountId,
                    transfer_transaction.transfer_from_account_id AS TransferFromAccountId,
                    transfer_transaction.transfer_to_account_id AS TransferToAccountId
                FROM transaction
                LEFT JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                LEFT JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                WHERE transaction.transaction_id = @TransactionId
                """;

            // Execute the query inside the caller's database transaction and return the result
            return await _connection.QuerySingleOrDefaultAsync<TransactionEffectSnapshotModel>(query, new { TransactionId = transactionId }, dbTransaction);
        }

        /// <summary>
        /// Applies (direction = 1) or reverses (direction = -1) a transaction's effect on envelope
        /// remaining amounts and account balances. Runs inside the caller's database transaction,
        /// so the caller is responsible for committing or rolling back.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionId"></param>
        /// <param name="direction">1 to apply the effect, -1 to reverse it</param>
        /// <param name="snapshot">The transaction's current account info (from GetEffectSnapshotAsync)</param>
        /// <param name="dbTransaction"></param>
        /// <returns>null on success, otherwise an error message</returns>
        private async Task<string?> ApplyTransactionEffectAsync(int userId, int transactionId, int direction, TransactionEffectSnapshotModel snapshot, MySqlTransaction dbTransaction)
        {
            // Get every split on the transaction. Payments and envelope transfers both store their
            // envelope movement as splits, so one query covers both.
            query = """
                SELECT 
                    transaction_split_id AS TransactionSplitId,
                    sinking_fund_envelope_id AS SinkingFundEnvelopeId,
                    rollover_envelope_month_id AS RolloverEnvelopeMonthId,
                    amount AS Amount
                FROM transaction_split
                WHERE transaction_id = @TransactionId
                """;
            List<TransactionSplitSnapshotModel> splits = (await _connection.QueryAsync<TransactionSplitSnapshotModel>(query, new { TransactionId = transactionId }, dbTransaction)).ToList();

            // Envelope effect: each split moved its envelope by its (signed) amount when the
            // transaction was created, so multiplying by direction applies it or undoes it
            foreach (TransactionSplitSnapshotModel split in splits)
            {
                // Each split points at exactly one of a sinking fund or a rollover envelope month
                bool updated = split.SinkingFundEnvelopeId.HasValue
                    ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(userId, split.SinkingFundEnvelopeId, split.Amount * direction, dbTransaction, _connection)
                    : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(userId, split.RolloverEnvelopeMonthId, split.Amount * direction, dbTransaction, _connection);

                // Stop and report if an envelope update failed
                if (!updated)
                {
                    return "There was an issue updating an envelope's remaining amount";
                }
            }

            // Account effect: a payment moves one account by the total amount
            if (snapshot.PaymentAccountId.HasValue)
            {
                if (!await _accountDAO.UpdateAccountBalanceAsync(userId, snapshot.PaymentAccountId, snapshot.TotalAmount * direction, dbTransaction, _connection))
                {
                    return "There was an issue updating the account balance";
                }
            }
            // An account transfer takes the amount out of "from" and puts it into "to"
            else if (snapshot.TransferFromAccountId.HasValue && snapshot.TransferToAccountId.HasValue)
            {
                if (!await _accountDAO.UpdateAccountBalanceAsync(userId, snapshot.TransferFromAccountId, -snapshot.TotalAmount * direction, dbTransaction, _connection) ||
                    !await _accountDAO.UpdateAccountBalanceAsync(userId, snapshot.TransferToAccountId, snapshot.TotalAmount * direction, dbTransaction, _connection))
                {
                    return "There was an issue updating the account balances";
                }
            }
            // Envelope transfers have no account columns, so there is no account effect to handle

            // Return that everything succeeded
            return null;
        }

        /// <summary>
        /// DAO method to soft delete a transaction. The row is kept (so it can be restored), but its
        /// effect on envelopes and account balances is reversed so deleted transactions don't count.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> SoftDeleteTransactionAsync(BaseIdRequest request)
        {
            // Make sure the transaction belongs to the user
            if (!await _authService.VerifyUserOwnsTransactionAsync(request.EntityId, request.UserId))
            {
                return new BaseResponse(403, "Transaction does not belong to the current user");
            }

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();

            // Setting up the transaction so the reversal and the delete flag succeed or fail together
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Read the transaction's current account info and deleted state
                TransactionEffectSnapshotModel? snapshot = await GetEffectSnapshotAsync(request.EntityId, dbTransaction);

                // Make sure the transaction exists
                if (snapshot == null)
                {
                    dbTransaction.Rollback();
                    return new BaseResponse(404, "Transaction not found");
                }

                // Make sure it isn't already deleted - reversing a second time would double count
                if (snapshot.IsDeleted)
                {
                    dbTransaction.Rollback();
                    return new BaseResponse(400, "Transaction is already deleted");
                }

                // Reverse the transaction's effect on envelopes and accounts
                string? error = await ApplyTransactionEffectAsync(request.UserId, request.EntityId, -1, snapshot, dbTransaction);
                if (error != null)
                {
                    // Roll everything back and report the issue
                    dbTransaction.Rollback();
                    return new BaseResponse(500, error);
                }

                // Mark the transaction as deleted
                query = "UPDATE transaction SET deleted_at = NOW() WHERE transaction_id = @TransactionId";
                await _connection.ExecuteAsync(query, new { TransactionId = request.EntityId }, dbTransaction);

                // Commit the transaction
                dbTransaction.Commit();
            }

            // Return the success
            return new BaseResponse(200, "Transaction deleted successfully");
        }

        /// <summary>
        /// DAO method to restore a soft-deleted transaction. Re-applies the effect on envelopes and
        /// account balances that the soft delete removed.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> RestoreTransactionAsync(BaseIdRequest request)
        {
            // Make sure the transaction belongs to the user
            if (!await _authService.VerifyUserOwnsTransactionAsync(request.EntityId, request.UserId))
            {
                return new BaseResponse(403, "Transaction does not belong to the current user");
            }

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();

            // Setting up the transaction so the re-apply and the flag clear succeed or fail together
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Read the transaction's current account info and deleted state
                TransactionEffectSnapshotModel? snapshot = await GetEffectSnapshotAsync(request.EntityId, dbTransaction);

                // Make sure the transaction exists
                if (snapshot == null)
                {
                    dbTransaction.Rollback();
                    return new BaseResponse(404, "Transaction not found");
                }

                // Make sure it is actually deleted - applying again would double count
                if (!snapshot.IsDeleted)
                {
                    dbTransaction.Rollback();
                    return new BaseResponse(400, "Transaction is not deleted");
                }

                // Re-apply the transaction's effect on envelopes and accounts
                string? error = await ApplyTransactionEffectAsync(request.UserId, request.EntityId, 1, snapshot, dbTransaction);
                if (error != null)
                {
                    // Roll everything back and report the issue
                    dbTransaction.Rollback();
                    return new BaseResponse(500, error);
                }

                // Clear the deleted flag
                query = "UPDATE transaction SET deleted_at = NULL WHERE transaction_id = @TransactionId";
                await _connection.ExecuteAsync(query, new { TransactionId = request.EntityId }, dbTransaction);

                // Commit the transaction
                dbTransaction.Commit();
            }

            // Return the success
            return new BaseResponse(200, "Transaction restored successfully");
        }

        /// <summary>
        /// DAO method to permanently delete a transaction. If the transaction is still live, its
        /// effect is reversed first; if it was already soft-deleted, the effect was already reversed
        /// so only the row is removed.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> HardDeleteTransactionAsync(BaseIdRequest request)
        {
            // Make sure the transaction belongs to the user
            if (!await _authService.VerifyUserOwnsTransactionAsync(request.EntityId, request.UserId))
            {
                return new BaseResponse(403, "Transaction does not belong to the current user");
            }

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();

            // Setting up the transaction so the reversal and the delete succeed or fail together
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Read the transaction's current account info and deleted state
                TransactionEffectSnapshotModel? snapshot = await GetEffectSnapshotAsync(request.EntityId, dbTransaction);

                // Make sure the transaction exists
                if (snapshot == null)
                {
                    dbTransaction.Rollback();
                    return new BaseResponse(404, "Transaction not found");
                }

                // Only reverse the effect if the transaction is still live. A soft-deleted
                // transaction was already reversed, so reversing again would double count.
                if (!snapshot.IsDeleted)
                {
                    string? error = await ApplyTransactionEffectAsync(request.UserId, request.EntityId, -1, snapshot, dbTransaction);
                    if (error != null)
                    {
                        // Roll everything back and report the issue
                        dbTransaction.Rollback();
                        return new BaseResponse(500, error);
                    }
                }

                // Delete the row. The splits and payment/transfer rows are removed by ON DELETE CASCADE.
                query = "DELETE FROM transaction WHERE transaction_id = @TransactionId";
                await _connection.ExecuteAsync(query, new { TransactionId = request.EntityId }, dbTransaction);

                // Commit the transaction
                dbTransaction.Commit();
            }

            // Return the success
            return new BaseResponse(200, "Transaction deleted permanently");
        }
    }
}