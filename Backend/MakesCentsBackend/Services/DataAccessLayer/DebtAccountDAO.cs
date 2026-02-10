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
    public class DebtAccountDAO
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
        public DebtAccountDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create a new debt account
        /// </summary>
        /// <param name="debtAccount"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateDebtAccountAsync(CreateDebtAccountRequest debtAccount)
        {
            // Declare and initialize
            int accountId;
            int debtAccountId;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Setting up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Set up the try catch to roll the transaction back if anything fails
                try
                {
                    // Make sure the budget belongs to the user
                    if (!await _authService.VerifyUserOwnsBudgetAsync(debtAccount.BudgetId, debtAccount.UserId, dbTransaction))
                    {
                        return new BaseIdResponse(403, "Budget does not belong to the current user.");
                    }
                    // Query for insert for account table
                    query = """
                        INSERT INTO account (budget_id, account_type_id, account_name, institution, balance)
                        VALUES (@BudgetId, 3, @AccountName, @Institution, @Balance);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Get the new account id
                    accountId = await _connection.QuerySingleAsync<int>(query, debtAccount, dbTransaction);
                    // Add the account id to the model
                    debtAccount.AccountId = accountId;

                    // Query for insert for debt account table
                    query = """
                        INSERT INTO debt_account (account_id, debt_account_type_id, debt_account_number, date_of_next_bill, amount_of_next_bill, debt_payment_regularity_id)
                        VALUES (@BudgetId, @DebtAccountType, @DebtAccountNumber, @DateOfNextBill, @AmountOfNextBill, @DebtPaymentRegularity);
                        SELECT LAST_INSERT_ID();
                        """;
                    debtAccountId = await _connection.QuerySingleAsync<int>(query, debtAccount, dbTransaction);

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
            return new BaseIdResponse(201, "Debt account created successfully", debtAccountId);
        }
    }
}
