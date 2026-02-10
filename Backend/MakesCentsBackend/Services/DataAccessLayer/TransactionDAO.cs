/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MySqlConnector;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class TransactionDAO
    {
        // Class level variables
        // string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public TransactionDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }
    }
}
