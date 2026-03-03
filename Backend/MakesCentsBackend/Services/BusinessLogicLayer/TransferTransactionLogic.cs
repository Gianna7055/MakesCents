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
    public class TransferTransactionLogic
    {
        // Class level variables
        private readonly TransferTransactionDAO _transferTransactionDAO;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="transferTransactionDAO"></param>
        public TransferTransactionLogic(TransferTransactionDAO transferTransactionDAO, IMapper mapper)
        {
            _transferTransactionDAO = transferTransactionDAO;
            _mapper = mapper;
        }

        public async Task<CreateTransferTransactionResponse> CreateTransferTransactionAsync(CreateTransferTransactionRequest transferTransaction)
        {
            // Declare and initialize
            CreateTransferTransactionResponse response;
            
            // Make sure the necessary information was sent
            if (transferTransaction.BudgetId == 0 || transferTransaction.UserId == 0 || transferTransaction.TransactionDate == DateOnly.MinValue || transferTransaction.TotalAmount == 0m || transferTransaction.TransferToId == 0 || transferTransaction.TransferFromId == 0 || transferTransaction.TransferTransactionType == TransferTransactionType.Unknown)
            {
                return new CreateTransferTransactionResponse(400, "Missing information for transfer transaction creation");
            }
            // Call the DAO method
            response = await _transferTransactionDAO.CreateTransferTransactionAsync(transferTransaction);
            // Return the response
            return response;
        }


        public async Task<GetTransferTransactionDTOResponse> GetTransferTransactionAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetTransferTransactionDTOResponse dtoResponse;
            GetTransferTransactionEntityResponse entityResponse;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetTransferTransactionDTOResponse(400, "Missing information to get transfer transaction");
            }
            // Call the DAO method
            entityResponse = await _transferTransactionDAO.GetTransferTransactionAsync(request);
            // Map the entity response to the dto response
            dtoResponse = _mapper.Map<GetTransferTransactionDTOResponse>(entityResponse);

            // Map the transfer to and from ids
            if (entityResponse.TransferTransaction.TransferTransactionType == TransferTransactionType.Envelope)
            {
                dtoResponse.TransferTransaction.TransferToId = entityResponse.TransferTransaction.TransferToEnvelopeId;
                dtoResponse.TransferTransaction.TransferFromId = entityResponse.TransferTransaction.TransferFromEnvelopeId;
            }
            if (entityResponse.TransferTransaction.TransferTransactionType == TransferTransactionType.Account)
            {
                dtoResponse.TransferTransaction.TransferToId = entityResponse.TransferTransaction.TransferToAccountId;
                dtoResponse.TransferTransaction.TransferFromId = entityResponse.TransferTransaction.TransferFromAccountId;
            }
            // Return the DTO
            return dtoResponse;
        }

        public async Task<BaseIdResponse> UpdateTransferTransactionAsync(UpdateTransferTransactionRequest transferTransaction)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (transferTransaction.TransactionId == 0 || transferTransaction.TransferTransactionId == 0 || transferTransaction.UserId == 0)
            {
                // Return that there is not enough information
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Call the update method in the DAO
            response = await _transferTransactionDAO.UpdateTransferTransactionAsync(transferTransaction);
            // Return the response
            return response;
        }
    }
}
