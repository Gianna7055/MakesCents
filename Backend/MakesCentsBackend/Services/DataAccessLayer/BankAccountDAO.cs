/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using MakesCentsBackend.Models;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<CreateBankAccountResponse> CreateBankAccountAsync(CreateBankAccountRequest bankAccount)
        {
            // Declare and initialize
            int accountId;
            int bankAccountId;
            CreateBankAccountResponse response = new CreateBankAccountResponse();

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
                        return new CreateBankAccountResponse(403, "Budget does not belong to the current user.");
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
                    // Set the account id
                    response.AccountId = accountId;

                    // Query for insert for bank account table
                    query = """
                        INSERT INTO bank_account (account_id, bank_account_type_id)
                        VALUES (@AccountId, @BankAccountType);
                        SELECT LAST_INSERT_ID();
                        """;
                    bankAccountId = await _connection.QuerySingleAsync<int>(query, bankAccount, dbTransaction);
                    // Set the bank account id in the response
                    response.BankAccountId = bankAccountId;
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
                            return new CreateBankAccountResponse(400, "Account name already exists in this budget");
                        }
                    }
                    return new CreateBankAccountResponse(500, $"{ex.Number}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new CreateBankAccountResponse(500, $"{ex.Message}");
                }
            }
            // Set the status and message
            response.HttpStatus = 201;
            response.Message = "Bank account created successfully";
            // Return the result
            return response;
        }


        public async Task<GetBankAccountEntityResponse> GetBankAccountAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetBankAccountEntityResponse response = new GetBankAccountEntityResponse();
            GetBankAccountEntity bankAccount;

            // Make sure the account belongs to the user
            if (!await _authService.VerifyUserOwnsAccountAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetBankAccountEntityResponse(403, "Account does not belong to the user");
            }
            // Set up the query to get the bank account
            query = """
                SELECT 
                    account.account_id AS AccountId,
                    account.budget_id AS BudgetId,
                    account.account_type_id AS AccountTypeId,
                    account.account_name AS AccountName,
                    account.institution AS Institution,
                    account.balance AS Balance,
                    bank_account.bank_account_id AS BankAccountId,
                    bank_account.bank_account_type_id AS BankAccountTypeId
                FROM account
                INNER JOIN bank_account ON account.account_id = bank_account.account_id
                WHERE bank_account.bank_account_id = @BankAccountId;
                """;
            // Get the bank account
            bankAccount = await _connection.QueryFirstAsync<GetBankAccountEntity>(query, new { BankAccountId = request.EntityId });
            // Make sure the bank account is not null
            if (bankAccount == null)
            {
                return new GetBankAccountEntityResponse(404, "Bank account not found");
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
            bankAccount.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { AccountId = request.EntityId })).ToList();

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
            bankAccount.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { AccountId = request.EntityId })).ToList());
            // Set the bank account in the response model
            response.BankAccount = bankAccount;
            // Set the status and the message for the response
            response.HttpStatus = 200;
            response.Message = "Bank account found";
            // Return the response
            return response;
        }

    }
}