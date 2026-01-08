/*
 * Gianna Ross
 * File Created: 12/15/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - User DAO
 * Sources: 
 */
using MakesCentsBackend.Models;
using MySql.Data.MySqlClient;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    /// <summary>
    /// Data Access Object for Users
    /// </summary>
    public class UserDAO
    {
        // Class level variables
        private static readonly string serverName = "localhost";
        private static readonly string username = "root";
        private static readonly string password = "root";
        private static readonly string dbName = "makes-cents";
        private static readonly string port = "3306";
        private static readonly string connectionString = $"server={serverName};user={username};database={dbName};port={port};password={password};";
        string query = "";

        /// <summary>
        /// Add a new user based on a user model
        /// </summary>
        /// <param name="user">the data for the new user</param>
        /// <returns>The Id of the new user</returns>
        public int CreateUser(UserModel user)
        {
            // Declare and initialize

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

            }
        }
    }
}
