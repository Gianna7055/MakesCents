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
            if (paymentTransaction.BudgetId == null || paymentTransaction.UserId == null || paymentTransaction.TransactionDate == null || paymentTransaction.TotalAmount == null || paymentTransaction.PaymentTransactionType == PaymentTransactionType.Unknown)
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

                if (split.EnvelopeId == null)
                    return new CreatePaymentTransactionResponse(400, "Split envelope is required");
            }
            // Total the splits
            sumOfSplits = paymentTransaction.TransactionSplits.Sum(s => s.Amount);
            if (sumOfSplits == null || sumOfSplits.Value != paymentTransaction.TotalAmount)
            {
                return new CreatePaymentTransactionResponse(400, "Split totals must equal transaction total");

            }
            // Call the DAO method
            response = await _paymentTransactionDAO.CreatePaymentTransactionAsync(paymentTransaction);
            // Return the response
            return response;
        }


        public async Task<GetPaymentTransactionResponse> GetPaymentTransactionAsync(BaseGetRequest request)
        {
            // Declare and initialize
            GetPaymentTransactionResponse response;

            // Call the DAO method
            response = await _paymentTransactionDAO.GetPaymentTransactionAsync(request);
            // Return the response
            return response;
        }

    }
}
