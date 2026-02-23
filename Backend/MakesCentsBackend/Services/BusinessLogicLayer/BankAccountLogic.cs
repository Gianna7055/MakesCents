/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class BankAccountLogic
    {
        // Class level variables
        private readonly BankAccountDAO _bankAccountDAO;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="bankAccountDAO"></param>
        public BankAccountLogic(BankAccountDAO bankAccountDAO, IMapper mapper)
        {
            _bankAccountDAO = bankAccountDAO;
            _mapper = mapper;
        }

        /// <summary>
        /// Logic method to create a new bank account
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        public async Task<CreateBankAccountResponse> CreateBankAccountAsync(CreateBankAccountRequest bankAccount)
        {
            // Declare and initialize
            CreateBankAccountResponse response;

            // Make sure the necessary information was sent
            if (bankAccount.BudgetId == null || bankAccount.UserId == null || string.IsNullOrEmpty(bankAccount.AccountName) || string.IsNullOrEmpty(bankAccount.Institution) || bankAccount.Balance == null || bankAccount.BankAccountType == Models.Enums.BankAccountType.Unknown)
            {
                return new CreateBankAccountResponse(400, "Missing information for bank account creation");
            }
            // Call the DAO method
            response = await _bankAccountDAO.CreateBankAccountAsync(bankAccount);
            // Return the response
            return response;
        }


        public async Task<GetBankAccountDTOResponse> GetBankAccountAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetBankAccountDTOResponse dtoResponse;
            GetBankAccountEntityResponse entityResponse;

            // Call the DAO method
            entityResponse = await _bankAccountDAO.GetBankAccountAsync(request);
            // Map the entity response to the dto response
            dtoResponse = _mapper.Map<GetBankAccountDTOResponse>(entityResponse);
            // Map each entity transaction to a dto transaction
            foreach (SummaryTransactionEntityModel entityTransaction in entityResponse.BankAccount.Transactions)
            {
                dtoResponse.BankAccount.Transactions.Add(_mapper.Map<SummaryTransactionDTOModel>(entityTransaction));
            }
            // Return the DTO
            return dtoResponse;
        }
    }
}
