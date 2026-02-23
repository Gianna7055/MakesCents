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
            if (paycheck.BudgetId == null || paycheck.UserId == null || string.IsNullOrEmpty(paycheck.PaycheckName) || paycheck.StartingDate == null || paycheck.TotalAmount == null || paycheck.PaycheckRegularity == PaycheckRegularity.Unknown)
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

                if (split.EnvelopeId == null)
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


        public async Task<GetAllPaychecksResponse> GetAllPaychecksAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetAllPaychecksResponse response;

            // Call the DAO method
            response = await _paycheckDAO.GetAllPaychecksAsync(request);
            // Return the response
            return response;
        }


        public async Task<GetPaycheckResponse> GetPaycheckAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetPaycheckResponse response;

            // Call the DAO method
            response = await _paycheckDAO.GetPaycheckAsync(request);
            // Return the response
            return response;
        }

    }
}
