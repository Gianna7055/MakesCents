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

        /// <summary>
        /// HTTP GET method to get every transaction for a budget month.
        /// NOTE: the route param is now a budget_month_id, not a budget_id - transactions are
        /// scoped to one specific month.
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("budget-month/{budgetMonthId}")]
        public async Task<ActionResult> GetAllTransactionsAsync(int budgetMonthId)
        {
            // Declare or initialize
            GetAllTransactionsDTOResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget month id in the request
            request.UserId = userId;
            request.EntityId = budgetMonthId;
            // Call the logic method
            response = await _transactionLogic.GetAllTransactionsAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    budgetMonthId = budgetMonthId
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
        /// HTTP DELETE method to soft delete a transaction
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HTTP PATCH method to restore a soft-deleted transaction
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// HTTP DELETE method to permanently delete a transaction
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
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