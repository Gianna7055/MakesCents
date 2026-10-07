/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class EnvelopeFundingLogic
    {
        // Class level variables
        private readonly EnvelopeFundingDAO _envelopeFundingDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="envelopeFundingDAO"></param>
        public EnvelopeFundingLogic(EnvelopeFundingDAO envelopeFundingDAO)
        {
            _envelopeFundingDAO = envelopeFundingDAO;
        }

        /// <summary>
        /// Logic method to set the envelope funding queue for a budget month
        /// </summary>
        /// <param name="fundingQueue"></param>
        /// <returns></returns>
        public async Task<GetEnvelopeFundingQueueResponse> SetEnvelopeFundingQueueAsync(SetEnvelopeFundingQueueRequest fundingQueue)
        {
            // Declare and initialize
            GetEnvelopeFundingQueueResponse response;

            // Make sure the necessary information was sent
            if (fundingQueue.BudgetMonthId == 0 || fundingQueue.UserId == 0)
            {
                return new GetEnvelopeFundingQueueResponse(400, "Missing information to set the funding queue");
            }
            // Loop through the items to make sure the necessary information was sent for each
            foreach (SetEnvelopeFundingItemRequest item in fundingQueue.FundingQueue)
            {
                if (item == null)
                    return new GetEnvelopeFundingQueueResponse(400, "Funding item cannot be null");

                if (item.EnvelopeId == 0)
                    return new GetEnvelopeFundingQueueResponse(400, "Funding item envelope is required");

                if (item.PlannedAmount <= 0)
                    return new GetEnvelopeFundingQueueResponse(400, "Funding item planned amount must be greater than 0");
            }
            // Call the DAO method
            response = await _envelopeFundingDAO.SetEnvelopeFundingQueueAsync(fundingQueue);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get the envelope funding queue for a budget month
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetEnvelopeFundingQueueResponse> GetEnvelopeFundingQueueAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetEnvelopeFundingQueueResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetEnvelopeFundingQueueResponse(400, "Missing information to get the funding queue");
            }
            // Call the DAO method
            response = await _envelopeFundingDAO.GetEnvelopeFundingQueueAsync(request);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete a single envelope funding row
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteEnvelopeFundingAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for delete");
            }
            // Return a call to the DAO method
            return await _envelopeFundingDAO.DeleteEnvelopeFundingAsync(request);
        }
    }
}