/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using MakesCentsBackend.Models;
using MySqlConnector;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class AccountDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public AccountDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to update the balance of an account by the amount
        /// </summary>
        /// <param name="dbTransaction"></param>
        /// <param name="userId"></param>
        /// <param name="accountId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAccountBalanceAsync(int? userId, int? accountId, decimal? amount, MySqlTransaction dbTransaction)
        {
            // Declare and initialize
            query = """
                UPDATE account
                SET balance = balance + @Amount
                WHERE accountId = @AccountId;
                """;

            // Make sure the account belongs to the user
            if (!await _authService.VerifyUserOwnsAccountAsync(accountId, userId))
            {
                return false;
            }
            // Execute the request
            try
            {
                // Execute
                await _connection.ExecuteAsync(query, new
                {
                    Amount = amount,
                    AccountId = accountId
                }, dbTransaction);
            }
            catch (Exception)
            {
                return false;
            }
            // Return true
            return true;
        }


        public async Task<GetAllAccountsResponse> GetAllAccountsAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetAllAccountsResponse response = new GetAllAccountsResponse();
            List<AccountSummaryDTO> accounts = new List<AccountSummaryDTO>();

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetAllAccountsResponse(403, "Budget does not belong to the current user");
            }
            // Set up the query to get all bank accounts
            query = """
                SELECT 
                    account.account_id AS AccountId,
                    account.budget_id AS BudgetId,
                    account.account_type_id AS AccountTypeId,
                    account.account_name AS AccountName,
                    account.balance AS Balance,
                    bank_account.bank_account_id AS BankAccountId,
                    bank_account.bank_account_type_id AS BankAccountTypeId
                FROM account
                INNER JOIN bank_account ON account.account_id = bank_account.account_id
                WHERE account.budget_id = @BudgetId
                ORDER BY account.account_name
                """;
            // Read the list of bank accounts into the accounts list
            accounts.AddRange((await _connection.QueryAsync<BankAccountSummaryDTO>(query, new { BudgetId = request.EntityId })).ToList());

            // Set up the query to get all the debt accounts
            query = """
                SELECT 
                    account.account_id AS AccountId,
                    account.budget_id AS BudgetId,
                    account.account_type_id AS AccountTypeId,
                    account.account_name AS AccountName,
                    account.balance AS Balance,
                    debt_account.debt_account_id AS DebtAccountId,
                    debt_account.debt_account_type_id AS DebtAccountTypeId
                FROM account
                INNER JOIN debt_account ON account.account_id = debt_account.account_id
                WHERE account.budget_id = @BudgetId
                ORDER BY account.account_name
                """;
            // Read the list of debt accounts into the accounts list
            accounts.AddRange((await _connection.QueryAsync<DebtAccountSummaryDTO>(query, new { BudgetId = request.EntityId })).ToList());

            // Set up the query to get all the investment accounts
            query = """
                SELECT 
                    account.account_id AS AccountId,
                    account.budget_id AS BudgetId,
                    account.account_type_id AS AccountTypeId,
                    account.account_name AS AccountName,
                    account.balance AS Balance,
                    investment_account.investment_account_id AS InvestmentAccountId,
                    investment_account.investment_account_type_id AS InvestmentAccountTypeId
                FROM account
                INNER JOIN investment_account ON account.account_id = investment_account.account_id
                WHERE account.budget_id = @BudgetId
                ORDER BY account.account_name
                """;
            // Read the list of investment accounts into the accounts list
            accounts.AddRange((await _connection.QueryAsync<InvestmentAccountSummaryDTO>(query, new { BudgetId = request.EntityId })).ToList());

            // Set the list in the response
            response.Accounts = accounts;
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Accounts found";
            // Return the response
            return response;
        }
    }
}
