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
    [Route("api/transfer-transactions")]
    [ApiController]
    public class TransferTransactionController : ControllerBase
    {
        // Class level properties
        private readonly TransferTransactionLogic _transferTransactionLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="transferTransactionLogic"></param>
        public TransferTransactionController(TransferTransactionLogic transferTransactionLogic)
        {
            _transferTransactionLogic = transferTransactionLogic;
        }

        /// <summary>
        /// HTTP POST method to create a new transfer transaction
        /// </summary>
        /// <param name="transferTransaction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateTransferTransactionAsync(CreateTransferTransactionRequest transferTransaction)
        {
            // Declare and initialize
            CreateTransferTransactionResponse response;
            // Get the User id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the transfer transaction
            transferTransaction.UserId = userId;
            // Call the logic method
            response = await _transferTransactionLogic.CreateTransferTransactionAsync(transferTransaction);
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
            else // Response HttpStatus is 201
            {
                // Return the success
                return Created("", new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    transactionId = response.Id,
                    transferTransactionId = response.TransferTransactionId
                });
            }
        }

        [Authorize]
        [HttpGet("{paycheckTransactionId}")]
        public async Task<ActionResult> GetTransferTransactionAsync(int paycheckTransactionId)
        {
            // Declare and initialize
            GetTransferTransactionDTOResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user and paycheck transaction ids in the request
            request.UserId = userId;
            request.EntityId = paycheckTransactionId;
            // Call the logic method
            response = await _transferTransactionLogic.GetTransferTransactionAsync(request);
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
                transferTransaction = response.TransferTransaction
            });
        }

        [Authorize]
        [HttpPut("{transferTransactionId}")]
        public async Task<ActionResult> UpdateTransferTransactionAsync(int transferTransactionId, UpdateTransferTransactionRequest transferTransaction)
        {
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the payment transaction id and user id in the request
            transferTransaction.UserId = userId;
            transferTransaction.TransferTransactionId = transferTransactionId;
            // Call the logic method
            response = await _transferTransactionLogic.UpdateTransferTransactionAsync(transferTransaction);

            // Check if the status came back as a success
            if (response.HttpStatus == 400)
            {
                // Return a bad request
                return BadRequest(response.Message);
            }
            else if (response.HttpStatus == 403)
            {
                // Return the forbidden response
                return Forbid(response.Message);
            }
            else if (response.HttpStatus == 404)
            {
                // Return a not found
                return NotFound(response.Message);
            }
            else
            {
                // Return Ok
                return Ok(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    transferTransactionId = response.Id
                });
            }
        }
    }
}
