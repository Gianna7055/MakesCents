/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using MySqlConnector;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class AuthorizationService
    {
        // Class level variables
        private readonly MySqlConnection _connection;
        string query = "";

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public AuthorizationService(MySqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Auth method to check if a user owns the budget specified
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsBudgetAsync(int? budgetId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM budget 
                WHERE budget_id = @BudgetId 
                AND user_id = @UserId)
            """; 

            // Check for nulls
            if (!budgetId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { BudgetId = budgetId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the envelope category specified
        /// </summary>
        /// <param name="envelopeCategoryId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsEnvelopeCategoryAsync(int? envelopeCategoryId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM envelope_category
                INNER JOIN budget ON envelope_category.budget_id = budget.budget_id
                WHERE envelope_category.envelope_category_id = @EnvelopeCategoryId 
                AND budget.user_id = @UserId)
            """;

            // Check for nulls
            if (!envelopeCategoryId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { EnvelopeCategoryId = envelopeCategoryId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the envelope specified
        /// </summary>
        /// <param name="envelopeId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsEnvelopeAsync(int? envelopeId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM envelope
                INNER JOIN envelope_category ON envelope.envelope_category_id = envelope_category.envelope_category_id
                INNER JOIN budget ON envelope_category.budget_id = budget.budget_id
                WHERE envelope.envelope_id = @EnvelopeId 
                AND budget.user_id = @UserId)
            """;

            // Check for nulls
            if (!envelopeId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { EnvelopeId = envelopeId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the account specified
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsAccountAsync(int? accountId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM account
                INNER JOIN budget ON account.budget_id = budget.budget_id
                WHERE account.account_id = @AccountId 
                AND budget.user_id = @UserId)
            """;

            // Check for nulls
            if (!accountId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { AccountId = accountId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the paycheck specified
        /// </summary>
        /// <param name="paycheckId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsPaycheckAsync(int? paycheckId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM paycheck
                INNER JOIN budget ON paycheck.budget_id = budget.budget_id
                WHERE paycheck.paycheck_id = @PaycheckId
                AND budget.user_id = @UserId)
            """;

            // Check for nulls
            if (!paycheckId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { AccountId = paycheckId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the transaction specified
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsTransactionAsync(int? transactionId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM transaction
                INNER JOIN budget ON transaction.budget_id = budget.budget_id
                WHERE transaction.transaction_id = @TransactionId 
                AND budget.user_id = @UserId)
            """;

            // Check for nulls
            if (!transactionId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { TransactionId = transactionId, UserId = userId },
                transaction);
        }
    }
}
