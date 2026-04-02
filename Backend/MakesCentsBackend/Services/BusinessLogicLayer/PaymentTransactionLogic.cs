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
    public class PaymentTransactionLogic
    {
        // Class level variables
        private readonly PaymentTransactionDAO _paymentTransactionDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="paymentTransactionDAO"></param>
        public PaymentTransactionLogic(PaymentTransactionDAO paymentTransactionDAO)
        {
            _paymentTransactionDAO = paymentTransactionDAO;
        }

        /// <summary>
        /// Logic method to create a new payment transaction
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <returns></returns>
        public async Task<CreatePaymentTransactionResponse> CreatePaymentTransactionAsync(CreatePaymentTransactionRequest paymentTransaction)
        {
            // Declare and initialize
            CreatePaymentTransactionResponse response;
            decimal? sumOfSplits;

            // Make sure the necessary information was sent
            if (paymentTransaction.BudgetId == 0 || paymentTransaction.UserId == 0 || paymentTransaction.TransactionDate == DateOnly.MinValue || paymentTransaction.TotalAmount == 0m || paymentTransaction.PaymentTransactionType == PaymentTransactionType.Unknown)
            {
                return new CreatePaymentTransactionResponse(400, "Missing information for payment transaction creation");
            }
            if (paymentTransaction.TransactionSplits == null || paymentTransaction.TransactionSplits.Count == 0)
            {
                return new CreatePaymentTransactionResponse(400, "Payment transaction must contain at least one split");
            }
            // Loop through the splits to make sure the necessary information was sent
            foreach (CreateTransactionSplitRequest split in paymentTransaction.TransactionSplits)
            {
                if (split == null)
                    return new CreatePaymentTransactionResponse(400, "Split cannot be null");

                if (split.Amount <= 0)
                    return new CreatePaymentTransactionResponse(400, "Split amount must be greater than 0");

                if (split.EnvelopeId == 0)
                    return new CreatePaymentTransactionResponse(400, "Split envelope is required");
            }
            // Total the splits
            sumOfSplits = paymentTransaction.TransactionSplits.Sum(s => s.Amount);
            if (sumOfSplits == null || sumOfSplits.Value != Math.Abs(paymentTransaction.TotalAmount))
            {
                return new CreatePaymentTransactionResponse(400, "Split totals must equal transaction total");

            }
            // Call the DAO method
            response = await _paymentTransactionDAO.CreatePaymentTransactionAsync(paymentTransaction);
            // Return the response
            return response;
        }


        public async Task<GetPaymentTransactionResponse> GetPaymentTransactionAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetPaymentTransactionResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetPaymentTransactionResponse(400, "Missing information to get payment transaction");
            }
            // Call the DAO method
            response = await _paymentTransactionDAO.GetPaymentTransactionAsync(request);
            // Return the response
            return response;
        }


        public async Task<UpdatePaymentTransactionResponse> UpdatePaymentTransactionAsync(UpdatePaymentTransactionRequest paymentTransaction)
        {
            // Declare and initialize
            UpdatePaymentTransactionResponse response;

            // Check to make sure the required information was provided
            if (paymentTransaction.TransactionId == 0 || paymentTransaction.PaymentTransactionId == 0 || paymentTransaction.UserId == 0)
            {
                // Return that there is not enough information
                return new UpdatePaymentTransactionResponse(400, "Missing information for update");
            }
            if (paymentTransaction.TransactionSplits.Count != 0 || paymentTransaction.TransactionSplits.Sum(s => s.Amount) != paymentTransaction.TotalAmount)
            {
                return new UpdatePaymentTransactionResponse(400, "Split totals must equal paycheck total");
            }
            // Call the update method in the DAO
            response = await _paymentTransactionDAO.UpdatePaymentTransactionAsync(paymentTransaction);
            // Return the response
            return response;
        }
    }
}
