using System.Data;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class TransactionDAO
    {
        private readonly IDbConnection _connection;

        public TransactionDAO(IDbConnection connection)
        {
            _connection = connection;
        }

        // Class level variables
        string query = "";
    }
}
