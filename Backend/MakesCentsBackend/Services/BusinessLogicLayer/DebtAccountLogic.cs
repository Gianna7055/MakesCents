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
            if (debtAccount.BudgetId == null || debtAccount.UserId == null || string.IsNullOrEmpty(debtAccount.AccountName) || string.IsNullOrEmpty(debtAccount.Institution) || debtAccount.Balance == null || debtAccount.DebtAccountType == DebtAccountType.Unknown)
            {
                return new CreateDebtAccountResponse(400, "Missing information for debt account creation");
            }
            // Call the DAO method
            response = await _debtAccountDAO.CreateDebtAccountAsync(debtAccount);
            // Return the response
            return response;
        }


        public async Task<GetDebtAccountDTOResponse> GetDebtAccountAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetDebtAccountDTOResponse dtoResponse;
            GetDebtAccountEntityResponse entityResponse;

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
    }
}
