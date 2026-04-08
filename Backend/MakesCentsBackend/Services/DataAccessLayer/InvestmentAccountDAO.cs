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
    public class InvestmentAccountDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        private readonly AccountDAO _accountDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public InvestmentAccountDAO(MySqlConnection connection, AuthorizationService authService, AccountDAO accountDAO)
        {
            _connection = connection;
            _authService = authService;
            _accountDAO = accountDAO;
        }

        /// <summary>
        /// DAO method to create an investment account
        /// </summary>
        /// <param name="investmentAccount"></param>
        /// <returns></returns>
        public async Task<CreateInvestmentAccountResponse> CreateInvestmentAccountAsync(CreateInvestmentAccountRequest investmentAccount)
        {
            // Declare and initialize
            int accountId;
            int investmentAccountId;
            CreateInvestmentAccountResponse response = new CreateInvestmentAccountResponse();

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
                        return new CreateInvestmentAccountResponse(403, "Budget does not belong to the current user");
                    }
                    // Query for the account table
                    query = """
                        INSERT INTO account (budget_id, account_type_id, account_name, institution, balance)
                        VALUES (@BudgetId, 3, @AccountName, @Institution, @Balance);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Get the new account id
                    accountId = await _connection.QuerySingleAsync<int>(query, investmentAccount, dbTransaction);
                    // Add the account id to the model
                    investmentAccount.AccountId = accountId;
                    // Set the account id
                    response.AccountId = accountId;

                    // Query for insert for investment account table
                    query = """
                        INSERT INTO investment_account (account_id, investment_account_type_id, investment_account_number, is_tax_deferred, is_tax_exempt)
                        VALUES (@AccountId, @InvestmentAccountType, @InvestmentAccountNumber, @IsTaxDeferred, @IsTaxExempt);
                        SELECT LAST_INSERT_ID();
                        """;
                    investmentAccountId = await _connection.QuerySingleAsync<int>(query, investmentAccount, dbTransaction);
                    // Set the investment account id
                    response.InvestmentAccountId = investmentAccountId;
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
                            return new CreateInvestmentAccountResponse(400, "Account name already exists in this budget");
                        }
                    }
                    return new CreateInvestmentAccountResponse(500, $"{ex.Number}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new CreateInvestmentAccountResponse(500, $"{ex.Message}");
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Set the status and message
            response.HttpStatus = 201;
            response.Message = "Investment account created successfully";
            // Return the result
            return response;
        }

        public async Task<GetInvestmentAccountEntityResponse> GetInvestmentAccountAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetInvestmentAccountEntityResponse response = new GetInvestmentAccountEntityResponse();
            GetInvestmentAccountEntityModel investmentAccount;

            // Make sure the account belongs to the user
            if (!await _authService.VerifyUserOwnsAccountAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetInvestmentAccountEntityResponse(403, "Account does not belong to the user");
            }
            // Set up the query to get the investment account
            query = """
                SELECT 
                    account.account_id AS AccountId,
                    account.budget_id AS BudgetId,
                    account.account_type_id AS AccountTypeId,
                    account.account_name AS AccountName,
                    account.institution AS Institution,
                    account.balance AS Balance,
                    investment_account.investment_account_id AS InvestmentAccountId,
                    investment_account.investment_account_type_id AS InvestmentAccountTypeId,
                    investment_account.investment_account_number AS InvestmentAccountNumber,
                    investment_account.is_tax_deferred AS IsTaxDeferred,
                    investment_account.is_tax_exempt AS IsTaxExempt
                FROM account
                INNER JOIN investment_account ON account.account_id = investment_account.account_id
                WHERE account.account_id = @AccountId;
                """;
            // Get the investment account
            investmentAccount = await _connection.QueryFirstAsync<GetInvestmentAccountEntityModel>(query, new { AccountId = request.EntityId });
            // Make sure the investment account is not null
            if (investmentAccount == null)
            {
                return new GetInvestmentAccountEntityResponse(404, "Investment account not found");
            }

            // Set up the query to get the list of payment transactions
            query = """
                SELECT 
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionTypeId,
                    transaction.total_amount AS TotalAmount,
                    payment_transaction.merchant_source_name AS MerchantSourceName,
                    GROUP_CONCAT(DISTINCT envelope.envelope_name SEPARATOR ', ') AS EnvelopeNames
                FROM payment_transaction
                INNER JOIN transaction ON payment_transaction.transaction_id = transaction.transaction_id
                LEFT JOIN transaction_split ON transaction.transaction_id = transaction_split.transaction_id
                LEFT JOIN envelope ON transaction_split.envelope_id = envelope.envelope_id
                WHERE payment_transaction.account_id = @AccountId
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
            // Read the list of payment transactions
            investmentAccount.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { AccountId = request.EntityId })).ToList();

            // Set up the query to get the list of transfer transactions
            query = """
                SELECT 
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionTypeId,
                    transaction.total_amount AS TotalAmount,
                    transfer_transaction.transfer_from_account_id AS TransferFromAccountId,
                    transfer_transaction.transfer_to_account_id AS TransferToAccountId,
                    transfer_transaction.transfer_from_envelope_id AS TransferFromEnvelopeId,
                    transfer_transaction.transfer_to_envelope_id AS TransferToEnvelopeId
                FROM transfer_transaction
                INNER JOIN transaction ON transfer_transaction.transaction_id = transaction.transaction_id
                WHERE (transfer_transaction.transfer_from_account_id = @AccountId 
                       OR transfer_transaction.transfer_to_account_id = @AccountId)
                  AND transaction.deleted_at IS NULL
                  AND transaction.transaction_type_id = 3
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the list of transfer transactions
            investmentAccount.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { AccountId = request.EntityId })).ToList());
            // Set the investment account in the response model
            response.InvestmentAccount = investmentAccount;
            // Set the status and the message for the response
            response.HttpStatus = 200;
            response.Message = "Investment account found";
            // Return the response
            return response;
        }


        public async Task<BaseIdResponse> UpdateInvestmentAccountAsync(UpdateInvestmentAccountRequest investmentAccount)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;
            BaseIdResponse accountResponse;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Setting up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Call the update account method
                accountResponse = await _accountDAO.UpdateAccountAsync(investmentAccount, dbTransaction);

                // Check the account response
                if (accountResponse.HttpStatus != 200 || accountResponse.Message != "No fields to update")
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return accountResponse;
                }

                // Make sure the investment account belongs to the user
                if (!await _authService.VerifyUserOwnsAccountAsync(investmentAccount.InvestmentAccountId, investmentAccount.UserId, dbTransaction))
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new BaseIdResponse(403, "Investment account does not belong to the current user", investmentAccount.InvestmentAccountId);
                }

                // Loop through each field to see if an update is necessary
                foreach (PropertyInfo property in typeof(UpdateInvestmentAccountRequest).GetProperties())
                {
                    // Skip base table properties
                    if (typeof(UpdateAccountRequest).GetProperty(property.Name) != null) continue;
                    // Skip the id
                    if (property.Name == "InvestmentAccountId" || property.Name == "UserId") continue;
                    object? propertyValue = property.GetValue(investmentAccount);

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
                UPDATE investment_account
                SET {string.Join(", ", updates)}
                WHERE investment_account_id = @InvestmentAccount
                """;
                try
                {
                    // Execute the query
                    rowsAffected = await _connection.ExecuteAsync(query, investmentAccount, dbTransaction);
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new BaseIdResponse(500, $"{ex.Message}");
                }
                // Commit the transaction
                dbTransaction.Commit();
            }

            // Make sure the row was affected
            if (rowsAffected == 1)
            {
                return new BaseIdResponse(200, "Investment account updated successfully", investmentAccount.InvestmentAccountId);
            }
            else if (rowsAffected == 0)
            {
                return new BaseIdResponse(404, "Investment account not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
            }
        }
    }
}
