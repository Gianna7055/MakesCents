/*
 * Gianna Ross
 * File Created: 1/23/2026
 * File Last Updated: 1/23/2026
 * Makes Cents - Budget DAO
 * Sources: 
 */
using System.Data;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class BudgetDAO
    {
        // Class level variables
        string query = "";
        private readonly IDbConnection _connection;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public BudgetDAO(IDbConnection connection)
        {
            _connection = connection;
        }
    }
}
