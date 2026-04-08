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
    public class EnvelopeLogic
    {
        // Class level variables
        private readonly EnvelopeDAO _envelopeDAO;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="envelopeDAO"></param>
        public EnvelopeLogic(EnvelopeDAO envelopeDAO, IMapper mapper)
        {
            _envelopeDAO = envelopeDAO;
            _mapper = mapper;
        }

        /// <summary>
        /// Logic method to create a new envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateEnvelopeAsync(CreateEnvelopeRequest envelope)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (envelope.EnvelopeCategoryId == 0 ||
                string.IsNullOrWhiteSpace(envelope.EnvelopeName)/* ||
                envelope.PlannedAmount == 0m ||
                envelope.RemainingAmount == 0m*/)
            {
                return new BaseIdResponse(400, "Missing information for envelope creation");
            }
            // If the envelope is not a sinking fun, it must have a transfer envelope id
            if (envelope.IsSinkingFund == false &&
                envelope.TransferEnvelopeId.Value == null)
            {
                return new BaseIdResponse(400, "Rollover funds require a transfer envelope");
            }

            // Normalize the input
            if (envelope.IsSinkingFund == true)
            {
                envelope.TransferEnvelopeId = null;
            }
            else
            {
                envelope.GoalAmount = null;
                envelope.GoalEndDate = null;
            }

            // Call the DAO method
            response = await _envelopeDAO.CreateEnvelopeAsync(envelope);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a specific envelope
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetEnvelopeDTOResponse> GetEnvelopeAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetEnvelopeDTOResponse dtoResponse;
            GetEnvelopeEntityResponse entityResponse;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetEnvelopeDTOResponse(400, "Missing information to get envelope");
            }
            // Call the DAO method
            entityResponse = await _envelopeDAO.GetEnvelopeAsync(request);
            // Map the entity response the dto response
            dtoResponse = _mapper.Map<GetEnvelopeDTOResponse>(entityResponse);
            // Return the DTO
            return dtoResponse;
        }


        public async Task<BaseIdResponse> UpdateEnvelopeAsync(UpdateEnvelopeRequest requestEnvelope)
        {
            // Declare and initialize
            GetEnvelopeEntityResponse entityResponse;
            GetEnvelopeEntityModel entityModel;
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (requestEnvelope.EnvelopeCategoryId == 0 || requestEnvelope.UserId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }

            // Get the entity response for updating the envelope
            entityResponse = await _envelopeDAO.GetEnvelopeForUpdateAsync(new BaseIdRequest(requestEnvelope.UserId, requestEnvelope.EnvelopeId));
            // If the result it not a 200, return the status and message
            if (entityResponse.HttpStatus != 200)
            {
                return new BaseIdResponse(entityResponse.HttpStatus, entityResponse.Message, requestEnvelope.EnvelopeId);
            }
            // Get the model
            entityModel = entityResponse.EnvelopeEntity;

            // Set the values to update in the entity response
            entityModel.EnvelopeCategoryId = requestEnvelope.EnvelopeCategoryId ?? entityModel.EnvelopeCategoryId;
            entityModel.EnvelopeName = requestEnvelope.EnvelopeName ?? entityModel.EnvelopeName;
            entityModel.PlannedAmount = requestEnvelope.PlannedAmount ?? entityModel.PlannedAmount;
            entityModel.IsSinkingFund = requestEnvelope.IsSinkingFund ?? entityModel.IsSinkingFund;

            // Optional<T> updates
            if (requestEnvelope.GoalAmount.HasValue)
            {
                entityModel.GoalAmount = requestEnvelope.GoalAmount.Value;
            }
            if (requestEnvelope.GoalEndDate.HasValue)
            {
                entityModel.GoalEndDate = requestEnvelope.GoalEndDate.Value;
            }
            if (requestEnvelope.TransferEnvelopeId.HasValue)
            {
                entityModel.TransferEnvelopeId = requestEnvelope.TransferEnvelopeId.Value;
            }
            // If the envelope is not a sinking fun, it must have a transfer envelope id
            if (entityModel.IsSinkingFund == false && entityModel.TransferEnvelopeId == null)
            {
                return new BaseIdResponse(400, "Rollover funds require a transfer envelope");
            }

            // Normalize the input
            if (entityModel.IsSinkingFund == true)
            {
                requestEnvelope.TransferEnvelopeId = null;
            }
            else
            {
                requestEnvelope.GoalAmount = null;
                requestEnvelope.GoalEndDate = null;
            }
            // Call the DAO method
            response = await _envelopeDAO.UpdateEnvelopeAsync(requestEnvelope);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete an envelope
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteEnvelopeAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for update");
            }
            // Return a call the the DAO method
            return await _envelopeDAO.DeleteEnvelopeAsync(request);
        }
    }
}
