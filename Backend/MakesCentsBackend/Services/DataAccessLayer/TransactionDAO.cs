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
    public class TransactionDAO
    {
        // Class level variables
        // string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        string query = "";

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public TransactionDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }


        public async Task<GetAllTransactionsEntityResponse> GetAllTransactionsAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetAllTransactionsEntityResponse response = new GetAllTransactionsEntityResponse();

            // Set up the query for reading the payment transactions
            query = """
                SELECT 
                    transaction.transaction_id AS TransactionId,
                    transaction.transaction_date AS Date,
                    transaction.transaction_type_id AS TransactionTypeId,
                    transaction.total_amount AS TotalAmount,
                    payment_transaction.merchant_source_name AS MerchantSourceName,
                    GROUP_CONCAT(DISTINCT envelope.envelope_name SEPARATOR ', ') AS EnvelopeNames
                FROM transaction_split
                INNER JOIN transaction ON transaction_split.transaction_id = transaction.transaction_id
                INNER JOIN payment_transaction ON transaction.transaction_id = payment_transaction.transaction_id
                LEFT JOIN transaction_split AS ts2 ON transaction.transaction_id = ts2.transaction_id
                LEFT JOIN envelope ON ts2.envelope_id = envelope.envelope_id
                WHERE transaction_split.envelope_id = @EnvelopeId
                  AND transaction.deleted_at IS NULL
                  AND transaction.transaction_type_id = 2
                GROUP BY transaction.transaction_id
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the payment transactions
            response.Transactions = (await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { EnvelopeId = request.EntityId })).ToList();
            // Set up the query to read the transfer transactions
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
                FROM transaction_split
                INNER JOIN transaction ON transaction_split.transaction_id = transaction.transaction_id
                INNER JOIN transfer_transaction ON transaction.transaction_id = transfer_transaction.transaction_id
                WHERE transaction_split.envelope_id = @EnvelopeId
                  AND transaction.deleted_at IS NULL
                  AND transaction.transaction_type_id = 3
                ORDER BY transaction.transaction_date DESC;
                """;
            // Read the transfer transactions
            response.Transactions.AddRange((await _connection.QueryAsync<SummaryTransactionEntityModel>(query, new { EnvelopeId = request.EntityId })).ToList());
            // Set the status and message for the envelope response
            response.HttpStatus = 200;
            response.Message = "Transactions found";
            // Return the response
            return response;
        }
    }
}
