/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using MakesCentsBackend.Models;
using MySqlConnector;
using System.Data;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class InvestmentAccountDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public InvestmentAccountDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create an investment account
        /// </summary>
        /// <param name="investmentAccount"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateInvestmentAccountAsync(CreateInvestmentAccountRequest investmentAccount)
        {
            // Declare and initialize
            int accountId;
            int investmentAccountId;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Set up the try catch to roll the transaction back if anything fails
                try
                {
                    // Make sure the budget belongs to the user
                    if (!await _authService.VerifyUserOwnsBudgetAsync(investmentAccount.BudgetId, investmentAccount.UserId, dbTransaction))
                    {
                        return new BaseIdResponse(403, "Budget does not belong to the current user");
                    }
                    // Query for the account table
                    query = """
                        INSERT INTO account (budget_id, account_type_id, account_name, institution, balance)
                        VALUES (@BudgetId, 3, @AccountName, @Institution, @Balance);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Get the new account id
                    accountId = await _connection.QuerySingleAsync<int>(query, investmentAccount, dbTransaction);

                    // Query for insert for investment account table
                    query = """
                        INSERT INTO investment_account (account_id, investment_account_type_id, investment_account_number, is_tax_deferred, is_tax_exempt)
                        VALUES (@BudgetId, @InvestmentAccountType, @InvestmentAccountNumber, @IsTaxDeferred, @IsTaxExempt);
                        SELECT LAST_INSERT_ID();
                        """;
                    investmentAccountId = await _connection.QuerySingleAsync<int>(query, investmentAccount, dbTransaction);

                    // Commit the transaction
                    dbTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Check the specific SQL error
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
            }
            // Return the result
            return new BaseIdResponse(201, "Investment account created successfully", investmentAccountId);
        }
    }
}
