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
            if (response.HttpStatus != 201)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
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
            BaseIdRequest request = new BaseIdRequest();
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
            else if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the OK response
            return Ok(new
            {
                status = response.HttpStatus,
                message = response.Message,
                paymentTransaction = response.PaymentTransaction
            });
        }

        [Authorize]
        [HttpPut("{paymentTransactionId}")]
        public async Task<ActionResult> UpdatePaymentTransactionAsync(int paymentTransactionId, UpdatePaymentTransactionRequest paymentTransaction)
        {
            // Declare and initialize
            UpdatePaymentTransactionResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the payment transaction id and user id in the request
            paymentTransaction.UserId = userId;
            paymentTransaction.PaymentTransactionId = paymentTransactionId;
            // Call the logic method
            response = await _paymentTransactionLogic.UpdatePaymentTransactionAsync(paymentTransaction);

            // Check if the status came back as a success
            if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            else
            {
                // Return Ok
                return Ok(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    paymentTransactionId = response.Id,
                    transactionSplitIds = response.TransactionSplitIds
                });
            }
        }
    }
}
