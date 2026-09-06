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
    /// <summary>
    /// API controller for transactions
    /// </summary>
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        // Class level variables
        private readonly TransactionLogic _transactionLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="transactionLogic"></param>
        public TransactionController(TransactionLogic transactionLogic)
        {
            _transactionLogic = transactionLogic;
        }

        [Authorize]
        [HttpGet("{budgetId}")]
        public async Task<ActionResult> GetAllTransactionsAsync(int budgetId)
        {
            // Declare or initialize
            GetAllTransactionsDTOResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget id in the request
            request.UserId = userId;
            request.EntityId = budgetId;
            // Call the logic method
            response = await _transactionLogic.GetAllTransactionsAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    budgetId = budgetId
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


        [Authorize]
        [HttpDelete("{transactionId}")]
        public async Task<ActionResult> SoftDeleteTransactionAsync(int transactionId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = transactionId;
            // Call the logic method to delete the transaction
            response = await _transactionLogic.SoftDeleteTransactionAsync(request);

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

        [Authorize]
        [HttpPatch("{transactionId}/restore")]
        public async Task<ActionResult> RestoreTransactionAsync(int transactionId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();

            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and entity id in the request
            request.UserId = userId;
            request.EntityId = transactionId;

            // Call the logic method
            response = await _transactionLogic.RestoreTransactionAsync(request);

            // Check for errors
            if (response.HttpStatus != 200)
            {
                return StatusCode(response.HttpStatus, response.Message);
            }

            // Return success
            return Ok(response);
        }


        [Authorize]
        [HttpDelete("{transactionId}/permanent")]
        public async Task<ActionResult> HardDeleteTransactionAsync(int transactionId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = transactionId;
            // Call the logic method to delete the transaction
            response = await _transactionLogic.HardDeleteTransactionAsync(request);

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
