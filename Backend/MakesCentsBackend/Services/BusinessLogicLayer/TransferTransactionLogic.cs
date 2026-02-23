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
            if (transferTransaction.BudgetId == null || transferTransaction.UserId == null || transferTransaction.TransactionDate == null || transferTransaction.TotalAmount == null || transferTransaction.TransferToId == null || transferTransaction.TransferFromId == null || transferTransaction.TransferTransactionType == TransferTransactionType.Unknown)
            {
                return new CreateTransferTransactionResponse(400, "Missing information for transfer transaction creation");
            }
            // Call the DAO method
            response = await _transferTransactionDAO.CreateTransferTransactionAsync(transferTransaction);
            // Return the response
            return response;
        }


        public async Task<GetTransferTransactionDTOResponse> GetTransferTransactionAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetTransferTransactionDTOResponse dtoResponse;
            GetTransferTransactionEntityResponse entityResponse;

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
    }
}
