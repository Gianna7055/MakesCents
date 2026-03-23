/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using Humanizer;
using MakesCentsBackend.Models;
using MySqlConnector;
using System.Collections.Generic;
using System.Reflection;

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
        public async Task<bool> UpdateAccountBalanceAsync(int? userId, int? accountId, decimal? amount, MySqlTransaction dbTransaction, MySqlConnection connection)
        {
            // Declare and initialize
            query = """
                UPDATE account
                SET balance = balance + @Amount
                WHERE account_id = @AccountId;
                """;

            // Make sure the account belongs to the user
            if (!await _authService.VerifyUserOwnsAccountAsync(accountId, userId, dbTransaction))
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


        public async Task<GetAllAccountsResponse> GetAllAccountsAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllAccountsResponse response = new GetAllAccountsResponse();
            List<SummaryAccountDTOModel> accounts = new List<SummaryAccountDTOModel>();

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
                    account.account_type_id AS AccountType,
                    account.account_name AS AccountName,
                    account.balance AS Balance,
                    account.institution AS Institution,
                    bank_account.bank_account_id AS BankAccountId,
                    bank_account.bank_account_type_id AS BankAccountType
                FROM account
                INNER JOIN bank_account ON account.account_id = bank_account.account_id
                WHERE account.budget_id = @BudgetId
                ORDER BY account.account_name
                """;
            // Read the list of bank accounts into the accounts list
            accounts.AddRange((await _connection.QueryAsync<BankAccountSummaryDTOModel>(query, new { BudgetId = request.EntityId })).ToList());

            // Set up the query to get all the debt accounts
            query = """
                SELECT 
                    account.account_id AS AccountId,
                    account.budget_id AS BudgetId,
                    account.account_type_id AS AccountType,
                    account.account_name AS AccountName,
                    account.balance AS Balance,
                    debt_account.debt_account_id AS DebtAccountId,
                    debt_account.debt_account_type_id AS DebtAccountType
                FROM account
                INNER JOIN debt_account ON account.account_id = debt_account.account_id
                WHERE account.budget_id = @BudgetId
                ORDER BY account.account_name
                """;
            // Read the list of debt accounts into the accounts list
            accounts.AddRange((await _connection.QueryAsync<DebtAccountSummaryDTOModel>(query, new { BudgetId = request.EntityId })).ToList());

            // Set up the query to get all the investment accounts
            query = """
                SELECT 
                    account.account_id AS AccountId,
                    account.budget_id AS BudgetId,
                    account.account_type_id AS AccountType,
                    account.account_name AS AccountName,
                    account.balance AS Balance,
                    investment_account.investment_account_id AS InvestmentAccountId,
                    investment_account.investment_account_type_id AS InvestmentAccountType
                FROM account
                INNER JOIN investment_account ON account.account_id = investment_account.account_id
                WHERE account.budget_id = @BudgetId
                ORDER BY account.account_name
                """;
            // Read the list of investment accounts into the accounts list
            accounts.AddRange((await _connection.QueryAsync<InvestmentAccountSummaryDTOModel>(query, new { BudgetId = request.EntityId })).ToList());

            // Set the list in the response
            response.Accounts = accounts;
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Accounts found";
            // Return the response
            return response;
        }


        public async Task<BaseIdResponse> UpdateAccountAsync(UpdateAccountRequest account, MySqlTransaction dbTransaction)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the account belongs to the user
            if (!await _authService.VerifyUserOwnsAccountAsync(account.AccountId, account.UserId))
            {
                return new BaseIdResponse(403, "Account does not belong to the current user", account.AccountId);
            }

            // Loop through each field to see if an update is necessary
            foreach (PropertyInfo property in typeof(UpdateAccountRequest).GetProperties())
            {
                // Skip the id
                if (property.Name == "AccountId" || property.Name == "UserId") continue;
                object? propertyValue = property.GetValue(account);

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
                UPDATE account 
                SET {string.Join(", ", updates)}
                WHERE account_id = @AccountId
                """;
            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, account, dbTransaction);
            }
            catch (MySqlException ex)
            {
                // Roll the transaction back
                dbTransaction.Rollback();
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_account_name'"))
                    {
                        return new BaseIdResponse(400, "Account name already exists in this budget");
                    }
                }
                return new BaseIdResponse(500, $"{ex.Number}: {ex.Message}");
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
                return new BaseIdResponse(200, "Account updated successfully", account.AccountId);
            }
            else if (rowsAffected == 0)
            {
                // Roll the transaction back
                dbTransaction.Rollback();
                return new BaseIdResponse(404, "Account not found");
            }
            else
            {
                // Roll the transaction back
                dbTransaction.Rollback();
                return new BaseIdResponse(400, "An error occurred");
            }
        }


        public async Task<BaseResponse> DeleteAccountAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM account 
                WHERE account_id = @AccountId
                """;
            int rowsAffected;

            // Make sure the account belongs to the user
            if (!await _authService.VerifyUserOwnsAccountAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Account does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { AccountId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the user was not found
                return new BaseResponse(404, "Account not found");
            }
            // Return the success
            return new BaseResponse(200, "Account deleted successfully");
        }
    }
}
