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
    public class InvestmentAccountLogic
    {
        // Class level variables
        private readonly InvestmentAccountDAO _investmentAccountDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="investmentAccountDAO"></param>
        public InvestmentAccountLogic(InvestmentAccountDAO investmentAccountDAO)
        {
            _investmentAccountDAO = investmentAccountDAO;
        }

        /// <summary>
        /// Logic method to create a new investment account
        /// </summary>
        /// <param name="investmentAccount"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateInvestmentAccountAsync(CreateInvestmentAccountRequest investmentAccount)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (investmentAccount.BudgetId == null || investmentAccount.UserId == null || string.IsNullOrEmpty(investmentAccount.AccountName) || string.IsNullOrEmpty(investmentAccount.Institution) || investmentAccount.Balance == null || investmentAccount.InvestmentAccountType == InvestmentAccountType.Unknown || investmentAccount.IsTaxDeferred == null || investmentAccount.IsTaxExempt == null)
            {
                return new BaseIdResponse(400, "Missing information for investment account creation");
            }
            // Call the DAO method
            response = await _investmentAccountDAO.CreateInvestmentAccountAsync(investmentAccount);
            // Return the response
            return response;
        }
    }
}
