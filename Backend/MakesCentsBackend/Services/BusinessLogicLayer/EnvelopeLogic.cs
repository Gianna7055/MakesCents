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
            if (envelope.EnvelopeCategoryId == null ||
                string.IsNullOrWhiteSpace(envelope.EnvelopeName) ||
                envelope.PlannedAmount == null ||
                envelope.RemainingAmount == null || envelope.IsSinkingFund == null)
            {
                return new BaseIdResponse(400, "Missing information for envelope creation");
            }
            // If the envelope is a sinking fund, it must have a goal amount and end date
            if (envelope.IsSinkingFund == true &&
                (envelope.GoalAmount.Value == null || envelope.GoalEndDate.Value == null))
            {
                return new BaseIdResponse(400, "Sinking funds require a goal amount and goal end date");
            }
            // If the envelope is not a sinking fun, it must have a transfer envelope id
            if (!envelope.IsSinkingFund == false &&
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
        public async Task<GetEnvelopeDTOResponse> GetEnvelopeAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetEnvelopeDTOResponse dtoResponse;
            GetEnvelopeEntityResponse entityResponse;

            // Call the DAO method
            entityResponse = await _envelopeDAO.GetEnvelopeAsync(request);
            // Map the entity response the dto response
            dtoResponse = _mapper.Map<GetEnvelopeDTOResponse>(entityResponse);
            // Map each entity transaction to a dto transaction
            foreach (SummaryTransactionEntityModel entityTransaction in entityResponse.EnvelopeEntity.Transactions)
            {
                dtoResponse.EnvelopeDTO.Transactions.Add(_mapper.Map<SummaryTransactionDTOModel>(entityTransaction));
            }
            // Return the DTO
            return dtoResponse;
        }
    }
}
