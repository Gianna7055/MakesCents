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
        private readonly SinkingFundEnvelopeDAO _sinkingFundEnvelopeDAO;
        private readonly RolloverEnvelopeDAO _rolloverEnvelopeDAO;
        private readonly TransactionDAO _transactionDAO;
        string query = "";

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public TransferTransactionDAO(MySqlConnection connection, AuthorizationService authService, AccountDAO accountDAO, SinkingFundEnvelopeDAO sinkingFundEnvelopeDAO, RolloverEnvelopeDAO rolloverEnvelopeDAO, TransactionDAO transactionDAO)
        {
            _connection = connection;
            _authService = authService;
            _accountDAO = accountDAO;
            _sinkingFundEnvelopeDAO = sinkingFundEnvelopeDAO;
            _rolloverEnvelopeDAO = rolloverEnvelopeDAO;
            _transactionDAO = transactionDAO;
        }

        /// <summary>
        /// DAO method to create a new transfer transaction (account-to-account or envelope-to-envelope)
        /// </summary>
        /// <param name="transferTransaction"></param>
        /// <returns></returns>
        public async Task<CreateTransferTransactionResponse> CreateTransferTransactionAsync(CreateTransferTransactionRequest transferTransaction)
        {
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
                    // Make sure this budget month belongs to the user
                    if (!await _authService.VerifyUserOwnsBudgetMonthAsync(transferTransaction.BudgetMonthId, transferTransaction.UserId, dbTransaction))
                    {
                        dbTransaction.Rollback();
                        return new CreateTransferTransactionResponse(403, "Budget does not belong to the current user");
                    }
                    // Create the underlying transaction
                    query = """
                        INSERT INTO transaction (budget_month_id, transaction_date, total_amount, is_reconciled, notes, transaction_type_id)
                        VALUES (@BudgetMonthId, @TransactionDate, @TotalAmount, false, @Notes, 3);
                        SELECT LAST_INSERT_ID();
                        """;
                    transactionId = await _connection.QuerySingleAsync<int>(query, transferTransaction, dbTransaction);

                    if (transferTransaction.TransferTransactionType == TransferTransactionType.Account)
                    {
                        // Makes sure both accounts belong to the user
                        if (!await _authService.VerifyUserOwnsAccountAsync(transferTransaction.TransferFromId, transferTransaction.UserId, dbTransaction) ||
                            !await _authService.VerifyUserOwnsAccountAsync(transferTransaction.TransferToId, transferTransaction.UserId, dbTransaction))
                        {
                            dbTransaction.Rollback();
                            return new CreateTransferTransactionResponse(403, "One or both accounts do not belong to the current user");
                        }
                        // Set up the query for the transfer transaction
                        query = """
                            INSERT INTO transfer_transaction (transaction_id, transfer_from_account_id, transfer_to_account_id, transfer_transaction_type_id)
                            VALUES (@TransactionId, @TransferFromId, @TransferToId, 2);
                            SELECT LAST_INSERT_ID();
                            """;
                        transferTransactionId = await _connection.QuerySingleAsync<int>(query, new
                        {
                            TransactionId = transactionId,
                            transferTransaction.TransferFromId,
                            transferTransaction.TransferToId
                        }, dbTransaction);
                        response.TransferTransactionId = transferTransactionId;

                        // Update the to and from accounts
                        if (!await _accountDAO.UpdateAccountBalanceAsync(transferTransaction.UserId, transferTransaction.TransferFromId, -transferTransaction.TotalAmount, dbTransaction, _connection) ||
                            !await _accountDAO.UpdateAccountBalanceAsync(transferTransaction.UserId, transferTransaction.TransferToId, transferTransaction.TotalAmount, dbTransaction, _connection))
                        {
                            dbTransaction.Rollback();
                            return new CreateTransferTransactionResponse(400, "There was an issue moving the amount between accounts");
                        }
                    }
                    else if (transferTransaction.TransferTransactionType == TransferTransactionType.Envelope)
                    {
                        // Resolve each side down to a sinking_fund_envelope_id or a
                        // rollover_envelope_month_id (activating the month instance if needed)
                        (int? fromSinkingFundEnvelopeId, int? fromRolloverEnvelopeMonthId, CreateTransferTransactionResponse? fromError) =
                            await ResolveEnvelopeSideAsync(transferTransaction.TransferFromId, transferTransaction.TransferFromIsSinkingFund, transferTransaction.BudgetMonthId, transferTransaction.UserId, dbTransaction);
                        if (fromError != null)
                        {
                            dbTransaction.Rollback();
                            return fromError;
                        }
                        (int? toSinkingFundEnvelopeId, int? toRolloverEnvelopeMonthId, CreateTransferTransactionResponse? toError) =
                            await ResolveEnvelopeSideAsync(transferTransaction.TransferToId, transferTransaction.TransferToIsSinkingFund, transferTransaction.BudgetMonthId, transferTransaction.UserId, dbTransaction);
                        if (toError != null)
                        {
                            dbTransaction.Rollback();
                            return toError;
                        }

                        // Create the transfer transaction marker row - no account columns for an
                        // envelope transfer, the envelope references live on transaction_split instead
                        query = """
                            INSERT INTO transfer_transaction (transaction_id, transfer_transaction_type_id)
                            VALUES (@TransactionId, 3);
                            SELECT LAST_INSERT_ID();
                            """;
                        transferTransactionId = await _connection.QuerySingleAsync<int>(query, new { TransactionId = transactionId }, dbTransaction);
                        response.TransferTransactionId = transferTransactionId;

                        // Create the "from" split (negative) and "to" split (positive)
                        query = """
                            INSERT INTO transaction_split (transaction_id, sinking_fund_envelope_id, rollover_envelope_month_id, amount)
                            VALUES (@TransactionId, @SinkingFundEnvelopeId, @RolloverEnvelopeMonthId, @Amount);
                            """;
                        await _connection.ExecuteAsync(query, new
                        {
                            TransactionId = transactionId,
                            SinkingFundEnvelopeId = fromSinkingFundEnvelopeId,
                            RolloverEnvelopeMonthId = fromRolloverEnvelopeMonthId,
                            Amount = -transferTransaction.TotalAmount
                        }, dbTransaction);
                        await _connection.ExecuteAsync(query, new
                        {
                            TransactionId = transactionId,
                            SinkingFundEnvelopeId = toSinkingFundEnvelopeId,
                            RolloverEnvelopeMonthId = toRolloverEnvelopeMonthId,
                            Amount = transferTransaction.TotalAmount
                        }, dbTransaction);

                        // Update the remaining amounts on both sides
                        bool fromUpdated = fromSinkingFundEnvelopeId.HasValue
                            ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(transferTransaction.UserId, fromSinkingFundEnvelopeId, -transferTransaction.TotalAmount, dbTransaction, _connection)
                            : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(transferTransaction.UserId, fromRolloverEnvelopeMonthId, -transferTransaction.TotalAmount, dbTransaction, _connection);
                        bool toUpdated = toSinkingFundEnvelopeId.HasValue
                            ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(transferTransaction.UserId, toSinkingFundEnvelopeId, transferTransaction.TotalAmount, dbTransaction, _connection)
                            : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(transferTransaction.UserId, toRolloverEnvelopeMonthId, transferTransaction.TotalAmount, dbTransaction, _connection);
                        if (!fromUpdated || !toUpdated)
                        {
                            dbTransaction.Rollback();
                            return new CreateTransferTransactionResponse(400, "There was an issue moving the amount between envelopes");
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    return new CreateTransferTransactionResponse(500, $"{ex.Message}");
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            response.HttpStatus = 201;
            response.Message = "Transfer transaction created successfully";
            response.Id = transactionId;
            return response;
        }

        /// <summary>
        /// Shared helper: verifies ownership of one side of an envelope transfer and resolves it
        /// down to whichever FK column (sinking_fund_envelope_id or rollover_envelope_month_id)
        /// transaction_split needs. Returns a populated error response instead of throwing so the
        /// caller can roll back and return it directly.
        /// </summary>
        private async Task<(int? sinkingFundEnvelopeId, int? rolloverEnvelopeMonthId, CreateTransferTransactionResponse? error)> ResolveEnvelopeSideAsync(int envelopeId, bool? isSinkingFund, int budgetMonthId, int userId, MySqlTransaction dbTransaction)
        {
            if (isSinkingFund == true)
            {
                if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(envelopeId, userId, dbTransaction))
                {
                    return (null, null, new CreateTransferTransactionResponse(403, "One or both envelopes do not belong to the current user"));
                }
                return (envelopeId, null, null);
            }
            else
            {
                if (!await _authService.VerifyUserOwnsRolloverEnvelopeAsync(envelopeId, userId, dbTransaction))
                {
                    return (null, null, new CreateTransferTransactionResponse(403, "One or both envelopes do not belong to the current user"));
                }
                int? rolloverEnvelopeMonthId = await _rolloverEnvelopeDAO.GetOrActivateForMonthAsync(envelopeId, budgetMonthId, dbTransaction);
                if (rolloverEnvelopeMonthId == null)
                {
                    return (null, null, new CreateTransferTransactionResponse(500, "There was an issue activating a rollover envelope for this month"));
                }
                return (null, rolloverEnvelopeMonthId, null);
            }
        }

        /// <summary>
        /// DAO method to get a transfer transaction. For an envelope transfer, the two envelope
        /// sides are resolved from a follow-up transaction_split lookup rather than columns on
        /// transfer_transaction, since that table no longer has envelope columns.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
                    transaction.budget_month_id AS BudgetMonthId,
                    transaction.transaction_date AS TransactionDate,
                    transaction.total_amount AS TotalAmount,
                    transaction.is_reconciled AS IsReconciled,
                    transaction.notes AS Notes,
                    transaction.transaction_type_id AS TransactionType,
                    transfer_transaction.transfer_transaction_id AS TransferTransactionId,
                    transfer_transaction.transfer_from_account_id AS TransferFromAccountId,
                    transfer_transaction.transfer_to_account_id AS TransferToAccountId,
                    transfer_transaction.transfer_transaction_type_id AS TransferTransactionType
                FROM transaction
                INNER JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                WHERE transaction.transaction_id = @TransactionId
                  AND transaction.deleted_at IS NULL
                """;
            // Execute the query to get the entity model
            response.TransferTransaction = await _connection.QuerySingleAsync<GetTransferTransactionEntityModel>(query, new { TransactionId = request.EntityId });

            // Make sure the transfer transaction was found
            if (response.TransferTransaction == null)
            {
                // Return the issue
                return new GetTransferTransactionEntityResponse(404, "Transfer transaction not found");
            }

            // If this is an envelope transfer, resolve the two sides from transaction_split
            if (response.TransferTransaction.TransferTransactionType == TransferTransactionType.Envelope)
            {
                query = """
                    SELECT 
                        transaction_split.amount AS Amount,
                        COALESCE(transaction_split.sinking_fund_envelope_id, rollover_envelope_month.rollover_envelope_id) AS EnvelopeId,
                        (transaction_split.sinking_fund_envelope_id IS NOT NULL) AS IsSinkingFund
                    FROM transaction_split
                    LEFT JOIN rollover_envelope_month ON transaction_split.rollover_envelope_month_id = rollover_envelope_month.rollover_envelope_month_id
                    WHERE transaction_split.transaction_id = @TransactionId
                    """;
                List<TransactionSplitTransferSideModel> sides = (await _connection.QueryAsync<TransactionSplitTransferSideModel>(query, new { TransactionId = request.EntityId })).ToList();
                TransactionSplitTransferSideModel? fromSide = sides.FirstOrDefault(s => s.Amount < 0);
                TransactionSplitTransferSideModel? toSide = sides.FirstOrDefault(s => s.Amount > 0);
                if (fromSide != null)
                {
                    response.TransferTransaction.TransferFromEnvelopeId = fromSide.EnvelopeId;
                    response.TransferTransaction.TransferFromIsSinkingFund = fromSide.IsSinkingFund;
                }
                if (toSide != null)
                {
                    response.TransferTransaction.TransferToEnvelopeId = toSide.EnvelopeId;
                    response.TransferTransaction.TransferToIsSinkingFund = toSide.IsSinkingFund;
                }
            }

            // Set the status and message for the transfer transaction response
            response.HttpStatus = 200;
            response.Message = "Transfer transaction found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to update a transfer transaction. Reverses whatever the OLD effect was
        /// (old account balances if it was an account transfer, or old envelope remaining amounts
        /// if it was an envelope transfer) before applying the NEW effect, so edited amounts,
        /// re-assigned accounts/envelopes, or even switching between transfer types don't leave
        /// balances double-counted or stale.
        /// </summary>
        /// <param name="transferTransaction"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateTransferTransactionAsync(UpdateTransferTransactionRequest transferTransaction)
        {
            // Declare and initialize
            BaseIdResponse transactionResponse;
            int rowsAffected;
            int? oldFromSinkingFundEnvelopeId = null, oldFromRolloverEnvelopeMonthId = null;
            int? oldToSinkingFundEnvelopeId = null, oldToRolloverEnvelopeMonthId = null;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync();
            // Setting up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Snapshot the transfer's current state before anything changes
                query = """
                    SELECT 
                        transaction.total_amount AS TotalAmount,
                        transaction.budget_month_id AS BudgetMonthId,
                        transaction.deleted_at IS NOT NULL AS IsDeleted,
                        transfer_transaction.transfer_transaction_type_id AS TransferTransactionType,
                        transfer_transaction.transfer_from_account_id AS TransferFromAccountId,
                        transfer_transaction.transfer_to_account_id AS TransferToAccountId
                    FROM transaction
                    INNER JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                    WHERE transfer_transaction.transfer_transaction_id = @TransferTransactionId
                    """;
                TransferTransactionSnapshotModel? oldSnapshot = await _connection.QuerySingleOrDefaultAsync<TransferTransactionSnapshotModel>(query, new { transferTransaction.TransferTransactionId }, dbTransaction);
                if (oldSnapshot == null)
                {
                    dbTransaction.Rollback();
                    return new BaseIdResponse(404, "Transfer transaction not found");
                }

                // Make sure the transaction isn't soft-deleted (its effects are currently reversed,
                // so editing it would reverse and apply effects that aren't there)
                if (oldSnapshot.IsDeleted)
                {
                    dbTransaction.Rollback();
                    return new BaseIdResponse(400, "Restore the transaction before editing it");
                }

                // Reverse the OLD effect, based on the OLD type
                if (oldSnapshot.TransferTransactionType == TransferTransactionType.Account)
                {
                    if (oldSnapshot.TransferFromAccountId.HasValue &&
                        !await _accountDAO.UpdateAccountBalanceAsync(transferTransaction.UserId, oldSnapshot.TransferFromAccountId, oldSnapshot.TotalAmount, dbTransaction, _connection))
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(500, "There was an issue reversing the old transfer-from account balance");
                    }
                    if (oldSnapshot.TransferToAccountId.HasValue &&
                        !await _accountDAO.UpdateAccountBalanceAsync(transferTransaction.UserId, oldSnapshot.TransferToAccountId, -oldSnapshot.TotalAmount, dbTransaction, _connection))
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(500, "There was an issue reversing the old transfer-to account balance");
                    }
                }
                else // Old type was Envelope
                {
                    query = """
                        SELECT sinking_fund_envelope_id AS SinkingFundEnvelopeId, rollover_envelope_month_id AS RolloverEnvelopeMonthId, amount AS Amount
                        FROM transaction_split
                        WHERE transaction_id = @TransactionId
                        """;
                    List<TransactionSplitSnapshotModel> oldSplits = (await _connection.QueryAsync<TransactionSplitSnapshotModel>(query, new { transferTransaction.TransactionId }, dbTransaction)).ToList();
                    TransactionSplitSnapshotModel? oldFrom = oldSplits.FirstOrDefault(s => s.Amount < 0);
                    TransactionSplitSnapshotModel? oldTo = oldSplits.FirstOrDefault(s => s.Amount > 0);
                    if (oldFrom != null)
                    {
                        oldFromSinkingFundEnvelopeId = oldFrom.SinkingFundEnvelopeId;
                        oldFromRolloverEnvelopeMonthId = oldFrom.RolloverEnvelopeMonthId;
                        bool reversed = oldFrom.SinkingFundEnvelopeId.HasValue
                            ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(transferTransaction.UserId, oldFrom.SinkingFundEnvelopeId, -oldFrom.Amount, dbTransaction, _connection)
                            : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(transferTransaction.UserId, oldFrom.RolloverEnvelopeMonthId, -oldFrom.Amount, dbTransaction, _connection);
                        if (!reversed)
                        {
                            dbTransaction.Rollback();
                            return new BaseIdResponse(500, "There was an issue reversing the old transfer-from envelope amount");
                        }
                    }
                    if (oldTo != null)
                    {
                        oldToSinkingFundEnvelopeId = oldTo.SinkingFundEnvelopeId;
                        oldToRolloverEnvelopeMonthId = oldTo.RolloverEnvelopeMonthId;
                        bool reversed = oldTo.SinkingFundEnvelopeId.HasValue
                            ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(transferTransaction.UserId, oldTo.SinkingFundEnvelopeId, -oldTo.Amount, dbTransaction, _connection)
                            : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(transferTransaction.UserId, oldTo.RolloverEnvelopeMonthId, -oldTo.Amount, dbTransaction, _connection);
                        if (!reversed)
                        {
                            dbTransaction.Rollback();
                            return new BaseIdResponse(500, "There was an issue reversing the old transfer-to envelope amount");
                        }
                    }
                    // The old splits are replaced below with fresh rows (possibly the same envelopes, new amount)
                    query = "DELETE FROM transaction_split WHERE transaction_id = @TransactionId";
                    await _connection.ExecuteAsync(query, new { transferTransaction.TransactionId }, dbTransaction);
                }

                // Call the transaction update method (updates total_amount/date/notes on `transaction`)
                transactionResponse = await _transactionDAO.UpdateTransactionAsync(transferTransaction, dbTransaction);
                if (transactionResponse.HttpStatus < 200 || transactionResponse.HttpStatus >= 300)
                {
                    dbTransaction.Rollback();
                    return transactionResponse;
                }

                decimal newTotalAmount = transferTransaction.TotalAmount ?? oldSnapshot.TotalAmount;
                TransferTransactionType newType = transferTransaction.TransferTransactionType ?? oldSnapshot.TransferTransactionType;

                if (newType == TransferTransactionType.Account)
                {
                    // Reuse the old account ids only if this was already an account transfer -
                    // switching from Envelope to Account requires both new account ids
                    int? newFromId = transferTransaction.TransferFromId ?? (oldSnapshot.TransferTransactionType == TransferTransactionType.Account ? oldSnapshot.TransferFromAccountId : null);
                    int? newToId = transferTransaction.TransferToId ?? (oldSnapshot.TransferTransactionType == TransferTransactionType.Account ? oldSnapshot.TransferToAccountId : null);
                    if (!newFromId.HasValue || !newToId.HasValue)
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(400, "Both accounts are required for an account transfer");
                    }
                    // Makes sure both accounts belong to the user
                    if (!await _authService.VerifyUserOwnsAccountAsync(newFromId, transferTransaction.UserId, dbTransaction) ||
                        !await _authService.VerifyUserOwnsAccountAsync(newToId, transferTransaction.UserId, dbTransaction))
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(403, "One or both accounts do not belong to the current user");
                    }

                    query = """
                        UPDATE transfer_transaction 
                        SET transfer_transaction_type_id = 2,
                            transfer_from_account_id = @FromId,
                            transfer_to_account_id = @ToId
                        WHERE transfer_transaction_id = @TransferTransactionId
                        """;
                    try
                    {
                        rowsAffected = await _connection.ExecuteAsync(query, new
                        {
                            FromId = newFromId,
                            ToId = newToId,
                            transferTransaction.TransferTransactionId
                        }, dbTransaction);
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(500, $"{ex.Message}");
                    }
                    if (rowsAffected == 0)
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(404, "Transfer transaction not found");
                    }

                    // Apply the new account balances
                    if (!await _accountDAO.UpdateAccountBalanceAsync(transferTransaction.UserId, newFromId, -newTotalAmount, dbTransaction, _connection) ||
                        !await _accountDAO.UpdateAccountBalanceAsync(transferTransaction.UserId, newToId, newTotalAmount, dbTransaction, _connection))
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(500, "There was an issue applying the new account balances");
                    }
                }
                else // newType == Envelope
                {
                    // Resolve the "from" side - reuse the old one only if this was already an
                    // envelope transfer and a new value wasn't supplied
                    int? newFromSinkingFundEnvelopeId, newFromRolloverEnvelopeMonthId;
                    if (transferTransaction.TransferFromId.HasValue)
                    {
                        (int? sfId, int? rmId, CreateTransferTransactionResponse? error) =
                            await ResolveEnvelopeSideAsync(transferTransaction.TransferFromId.Value, transferTransaction.TransferFromIsSinkingFund, oldSnapshot.BudgetMonthId, transferTransaction.UserId, dbTransaction);
                        if (error != null)
                        {
                            dbTransaction.Rollback();
                            return new BaseIdResponse(error.HttpStatus, error.Message);
                        }
                        newFromSinkingFundEnvelopeId = sfId;
                        newFromRolloverEnvelopeMonthId = rmId;
                    }
                    else if (oldSnapshot.TransferTransactionType == TransferTransactionType.Envelope)
                    {
                        newFromSinkingFundEnvelopeId = oldFromSinkingFundEnvelopeId;
                        newFromRolloverEnvelopeMonthId = oldFromRolloverEnvelopeMonthId;
                    }
                    else
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(400, "Both envelopes are required for an envelope transfer");
                    }

                    // Resolve the "to" side the same way
                    int? newToSinkingFundEnvelopeId, newToRolloverEnvelopeMonthId;
                    if (transferTransaction.TransferToId.HasValue)
                    {
                        (int? sfId, int? rmId, CreateTransferTransactionResponse? error) =
                            await ResolveEnvelopeSideAsync(transferTransaction.TransferToId.Value, transferTransaction.TransferToIsSinkingFund, oldSnapshot.BudgetMonthId, transferTransaction.UserId, dbTransaction);
                        if (error != null)
                        {
                            dbTransaction.Rollback();
                            return new BaseIdResponse(error.HttpStatus, error.Message);
                        }
                        newToSinkingFundEnvelopeId = sfId;
                        newToRolloverEnvelopeMonthId = rmId;
                    }
                    else if (oldSnapshot.TransferTransactionType == TransferTransactionType.Envelope)
                    {
                        newToSinkingFundEnvelopeId = oldToSinkingFundEnvelopeId;
                        newToRolloverEnvelopeMonthId = oldToRolloverEnvelopeMonthId;
                    }
                    else
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(400, "Both envelopes are required for an envelope transfer");
                    }

                    // Recreate the two split rows with the (possibly unchanged) envelopes and the new amount
                    query = """
                        INSERT INTO transaction_split (transaction_id, sinking_fund_envelope_id, rollover_envelope_month_id, amount)
                        VALUES (@TransactionId, @SinkingFundEnvelopeId, @RolloverEnvelopeMonthId, @Amount);
                        """;
                    await _connection.ExecuteAsync(query, new
                    {
                        transferTransaction.TransactionId,
                        SinkingFundEnvelopeId = newFromSinkingFundEnvelopeId,
                        RolloverEnvelopeMonthId = newFromRolloverEnvelopeMonthId,
                        Amount = -newTotalAmount
                    }, dbTransaction);
                    await _connection.ExecuteAsync(query, new
                    {
                        transferTransaction.TransactionId,
                        SinkingFundEnvelopeId = newToSinkingFundEnvelopeId,
                        RolloverEnvelopeMonthId = newToRolloverEnvelopeMonthId,
                        Amount = newTotalAmount
                    }, dbTransaction);

                    query = """
                        UPDATE transfer_transaction 
                        SET transfer_transaction_type_id = 3,
                            transfer_from_account_id = null,
                            transfer_to_account_id = null
                        WHERE transfer_transaction_id = @TransferTransactionId
                        """;
                    try
                    {
                        rowsAffected = await _connection.ExecuteAsync(query, new { transferTransaction.TransferTransactionId }, dbTransaction);
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(500, $"{ex.Message}");
                    }
                    if (rowsAffected == 0)
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(404, "Transfer transaction not found");
                    }

                    // Apply the new remaining-amount effects on both sides
                    bool fromApplied = newFromSinkingFundEnvelopeId.HasValue
                        ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(transferTransaction.UserId, newFromSinkingFundEnvelopeId, -newTotalAmount, dbTransaction, _connection)
                        : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(transferTransaction.UserId, newFromRolloverEnvelopeMonthId, -newTotalAmount, dbTransaction, _connection);
                    bool toApplied = newToSinkingFundEnvelopeId.HasValue
                        ? await _sinkingFundEnvelopeDAO.UpdateRemainingAmountAsync(transferTransaction.UserId, newToSinkingFundEnvelopeId, newTotalAmount, dbTransaction, _connection)
                        : await _rolloverEnvelopeDAO.UpdateRemainingAmountForMonthAsync(transferTransaction.UserId, newToRolloverEnvelopeMonthId, newTotalAmount, dbTransaction, _connection);
                    if (!fromApplied || !toApplied)
                    {
                        dbTransaction.Rollback();
                        return new BaseIdResponse(500, "There was an issue applying the new envelope amounts");
                    }
                }

                // Commit the transaction
                dbTransaction.Commit();
            }
            // Return the result
            return new BaseIdResponse(200, "Transfer transaction updated successfully", transferTransaction.TransactionId);
        }
    }
}