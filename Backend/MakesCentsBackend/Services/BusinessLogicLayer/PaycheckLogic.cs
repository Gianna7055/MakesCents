/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Models.Enums;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class PaycheckLogic
    {
        // Class level variables
        private readonly PaycheckDAO _paycheckDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="paycheckDAO"></param>
        public PaycheckLogic(PaycheckDAO paycheckDAO)
        {
            _paycheckDAO = paycheckDAO;
        }

        /// <summary>
        /// Logic method to create a paycheck
        /// </summary>
        /// <param name="paycheck"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreatePaycheckAsync(CreatePaycheckRequest paycheck)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (paycheck.BudgetId == 0 || paycheck.UserId == 0 || string.IsNullOrEmpty(paycheck.PaycheckName) || paycheck.StartingDate == DateOnly.MinValue || paycheck.TotalAmount == 0m || paycheck.PaycheckRegularity == PaycheckRegularity.Unknown)
            {
                return new BaseIdResponse(400, "Missing information for paycheck creation");
            }
            // Call the DAO method
            response = await _paycheckDAO.CreatePaycheckAsync(paycheck);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get every paycheck for a budget
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetAllPaychecksResponse> GetAllPaychecksAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllPaychecksResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetAllPaychecksResponse(400, "Missing information to get all paychecks");
            }
            // Call the DAO method
            response = await _paycheckDAO.GetAllPaychecksAsync(request);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a single paycheck
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetPaycheckResponse> GetPaycheckAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetPaycheckResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetPaycheckResponse(400, "Missing information to get paycheck");
            }
            // Call the DAO method
            response = await _paycheckDAO.GetPaycheckAsync(request);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to update a paycheck
        /// </summary>
        /// <param name="paycheck"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdatePaycheckAsync(UpdatePaycheckRequest paycheck)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (paycheck.PaycheckId == 0 || paycheck.UserId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Call the DAO method
            response = await _paycheckDAO.UpdatePaycheckAsync(paycheck);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete a paycheck
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeletePaycheckAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for delete");
            }
            // Return a call to the DAO method
            return await _paycheckDAO.DeletePaycheckAsync(request);
        }
    }
}