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


        public async Task<GetAllAccountsResponse> GetAllAccountsAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllAccountsResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetAllAccountsResponse(400, "Missing information to get all accounts");
            }
            // Call the DAO method
            response = await _accountDAO.GetAllAccountsAsync(request);

            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete an account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteAccountAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for update");
            }
            // Return a call the the DAO method
            return await _accountDAO.DeleteAccountAsync(request);
        }
    }
}
