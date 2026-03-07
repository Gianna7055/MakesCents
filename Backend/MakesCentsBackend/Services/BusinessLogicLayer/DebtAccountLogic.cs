/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;
using MakesCentsBackend.Models.Enums;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class DebtAccountLogic
    {
        // Class level variables
        private readonly DebtAccountDAO _debtAccountDAO;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="debtAccountDAO"></param>
        public DebtAccountLogic(DebtAccountDAO debtAccountDAO, IMapper mapper)
        {
            _debtAccountDAO = debtAccountDAO;
            _mapper = mapper;
        }

        /// <summary>
        /// Logic method to create a new debt account
        /// </summary>
        /// <param name="debtAccount"></param>
        /// <returns></returns>
        public async Task<CreateDebtAccountResponse> CreateDebtAccountAsync(CreateDebtAccountRequest debtAccount)
        {
            // Declare and initialize
            CreateDebtAccountResponse response;

            // Make sure the necessary information was sent
            if (debtAccount.BudgetId == 0 || debtAccount.UserId == 0 || string.IsNullOrEmpty(debtAccount.AccountName) || string.IsNullOrEmpty(debtAccount.Institution) || debtAccount.Balance == 0m || debtAccount.DebtAccountType == DebtAccountType.Unknown)
            {
                return new CreateDebtAccountResponse(400, "Missing information for debt account creation");
            }
            // Call the DAO method
            response = await _debtAccountDAO.CreateDebtAccountAsync(debtAccount);
            // Return the response
            return response;
        }


        public async Task<GetDebtAccountDTOResponse> GetDebtAccountAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetDebtAccountDTOResponse dtoResponse;
            GetDebtAccountEntityResponse entityResponse;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetDebtAccountDTOResponse(400, "Missing information to get debt account");
            }
            // Call the DAO method
            entityResponse = await _debtAccountDAO.GetDebtAccountAsync(request);
            // Map the entity response to the dto response
            dtoResponse = _mapper.Map<GetDebtAccountDTOResponse>(entityResponse);
            // Map each entity transaction to a dto transaction
            foreach (SummaryTransactionEntityModel entityTransaction in entityResponse.DebtAccount.Transactions)
            {
                dtoResponse.DebtAccount.Transactions.Add(_mapper.Map<SummaryTransactionDTOModel>(entityTransaction));
            }
            // Return the DTO
            return dtoResponse;
        }


        public async Task<BaseIdResponse> UpdateDebtAccountAsync(UpdateDebtAccountRequest debtAccount)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (debtAccount.DebtAccountId == 0 || debtAccount.UserId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Call the update method in the DAO
            response = await _debtAccountDAO.UpdateDebtAccountAsync(debtAccount);
            // Return the response
            return response;
        }
    }
}
