/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.BusinessLogicLayer;
using MakesCentsBackend.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakesCentsBackend.Controllers
{
    [Route("api/payment-transactions")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        // Class level variables
        private readonly PaymentTransactionLogic _paymentTransactionLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="paymentTransactionLogic"></param>
        public PaymentTransactionController(PaymentTransactionLogic paymentTransactionLogic)
        {
            _paymentTransactionLogic = paymentTransactionLogic;
        }

        /// <summary>
        /// HTTP POST request to create a new payment transaction
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreatePaymentTransactionAsync(CreatePaymentTransactionRequest paymentTransaction)
        {
            // Declare and initialize
            CreatePaymentTransactionResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the payment transaction
            paymentTransaction.UserId = userId;
            // Call the logic metho
        }
    }
}
