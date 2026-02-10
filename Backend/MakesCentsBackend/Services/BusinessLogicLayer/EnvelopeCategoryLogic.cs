/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class EnvelopeCategoryLogic
    {
        // Class level variables
        private readonly EnvelopeCategoryDAO _envelopeCategoryDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="envelopeCategoryDAO"></param>
        public EnvelopeCategoryLogic(EnvelopeCategoryDAO envelopeCategoryDAO)
        {
            _envelopeCategoryDAO = envelopeCategoryDAO;
        }

        /// <summary>
        /// Logic method to create an envelope category
        /// </summary>
        /// <param name="envelopeCategory"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateEnvelopeCategoryAsync(CreateEnvelopeCategoryRequest envelopeCategory)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (string.IsNullOrEmpty(envelopeCategory.EnvelopeCategoryName))
            {
                return new BaseIdResponse(400, "Missing information for envelope category creation");
            }
            // Call the DAO method
            response = await _envelopeCategoryDAO.CreateEnvelopeCategoryAsync(envelopeCategory);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get an envelope category
        /// </summary>
        /// <param name="envelopeCategoryId"></param>
        /// <returns></returns>
        public async Task<GetAllEnvelopeCategoriesResponse> GetAllEnvelopeCategoriesAsync(int envelopeCategoryId, int userId)
        {
            // Declare and initialize
            GetAllEnvelopeCategoriesResponse response;

            // Call the DAO method
            response = await _envelopeCategoryDAO.GetAllEnvelopeCategoriesAsync(envelopeCategoryId, userId);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to update an envelope category based on provided fields
        /// </summary>
        /// <param name="envelopeCategory"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateEnvelopeCategoryAsync(EditEnvelopeCategoryRequest envelopeCategory)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (envelopeCategory.EnvelopeCategoryId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Call the Create User method in the DAO
            response = await _envelopeCategoryDAO.UpdateEnvelopeCategoryAsync(envelopeCategory);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete an envelope category
        /// </summary>
        /// <param name="envelopeCategoryId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteEnvelopeCategoryAsync(int envelopeCategoryId, int userId)
        {
            // Return a call the the DAO method
            return await _envelopeCategoryDAO.DeleteEnvelopeCategoryAsync(envelopeCategoryId, userId);
        }
    }
}
