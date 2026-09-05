/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;
using MakesCentsBackend.Services.Mappers;

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


        public async Task<GetAllTransactionsDTOResponse> GetAllTransactionsAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllTransactionsDTOResponse dtoResponse;
            GetAllTransactionsEntityResponse entityResponse;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetAllTransactionsDTOResponse(400, "Missing information to get all transactions");
            }
            // Call the DAO method
            entityResponse = await _transactionDAO.GetAllTransactionsAsync(request);
            // Map the entity response the dto response
            dtoResponse = _mapper.Map<GetAllTransactionsDTOResponse>(entityResponse);
            // Return the DTO
            return dtoResponse;
        }

        /// <summary>
        /// Logic method to soft delete a transaction
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> SoftDeleteTransactionAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for soft delete");
            }
            // Return a call the the DAO method
            return await _transactionDAO.SoftDeleteTransactionAsync(request);
        }

        /// <summary>
        /// Logic method to restore a transaction
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> RestoreTransactionAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for restore");
            }
            // Return a call the the DAO method
            return await _transactionDAO.RestoreTransactionAsync(request);
        }

        /// <summary>
        /// Logic method to hard delete a transaction
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> HardDeleteTransactionAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for hard delete");
            }
            // Return a call the the DAO method
            return await _transactionDAO.HardDeleteTransactionAsync(request);
        }
    }
}
