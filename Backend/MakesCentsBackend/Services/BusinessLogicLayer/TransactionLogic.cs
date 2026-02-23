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
    public class TransactionLogic
    {
        // Class level variables
        private readonly TransactionDAO _transactionDAO;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="transactionDAO"></param>
        public TransactionLogic(TransactionDAO transactionDAO, IMapper mapper)
        {
            _transactionDAO = transactionDAO;
            _mapper = mapper;
        }


        public async Task<GetAllTransactionsDTOResponse> GetAllTransactionsAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetAllTransactionsDTOResponse dtoResponse;
            GetAllTransactionsEntityResponse entityResponse;

            // Call the DAO method
            entityResponse = await _transactionDAO.GetAllTransactionsAsync(request);
            // Map the entity response the dto response
            dtoResponse = _mapper.Map<GetAllTransactionsDTOResponse>(entityResponse);
            // Map each entity transaction to a dto transaction
            foreach (SummaryTransactionEntityModel entityTransaction in entityResponse.Transactions)
            {
                dtoResponse.Transactions.Add(_mapper.Map<SummaryTransactionDTOModel>(entityTransaction));
            }
            // Return the DTO
            return dtoResponse;
        }
    }
}
