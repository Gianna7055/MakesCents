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
    public class PaymentTransactionDAO
    {
        // Class level variables
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        string query = "";

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public PaymentTransactionDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }


        public async Task<CreatePaymentTransactionResponse> CreatePaymentTransactionAsync(CreatePaymentTransactionRequest paymentTransaction)
        {
            // Declare and initialize
            CreatePaymentTransactionResponse response = new CreatePaymentTransactionResponse();
            int paycheckId;
            int paycheckSplitId;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                try
                {
                    // Make sure the budget belongs to the user
                    if (!await _authService.VerifyUserOwnsBudgetAsync(paymentTransaction.BudgetId, paymentTransaction.UserId, dbTransaction))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new CreatePaymentTransactionResponse(403, "Budget does not belong to the current user");
                    }
                    // Set up the query for adding a paycheck
                    query = """
                        INSERT INTO transaction (budget_id, transaction_date, total_amount, is_reconciled, notes, transaction_type_id)
                        VALUES (@BudgetId, @TransactionDate, @TotalAmount, false, @Notes, 2);
                        SELECT LAST_INTSERT_ID();
                        """;
                    // Execute the query and get the transaction id
                    paycheckId = await _connection.QuerySingleAsync<int>(query, paymentTransaction, dbTransaction);
                    // Set the transaction id in the response
                    response.T
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new CreatePaymentTransactionResponse(500, $"{ex.Message}");
                }
            }
        }
    }
}
