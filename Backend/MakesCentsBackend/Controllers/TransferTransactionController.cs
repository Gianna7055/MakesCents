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
        /// HTTP POST method to create a new transfer transaction (account-to-account or envelope-to-envelope)
        /// </summary>
        /// <param name="transferTransaction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateTransferTransactionAsync(CreateTransferTransactionRequest transferTransaction)
        {
            // Check to make sure the transfer transaction is not null
            if (transferTransaction == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new transfer transaction");
            }
            // Declare and initialize
            CreateTransferTransactionResponse response;
            // Get the User id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the transfer transaction
            transferTransaction.UserId = userId;
            // Call the logic method
            response = await _transferTransactionLogic.CreateTransferTransactionAsync(transferTransaction);
            // Check the status
            if (response.HttpStatus != 201)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            else // Response HttpStatus is 201
            {
                // Return the success
                return Created("", response);
            }
        }

        /// <summary>
        /// HTTP GET method to get a transfer transaction
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{transactionId}")]
        public async Task<ActionResult> GetTransferTransactionAsync(int transactionId)
        {
            // Declare and initialize
            GetTransferTransactionDTOResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user and transaction ids in the request
            request.UserId = userId;
            request.EntityId = transactionId;
            // Call the logic method
            response = await _transferTransactionLogic.GetTransferTransactionAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    transactionId = transactionId
                });
            }
            else if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the OK response
            return Ok(response);
        }

        /// <summary>
        /// HTTP PUT method to update a transfer transaction
        /// </summary>
        /// <param name="transferTransactionId"></param>
        /// <param name="transferTransaction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{transferTransactionId}")]
        public async Task<ActionResult> UpdateTransferTransactionAsync(int transferTransactionId, UpdateTransferTransactionRequest transferTransaction)
        {
            // Check to make sure the transfer transaction is not null
            if (transferTransaction == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update a transfer transaction");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the transfer transaction id and user id in the request
            transferTransaction.UserId = userId;
            transferTransaction.TransferTransactionId = transferTransactionId;
            // Call the logic method
            response = await _transferTransactionLogic.UpdateTransferTransactionAsync(transferTransaction);

            // Check if the status came back as a success
            if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            else
            {
                // Return Ok
                return Ok(response);
            }
        }
    }
}