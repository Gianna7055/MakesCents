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
    public class SinkingFundEnvelopeLogic
    {
        // Class level variables
        private readonly SinkingFundEnvelopeDAO _sinkingFundEnvelopeDAO;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="sinkingFundEnvelopeDAO"></param>
        /// <param name="mapper"></param>
        public SinkingFundEnvelopeLogic(SinkingFundEnvelopeDAO sinkingFundEnvelopeDAO, IMapper mapper)
        {
            _sinkingFundEnvelopeDAO = sinkingFundEnvelopeDAO;
            _mapper = mapper;
        }

        /// <summary>
        /// Logic method to create a new sinking fund envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateSinkingFundEnvelopeAsync(CreateSinkingFundEnvelopeRequest envelope)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (envelope.EnvelopeCategoryId == 0 || string.IsNullOrWhiteSpace(envelope.EnvelopeName))
            {
                return new BaseIdResponse(400, "Missing information for envelope creation");
            }
            // Call the DAO method
            response = await _sinkingFundEnvelopeDAO.CreateSinkingFundEnvelopeAsync(envelope);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a specific sinking fund envelope, including its transaction history
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetSinkingFundEnvelopeDTOResponse> GetSinkingFundEnvelopeAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetSinkingFundEnvelopeDTOResponse dtoResponse;
            GetSinkingFundEnvelopeEntityResponse entityResponse;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetSinkingFundEnvelopeDTOResponse(400, "Missing information to get envelope");
            }
            // Call the DAO method
            entityResponse = await _sinkingFundEnvelopeDAO.GetSinkingFundEnvelopeAsync(request);
            // Map the entity response to the dto response
            dtoResponse = _mapper.Map<GetSinkingFundEnvelopeDTOResponse>(entityResponse);
            // Map each entity transaction to a dto transaction
            foreach (SummaryTransactionEntityModel entityTransaction in entityResponse.SinkingFundEnvelope.Transactions)
            {
                dtoResponse.SinkingFundEnvelopeDTO.Transactions.Add(_mapper.Map<SummaryTransactionDTOModel>(entityTransaction));
            }
            // Return the DTO
            return dtoResponse;
        }

        /// <summary>
        /// Logic method to update a sinking fund envelope based on provided fields
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateSinkingFundEnvelopeAsync(UpdateSinkingFundEnvelopeRequest envelope)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (envelope.SinkingFundEnvelopeId == 0 || envelope.UserId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Call the DAO method
            response = await _sinkingFundEnvelopeDAO.UpdateSinkingFundEnvelopeAsync(envelope);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get the rollover envelopes that use a sinking fund envelope as their transfer target
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetSinkingFundDependentsResponse> GetRolloverDependentsAsync(BaseIdRequest request)
        {
            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new GetSinkingFundDependentsResponse(400, "Missing information to get dependent envelopes");
            }
            // Call the DAO method
            return await _sinkingFundEnvelopeDAO.GetRolloverDependentsAsync(request);
        }

        /// <summary>
        /// Logic method to delete a sinking fund envelope
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteSinkingFundEnvelopeAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for delete");
            }
            // Return a call to the DAO method
            return await _sinkingFundEnvelopeDAO.DeleteSinkingFundEnvelopeAsync(request);
        }
    }
}