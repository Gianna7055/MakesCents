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
    public class RolloverEnvelopeLogic
    {
        // Class level variables
        private readonly RolloverEnvelopeDAO _rolloverEnvelopeDAO;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="rolloverEnvelopeDAO"></param>
        /// <param name="mapper"></param>
        public RolloverEnvelopeLogic(RolloverEnvelopeDAO rolloverEnvelopeDAO, IMapper mapper)
        {
            _rolloverEnvelopeDAO = rolloverEnvelopeDAO;
            _mapper = mapper;
        }

        /// <summary>
        /// Logic method to create a new rollover envelope's persistent definition
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateRolloverEnvelopeAsync(CreateRolloverEnvelopeRequest envelope)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (envelope.EnvelopeCategoryId == 0 || string.IsNullOrWhiteSpace(envelope.EnvelopeName) || envelope.TransferEnvelopeId == 0)
            {
                return new BaseIdResponse(400, "Missing information for envelope creation");
            }
            // Call the DAO method
            response = await _rolloverEnvelopeDAO.CreateRolloverEnvelopeAsync(envelope);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a rollover envelope for a specific budget month, including its
        /// transaction history for that month
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetRolloverEnvelopeDTOResponse> GetRolloverEnvelopeForMonthAsync(GetRolloverEnvelopeMonthRequest request)
        {
            // Declare and initialize
            GetRolloverEnvelopeDTOResponse dtoResponse;
            GetRolloverEnvelopeEntityResponse entityResponse;

            // Make sure the required information was sent
            if (request.RolloverEnvelopeId == 0 || request.BudgetMonthId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetRolloverEnvelopeDTOResponse(400, "Missing information to get envelope");
            }
            // Call the DAO method
            entityResponse = await _rolloverEnvelopeDAO.GetRolloverEnvelopeForMonthAsync(request);
            // Map the entity response to the dto response
            dtoResponse = _mapper.Map<GetRolloverEnvelopeDTOResponse>(entityResponse);
            // Map each entity transaction to a dto transaction
            foreach (SummaryTransactionEntityModel entityTransaction in entityResponse.RolloverEnvelope.Transactions)
            {
                dtoResponse.RolloverEnvelopeDTO.Transactions.Add(_mapper.Map<SummaryTransactionDTOModel>(entityTransaction));
            }
            // Return the DTO
            return dtoResponse;
        }

        /// <summary>
        /// Logic method to update a rollover envelope's persistent definition fields
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateRolloverEnvelopeAsync(UpdateRolloverEnvelopeRequest envelope)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (envelope.RolloverEnvelopeId == 0 || envelope.UserId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // A transfer envelope can be changed but not cleared
            if (envelope.TransferEnvelopeId == 0)
            {
                return new BaseIdResponse(400, "Transfer envelope cannot be 0");
            }
            // Call the DAO method
            response = await _rolloverEnvelopeDAO.UpdateRolloverEnvelopeAsync(envelope);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete a rollover envelope's persistent definition (cascades every
        /// month it was ever activated for)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteRolloverEnvelopeAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for delete");
            }
            // Return a call to the DAO method
            return await _rolloverEnvelopeDAO.DeleteRolloverEnvelopeAsync(request);
        }
    }
}