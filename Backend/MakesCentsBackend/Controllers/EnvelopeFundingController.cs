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
    /// API controller for the envelope funding queue - a budget month's plan for how much to
    /// put into each envelope and in what priority order.
    /// </summary>
    [Route("api/envelope-funding")]
    [ApiController]
    public class EnvelopeFundingController : ControllerBase
    {
        // Class level variables
        private readonly EnvelopeFundingLogic _envelopeFundingLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="envelopeFundingLogic"></param>
        public EnvelopeFundingController(EnvelopeFundingLogic envelopeFundingLogic)
        {
            _envelopeFundingLogic = envelopeFundingLogic;
        }

        /// <summary>
        /// HTTP PUT method to set (create/reorder/update/remove all at once) the funding queue
        /// for a budget month
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <param name="fundingQueue"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("budget-month/{budgetMonthId}")]
        public async Task<ActionResult> SetEnvelopeFundingQueueAsync(int budgetMonthId, SetEnvelopeFundingQueueRequest fundingQueue)
        {
            // Check to make sure the funding queue is not null
            if (fundingQueue == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to set the funding queue");
            }
            // Declare and initialize
            GetEnvelopeFundingQueueResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget month id for the funding queue
            fundingQueue.UserId = userId;
            fundingQueue.BudgetMonthId = budgetMonthId;
            // Call the logic method
            response = await _envelopeFundingLogic.SetEnvelopeFundingQueueAsync(fundingQueue);
            // Check the status
            if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the OK response
            return Ok(response);
        }

        /// <summary>
        /// HTTP GET method to get the funding queue for a budget month
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("budget-month/{budgetMonthId}")]
        public async Task<ActionResult> GetEnvelopeFundingQueueAsync(int budgetMonthId)
        {
            // Declare and initialize
            GetEnvelopeFundingQueueResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget month id in the request
            request.UserId = userId;
            request.EntityId = budgetMonthId;
            // Call the logic method
            response = await _envelopeFundingLogic.GetEnvelopeFundingQueueAsync(request);
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
        /// HTTP DELETE method to delete a single envelope funding row, without touching the
        /// rest of the queue
        /// </summary>
        /// <param name="envelopeFundingId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{envelopeFundingId}")]
        public async Task<ActionResult> DeleteEnvelopeFundingAsync(int envelopeFundingId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = envelopeFundingId;
            // Call the logic method to delete the envelope funding row
            response = await _envelopeFundingLogic.DeleteEnvelopeFundingAsync(request);

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