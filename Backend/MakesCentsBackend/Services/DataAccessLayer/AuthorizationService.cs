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
                FROM budget_users 
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
        /// Auth method to check if a user owns the specific month's budget instance specified
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsBudgetMonthAsync(int? budgetMonthId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM budget_month
                INNER JOIN budget_users ON budget_month.budget_id = budget_users.budget_id
                WHERE budget_month.budget_month_id = @BudgetMonthId 
                AND budget_users.user_id = @UserId)
            """;

            // Check for nulls
            if (!budgetMonthId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { BudgetMonthId = budgetMonthId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the envelope funding row specified
        /// </summary>
        /// <param name="envelopeFundingId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsEnvelopeFundingAsync(int? envelopeFundingId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM envelope_funding
                INNER JOIN budget_month ON envelope_funding.budget_month_id = budget_month.budget_month_id
                INNER JOIN budget_users ON budget_month.budget_id = budget_users.budget_id
                WHERE envelope_funding.envelope_funding_id = @EnvelopeFundingId 
                AND budget_users.user_id = @UserId)
            """;

            // Check for nulls
            if (!envelopeFundingId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { EnvelopeFundingId = envelopeFundingId, UserId = userId },
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
                INNER JOIN budget_users ON envelope_category.budget_id = budget_users.budget_id
                WHERE envelope_category.envelope_category_id = @EnvelopeCategoryId 
                AND budget_users.user_id = @UserId)
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
        /// Auth method to check if a user owns the sinking fund envelope specified
        /// </summary>
        /// <param name="sinkingFundEnvelopeId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsSinkingFundEnvelopeAsync(int? sinkingFundEnvelopeId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM sinking_fund_envelope
                INNER JOIN envelope_category ON sinking_fund_envelope.envelope_category_id = envelope_category.envelope_category_id
                INNER JOIN budget_users ON envelope_category.budget_id = budget_users.budget_id
                WHERE sinking_fund_envelope.sinking_fund_envelope_id = @SinkingFundEnvelopeId 
                AND budget_users.user_id = @UserId)
            """;

            // Check for nulls
            if (!sinkingFundEnvelopeId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { SinkingFundEnvelopeId = sinkingFundEnvelopeId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the persistent rollover envelope definition specified
        /// (used by planned_expense, which points at the definition rather than a specific month's instance)
        /// </summary>
        /// <param name="rolloverEnvelopeId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsRolloverEnvelopeAsync(int? rolloverEnvelopeId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM rollover_envelope
                INNER JOIN envelope_category ON rollover_envelope.envelope_category_id = envelope_category.envelope_category_id
                INNER JOIN budget_users ON envelope_category.budget_id = budget_users.budget_id
                WHERE rollover_envelope.rollover_envelope_id = @RolloverEnvelopeId 
                AND budget_users.user_id = @UserId)
            """;

            // Check for nulls
            if (!rolloverEnvelopeId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { RolloverEnvelopeId = rolloverEnvelopeId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the specific month's rollover envelope instance specified
        /// (used by transaction_split and paycheck_split, which point at the monthly instance)
        /// </summary>
        /// <param name="rolloverEnvelopeMonthId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsRolloverEnvelopeMonthAsync(int? rolloverEnvelopeMonthId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM rollover_envelope_month
                INNER JOIN rollover_envelope ON rollover_envelope_month.rollover_envelope_id = rollover_envelope.rollover_envelope_id
                INNER JOIN envelope_category ON rollover_envelope.envelope_category_id = envelope_category.envelope_category_id
                INNER JOIN budget_users ON envelope_category.budget_id = budget_users.budget_id
                WHERE rollover_envelope_month.rollover_envelope_month_id = @RolloverEnvelopeMonthId 
                AND budget_users.user_id = @UserId)
            """;

            // Check for nulls
            if (!rolloverEnvelopeMonthId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { RolloverEnvelopeMonthId = rolloverEnvelopeMonthId, UserId = userId },
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
                INNER JOIN budget_users ON account.budget_id = budget_users.budget_id
                WHERE account.account_id = @AccountId 
                AND budget_users.user_id = @UserId)
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
                INNER JOIN budget_users ON paycheck.budget_id = budget_users.budget_id
                WHERE paycheck.paycheck_id = @PaycheckId
                AND budget_users.user_id = @UserId)
            """;

            // Check for nulls
            if (!paycheckId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { PaycheckId = paycheckId, UserId = userId },
                transaction);
        }

        /// <summary>
        /// Auth method to check if a user owns the budget template specified
        /// </summary>
        /// <param name="budgetTemplateId"></param>
        /// <param name="userId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserOwnsBudgetTemplateAsync(int? budgetTemplateId, int? userId, MySqlTransaction? transaction = null)
        {
            // Declare and initialize
            query = """
            SELECT EXISTS(
                SELECT 1 
                FROM budget_template
                INNER JOIN budget_users ON budget_template.budget_id = budget_users.budget_id
                WHERE budget_template.budget_template_id = @BudgetTemplateId
                AND budget_users.user_id = @UserId)
            """;

            // Check for nulls
            if (!budgetTemplateId.HasValue || !userId.HasValue)
                return false;

            // Run the query and return the result
            return await _connection.QuerySingleOrDefaultAsync<bool>(
                query,
                new { BudgetTemplateId = budgetTemplateId, UserId = userId },
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
                INNER JOIN budget_month ON transaction.budget_month_id = budget_month.budget_month_id
                INNER JOIN budget_users ON budget_month.budget_id = budget_users.budget_id
                WHERE transaction.transaction_id = @TransactionId 
                AND budget_users.user_id = @UserId)
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