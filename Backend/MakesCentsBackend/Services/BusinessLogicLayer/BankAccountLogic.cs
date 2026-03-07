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
            if (bankAccount.BudgetId == 0 || bankAccount.UserId == 0 || string.IsNullOrEmpty(bankAccount.AccountName) || string.IsNullOrEmpty(bankAccount.Institution) || bankAccount.Balance == 0 || bankAccount.BankAccountType == Models.Enums.BankAccountType.Unknown)
            {
                return new CreateBankAccountResponse(400, "Missing information for bank account creation");
            }
            // Call the DAO method
            response = await _bankAccountDAO.CreateBankAccountAsync(bankAccount);
            // Return the response
            return response;
        }


        public async Task<GetBankAccountDTOResponse> GetBankAccountAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetBankAccountDTOResponse dtoResponse;
            GetBankAccountEntityResponse entityResponse;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetBankAccountDTOResponse(400, "Missing information to get bank account");
            }
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


        public async Task<BaseIdResponse> UpdateBankAccountAsync(UpdateBankAccountRequest bankAccount)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (bankAccount.BankAccountId == 0 || bankAccount.UserId == 0 || bankAccount.AccountId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Call the update method in the DAO
            response = await _bankAccountDAO.UpdateBankAccountAsync(bankAccount);
            // Return the response
            return response;
        }
    }
}
