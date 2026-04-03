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
        /// Logic method to get all envelope categories
        /// </summary>
        /// <param name="envelopeCategoryId"></param>
        /// <returns></returns>
        public async Task<GetAllEnvelopeCategoriesResponse> GetAllEnvelopeCategoriesAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllEnvelopeCategoriesResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetAllEnvelopeCategoriesResponse(400, "Missing information to get all envelope categories");
            }
            // Call the DAO method
            response = await _envelopeCategoryDAO.GetAllEnvelopeCategoriesAsync(request);
            // Return the response
            return response;
        }


        public async Task<GetEnvelopeCategoryResponse> GetEnvelopeCategoryAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetEnvelopeCategoryResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetEnvelopeCategoryResponse(400, "Missing information to get envelope category");
            }
            // Call the DAO method
            response = await _envelopeCategoryDAO.GetEnvelopeCategoryAsync(request);
            // Return the reponse
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
            if (envelopeCategory.EnvelopeCategoryId == 0 || envelopeCategory.UserId == 0)
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
        public async Task<BaseResponse> DeleteEnvelopeCategoryAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for update");
            }
            // Return a call the the DAO method
            return await _envelopeCategoryDAO.DeleteEnvelopeCategoryAsync(request);
        }
    }
}
