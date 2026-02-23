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
            // Call the logic method
            response = await _paymentTransactionLogic.CreatePaymentTransactionAsync(paymentTransaction);
            // Check the status
            if (response.HttpStatus == 400)
            {
                // Return the bad request
                return BadRequest(response.Message);
            }
            else if (response.HttpStatus == 403)
            {
                // Return the forbidden response
                return StatusCode(StatusCodes.Status403Forbidden, response.Message);
            }
            // Response HttpStatus is 201
            else
            {
                // Return the success
                return Created("", new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    transactionId = response.Id,
                    paymentTransactionId = response.PaymentTransactionId,
                    paycheckSplitIds = response.TransactionSplitIds
                });
            }
        }

        [Authorize]
        [HttpGet("{paycheckTransactionId}")]
        public async Task<ActionResult> GetPaymentTransactionAsync(int paycheckTransactionId)
        {
            // Declare and initialize
            GetPaymentTransactionResponse response;
            BaseGetRequest request = new BaseGetRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user and paycheck transaction ids in the request
            request.UserId = userId;
            request.EntityId = paycheckTransactionId;
            // Call the logic method
            response = await _paymentTransactionLogic.GetPaymentTransactionAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    paycheckTransactionId = paycheckTransactionId
                });
            }
            else if (response.HttpStatus == 403)
            {
                // Return the forbidden response
                return Forbid(response.Message);
            }
            // Return the OK response
            return Ok(new
            {
                status = response.HttpStatus,
                message = response.Message,
                paymentTransaction = response.PaymentTransaction
            });
        }
    }
}
