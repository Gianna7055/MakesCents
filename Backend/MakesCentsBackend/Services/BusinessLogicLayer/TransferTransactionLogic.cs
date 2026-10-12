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

        /// <summary>
        /// Logic method to create a new transfer transaction (account-to-account or envelope-to-envelope)
        /// </summary>
        /// <param name="transferTransaction"></param>
        /// <returns></returns>
        public async Task<CreateTransferTransactionResponse> CreateTransferTransactionAsync(CreateTransferTransactionRequest transferTransaction)
        {
            // Declare and initialize
            CreateTransferTransactionResponse response;

            // Make sure the necessary information was sent
            if (transferTransaction.BudgetMonthId == 0 || transferTransaction.UserId == 0 || transferTransaction.TransactionDate == DateOnly.MinValue || transferTransaction.TotalAmount == 0m || transferTransaction.TransferToId == 0 || transferTransaction.TransferFromId == 0 || transferTransaction.TransferTransactionType == TransferTransactionType.Unknown)
            {
                return new CreateTransferTransactionResponse(400, "Missing information for transfer transaction creation");
            }
            // For an envelope transfer, each side has to say which table it belongs to -
            // otherwise the DAO can't tell a sinking fund envelope from a rollover envelope
            if (transferTransaction.TransferTransactionType == TransferTransactionType.Envelope &&
                (!transferTransaction.TransferFromIsSinkingFund.HasValue || !transferTransaction.TransferToIsSinkingFund.HasValue))
            {
                return new CreateTransferTransactionResponse(400, "Must specify whether each envelope is a sinking fund or rollover envelope");
            }
            // Call the DAO method
            response = await _transferTransactionDAO.CreateTransferTransactionAsync(transferTransaction);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a transfer transaction
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

            // Map the transfer to and from ids, plus (for envelopes) which table each side belongs to
            if (entityResponse.TransferTransaction.TransferTransactionType == TransferTransactionType.Envelope)
            {
                dtoResponse.TransferTransaction.TransferToId = entityResponse.TransferTransaction.TransferToEnvelopeId;
                dtoResponse.TransferTransaction.TransferFromId = entityResponse.TransferTransaction.TransferFromEnvelopeId;
                dtoResponse.TransferTransaction.TransferFromIsSinkingFund = entityResponse.TransferTransaction.TransferFromIsSinkingFund;
                dtoResponse.TransferTransaction.TransferToIsSinkingFund = entityResponse.TransferTransaction.TransferToIsSinkingFund;
            }
            if (entityResponse.TransferTransaction.TransferTransactionType == TransferTransactionType.Account)
            {
                dtoResponse.TransferTransaction.TransferToId = entityResponse.TransferTransaction.TransferToAccountId;
                dtoResponse.TransferTransaction.TransferFromId = entityResponse.TransferTransaction.TransferFromAccountId;
            }
            // Return the DTO
            return dtoResponse;
        }

        /// <summary>
        /// Logic method to update a transfer transaction
        /// </summary>
        /// <param name="transferTransaction"></param>
        /// <returns></returns>
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
            // If either envelope side is being changed, both need to say which table they belong to
            if ((transferTransaction.TransferFromId.HasValue && !transferTransaction.TransferFromIsSinkingFund.HasValue) ||
                (transferTransaction.TransferToId.HasValue && !transferTransaction.TransferToIsSinkingFund.HasValue))
            {
                // Only actually required if this is (or is becoming) an envelope transfer -
                // the DAO ignores these flags for account transfers, so only warn when relevant
                if (transferTransaction.TransferTransactionType == TransferTransactionType.Envelope)
                {
                    return new BaseIdResponse(400, "Must specify whether each changed envelope is a sinking fund or rollover envelope");
                }
            }
            // Call the update method in the DAO
            response = await _transferTransactionDAO.UpdateTransferTransactionAsync(transferTransaction);
            // Return the response
            return response;
        }
    }
}