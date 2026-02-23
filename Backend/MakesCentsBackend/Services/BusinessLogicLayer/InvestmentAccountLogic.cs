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
    public class InvestmentAccountLogic
    {
        // Class level variables
        private readonly InvestmentAccountDAO _investmentAccountDAO;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="investmentAccountDAO"></param>
        public InvestmentAccountLogic(InvestmentAccountDAO investmentAccountDAO, IMapper mapper)
        {
            _investmentAccountDAO = investmentAccountDAO;
            _mapper = mapper;
        }

        /// <summary>
        /// Logic method to create a new investment account
        /// </summary>
        /// <param name="investmentAccount"></param>
        /// <returns></returns>
        public async Task<CreateInvestmentAccountResponse> CreateInvestmentAccountAsync(CreateInvestmentAccountRequest investmentAccount)
        {
            // Declare and initialize
            CreateInvestmentAccountResponse response;

            // Make sure the necessary information was sent
            if (investmentAccount.BudgetId == null || investmentAccount.UserId == null || string.IsNullOrEmpty(investmentAccount.AccountName) || string.IsNullOrEmpty(investmentAccount.Institution) || investmentAccount.Balance == null || investmentAccount.InvestmentAccountType == InvestmentAccountType.Unknown || investmentAccount.IsTaxDeferred == null || investmentAccount.IsTaxExempt == null)
            {
                return new CreateInvestmentAccountResponse(400, "Missing information for investment account creation");
            }
            // Call the DAO method
            response = await _investmentAccountDAO.CreateInvestmentAccountAsync(investmentAccount);
            // Return the response
            return response;
        }


        public async Task<GetInvestmentAccountDTOResponse> GetInvestmentAccountAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetInvestmentAccountDTOResponse dtoResponse;
            GetInvestmentAccountEntityResponse entityResponse;

            // Call the DAO method
            entityResponse = await _investmentAccountDAO.GetInvestmentAccountAsync(request);
            // Map the entity response to the dto response
            dtoResponse = _mapper.Map<GetInvestmentAccountDTOResponse>(entityResponse);
            // Map each entity transaction to a dto transaction
            foreach (SummaryTransactionEntityModel entityTransaction in entityResponse.InvestmentAccount.Transactions)
            {
                dtoResponse.InvestmentAccount.Transactions.Add(_mapper.Map<SummaryTransactionDTOModel>(entityTransaction));
            }
            // Return the DTO
            return dtoResponse;
        }
    }
}
