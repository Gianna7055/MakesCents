/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Models.Enums;
using MakesCentsBackend.Services.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

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
        /// Logic method to create a new paycheck
        /// </summary>
        /// <param name="paycheck"></param>
        /// <returns></returns>
        public async Task<CreatePaycheckResponse> CreatePaycheckAsync(CreatePaycheckRequest paycheck)
        {
            // Declare and initialize
            CreatePaycheckResponse response;
            decimal? sumOfSplits;

            // Make sure the necessary information was sent
            if (paycheck.BudgetId == 0 || paycheck.UserId == 0 || string.IsNullOrEmpty(paycheck.PaycheckName) || paycheck.StartingDate == DateOnly.MinValue || paycheck.TotalAmount == 0m || paycheck.PaycheckRegularity == PaycheckRegularity.Unknown)
            {
                return new CreatePaycheckResponse(400, "Missing information for paycheck creation");
            }
            if (paycheck.PaycheckSplits == null || paycheck.PaycheckSplits.Count == 0)
            {
                return new CreatePaycheckResponse(400, "Paycheck must contain at least one split");
            }
            // Loop through the splits to make sure the necessary information was sent
            foreach (CreatePaycheckSplitRequest split in paycheck.PaycheckSplits)
            {
                if (split == null)
                    return new CreatePaycheckResponse(400, "Split cannot be null");

                if (split.Amount <= 0)
                    return new CreatePaycheckResponse(400, "Split amount must be greater than 0");

                if (split.EnvelopeId == 0)
                    return new CreatePaycheckResponse(400, "Split envelope is required");
            }
            // Total the splits
            sumOfSplits = paycheck.PaycheckSplits.Sum(s => s.Amount);
            if (sumOfSplits == null || sumOfSplits.Value != paycheck.TotalAmount)
            {
                return new CreatePaycheckResponse(400, "Split totals must equal paycheck total");

            }
            // Call the DAO method
            response = await _paycheckDAO.CreatePaycheckAsync(paycheck);
            // Return the response
            return response;
        }


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


        public async Task<UpdatePaycheckResponse> UpdatePaycheckAsync(UpdatePaycheckRequest paycheck)
        {
            // Declare and initialize
            UpdatePaycheckResponse response;

            // Check to make sure the required information was provided
            if (paycheck.PaycheckId == 0 || paycheck.UserId == 0)
            {
                return new UpdatePaycheckResponse(400, "Missing information for update");
            }
            if (paycheck.PaycheckSplits.Count != 0 || paycheck.PaycheckSplits.Sum(s => s.Amount) != paycheck.TotalAmount)
            {
                return new UpdatePaycheckResponse(400, "Split totals must equal paycheck total");
            }

            // Call the DAO method
            response = await _paycheckDAO.UpdatePaycheckAsync(paycheck);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete an paycheck
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeletePaycheckAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for update");
            }
            // Return a call the the DAO method
            return await _paycheckDAO.DeletePaycheckAsync(request);
        }
    }
}
