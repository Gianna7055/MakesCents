/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class AccountLogic
    {
        // Class level variables
        private readonly AccountDAO _accountDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="accountDAO"></param>
        public AccountLogic(AccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }


        public async Task<GetAllAccountsResponse> GetAllAccountsAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetAllAccountsResponse response;

            // Call the DAO method
            response = await _accountDAO.GetAllAccountsAsync(request);

            // Return the response
            return response;
        }
    }
}
