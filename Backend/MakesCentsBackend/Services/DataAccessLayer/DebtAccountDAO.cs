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
    public class DebtAccountDAO
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
        public DebtAccountDAO(MySqlConnection connection, AuthorizationService authService, AccountDAO accountDAO)
        {
            _connection = connection;
            _authService = authService;
            _accountDAO = accountDAO;
        }

        /// <summary>
        /// DAO method to create a new debt account
        /// </summary>
        /// <param name="debtAccount"></param>
        /// <returns></returns>
        public async Task<CreateDebtAccountResponse> CreateDebtAccountAsync(CreateDebtAccountRequest debtAccount)
        {
            // Declare and initialize
            int accountId;
            int debtAccountId;
            CreateDebtAccountResponse response = new CreateDebtAccountResponse();

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
                        return new CreateDebtAccountResponse(403, "Budget does not belong to the current user");
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
                    // Set the account id
                    response.AccountId = accountId;

                    // Query for insert for debt account table
                    query = """
                        INSERT INTO debt_account (account_id, debt_account_type_id, debt_account_number, date_of_next_bill, amount_of_next_bill, debt_payment_regularity_id)
                        VALUES (@BudgetId, @DebtAccountType, @DebtAccountNumber, @DateOfNextBill, @AmountOfNextBill, @DebtPaymentRegularity);
                        SELECT LAST_INSERT_ID();
                        """;
                    debtAccountId = await _connection.QuerySingleAsync<int>(query, debtAccount, dbTransaction);
                    // Set the debt account id
                    response.DebtAccountId = debtAccountId;

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
                            return new CreateDebtAccountResponse(400, "Account name already exists in this budget");
                        }
                    }
                    return new CreateDebtAccountResponse(500, $"{ex.Number}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new CreateDebtAccountResponse(500, $"{ex.Message}");
                }
                // Commit the transaction
                dbTransaction.Commit();  
            }
            // Set the status and message
            response.HttpStatus = 201;
            response.Message = "Debt account created successfully";
            // Return the result
            return response;
        }


        public async Task<GetDebtAccountEntityResponse> GetDebtAccountAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetDebtAccountEntityResponse response = new GetDebtAccountEntityResponse();
            GetDebtAccountEntityModel debtAccount;

            // Make sure the account belongs to the user
            if (!await _authService.VerifyUserOwnsAccountAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetDebtAccountEntityResponse(403, "Account does not belong to the user");
            }
            // Set up the query to get the debt account
            query = """
                SELECT 
                    account.account_id AS AccountId,
                    account.budget_id AS BudgetId,
                    account.account_type_id AS AccountTypeId,
                    account.account_name AS AccountName,
                    account.institution AS Institution,
                    account.balance AS Balance,
                    debt_account.debt_account_id AS DebtAccountId,
                    debt_account.debt_account_type_id AS DebtAccountTypeId,
                    debt_account.debt_account_number AS DebtAccountNumber,
                    debt_account.date_of_next_bill AS DateOfNextBill,
                    debt_account.amount_of_next_bill AS AmountOfNextBill,
                    debt_account.debt_payment_regularity_id AS DebtPaymentRegularityId
                FROM account
                INNER JOIN debt_account ON account.account_id = debt_account.account_id
                WHERE debt_account.debt_account_id = @DebtAccountId;
                """;
            // Get the debt account
            debtAccount = await _connection.QueryFirstAsync<GetDebtAccountEntityModel>(query, new { DebtAccountId = request.EntityId });
            // Make sure the debt account is not null
            if (debtAccount == null)
            {
                return new GetDebtAccountEntityResponse(404, "Debt account not found");
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
                GROUP BY transaction.transaction_id
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the list of payment transactions
            debtAccount.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { AccountId = request.EntityId })).ToList();

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
            debtAccount.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { AccountId = request.EntityId })).ToList());
            // Set the debt account in the response model
            response.DebtAccount = debtAccount;
            // Set the status and the message for the response
            response.HttpStatus = 200;
            response.Message = "Debt account found";
            // Return the response
            return response;
        }


        public async Task<BaseIdResponse> UpdateDebtAccountAsync(UpdateDebtAccountRequest debtAccount)
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
                accountResponse = await _accountDAO.UpdateAccountAsync(debtAccount, dbTransaction);

                // Check the account response
                if (accountResponse.HttpStatus != 200 || accountResponse.Message != "No fields to update")
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return accountResponse;
                }

                // Make sure the debt account belongs to the user
                if (!await _authService.VerifyUserOwnsAccountAsync(debtAccount.DebtAccountId, debtAccount.UserId, dbTransaction))
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new BaseIdResponse(403, "Debt account does not belong to the current user", debtAccount.DebtAccountId);
                }

                // Loop through each field to see if an update is necessary
                foreach (PropertyInfo property in typeof(UpdateDebtAccountRequest).GetProperties())
                {
                    // Skip base table properties
                    if (typeof(UpdateAccountRequest).GetProperty(property.Name) != null) continue;
                    // Skip the id
                    if (property.Name == "DebtAccountId" || property.Name == "UserId") continue;
                    object? propertyValue = property.GetValue(debtAccount);

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
                UPDATE debt_account
                SET {string.Join(", ", updates)}
                WHERE debt_account_id = @DebtAccount
                """;
                try
                {
                    // Execute the query
                    rowsAffected = await _connection.ExecuteAsync(query, debtAccount, dbTransaction);
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
                return new BaseIdResponse(200, "Debt account updated successfully", debtAccount.DebtAccountId);
            }
            else if (rowsAffected == 0)
            {
                return new BaseIdResponse(404, "Debt account not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
            }
        }
    }
}
