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
    /// <summary>
    /// Data access object for bank accounts
    /// </summary>
    /// <remarks>
    /// Parameterized constructor to bring in DI variables
    /// </remarks>
    /// <param name="connection"></param>
    public class BankAccountDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public BankAccountDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create a new bank account
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBankAccountAsync(CreateBankAccountRequest bankAccount)
        {
            // Declare and initialize
            int accountId;
            int bankAccountId;

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
                    if (!await _authService.VerifyUserOwnsBudgetAsync(bankAccount.BudgetId, bankAccount.UserId, dbTransaction))
                    {
                        return new BaseIdResponse(403, "Budget does not belong to the current user.");
                    }
                    // Query for insert for account table
                    query = """
                        INSERT INTO account (budget_id, account_type_id, account_name, institution, balance)
                        VALUES (@BudgetId, 2, @AccountName, @Institution, @Balance);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Get the new account id
                    accountId = await _connection.QuerySingleAsync<int>(query, bankAccount, dbTransaction);
                    // Add the account id to the model
                    bankAccount.AccountId = accountId;

                    // Query for insert for bank account table
                    query = """
                        INSERT INTO bank_account (account_id, bank_account_type_id)
                        VALUES (@AccountId, @BankAccountType);
                        SELECT LAST_INSERT_ID();
                        """;
                    bankAccountId = await _connection.QuerySingleAsync<int>(query, bankAccount, dbTransaction);

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
            return new BaseIdResponse(201, "Bank account created successfully", bankAccountId);
        }
    }
}