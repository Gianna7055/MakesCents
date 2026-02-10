/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Models.Enums;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class DebtAccountLogic
    {
        // Class level variables
        private readonly DebtAccountDAO _debtAccountDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="debtAccountDAO"></param>
        public DebtAccountLogic(DebtAccountDAO debtAccountDAO)
        {
            _debtAccountDAO = debtAccountDAO;
        }

        /// <summary>
        /// Logic method to create a new debt account
        /// </summary>
        /// <param name="debtAccount"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateDebtAccountAsync(CreateDebtAccountRequest debtAccount)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (debtAccount.BudgetId == null || debtAccount.UserId == null || string.IsNullOrEmpty(debtAccount.AccountName) || string.IsNullOrEmpty(debtAccount.Institution) || debtAccount.Balance == null || debtAccount.DebtAccountType == DebtAccountType.Unknown)
            {
                return new BaseIdResponse(400, "Missing information for debt account creation");
            }
            // Call the DAO method
            response = await _debtAccountDAO.CreateDebtAccountAsync(debtAccount);
            // Return the response
            return response;
        }
    }
}
