/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class BankAccountLogic
    {
        // Class level variables
        private readonly BankAccountDAO _bankAccountDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="bankAccountDAO"></param>
        public BankAccountLogic(BankAccountDAO bankAccountDAO)
        {
            _bankAccountDAO = bankAccountDAO;
        }

        /// <summary>
        /// Logic method to create a new bank account
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBankAccountAsync(CreateBankAccountRequest bankAccount)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (bankAccount.BudgetId == null || bankAccount.UserId == null || string.IsNullOrEmpty(bankAccount.AccountName) || string.IsNullOrEmpty(bankAccount.Institution) || bankAccount.Balance == null || bankAccount.BankAccountType == Models.Enums.BankAccountType.Unknown)
            {
                return new BaseIdResponse(400, "Missing information for bank account creation");
            }
            // Call the DAO method
            response = await _bankAccountDAO.CreateBankAccountAsync(bankAccount);
            // Return the response
            return response;
        }
    }
}
