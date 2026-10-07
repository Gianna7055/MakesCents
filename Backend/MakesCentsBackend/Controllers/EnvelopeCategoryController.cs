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
    /// API controller for envelope categories
    /// </summary>
    /// <remarks>
    /// Parameterized constructor to bring in DI variables
    /// </remarks>
    /// <param name="budgetLogic"></param>
    [Route("api/envelope-categories")]
    [ApiController]
    public class EnvelopeCategoryController : ControllerBase
    {
        // Class level variables
        private readonly EnvelopeCategoryLogic _envelopeCategoryLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="envelopeCategoryLogic"></param>
        public EnvelopeCategoryController(EnvelopeCategoryLogic envelopeCategoryLogic)
        {
            _envelopeCategoryLogic = envelopeCategoryLogic;
        }

        /// <summary>
        /// HTTP POST method to create an envelope category. Still keyed to a persistent
        /// BudgetId - categories aren't month-scoped.
        /// </summary>
        /// <param name="envelopeCategory"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateEnvelopeCategoryAsync(CreateEnvelopeCategoryRequest envelopeCategory)
        {
            // Check to make sure the envelope category is not null
            if (envelopeCategory == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new envelope category");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            envelopeCategory.UserId = userId;
            // Call the logic method
            response = await _envelopeCategoryLogic.CreateEnvelopeCategoryAsync(envelopeCategory);
            // Check the status
            if (response.HttpStatus != 201)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the success
            return Created("", response);
        }

        /// <summary>
        /// HTTP GET method to get all envelope categories (with envelopes) for a budget month.
        /// NOTE: the route param is now a budget_month_id, not a budget_id - the DAO derives
        /// the persistent budget via a join, and needs the month to resolve rollover envelope amounts.
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("budget-month/{budgetMonthId}")]
        public async Task<ActionResult> GetAllEnvelopeCategoriesAsync(int budgetMonthId)
        {
            // Declare and initialize
            GetAllEnvelopeCategoriesResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id in the request
            request.UserId = userId;
            request.EntityId = budgetMonthId;
            // Call the logic method
            response = await _envelopeCategoryLogic.GetAllEnvelopeCategoriesAsync(request);
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
        /// HTTP GET method to get a single envelope category
        /// </summary>
        /// <param name="envelopeCategoryId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{envelopeCategoryId}")]
        public async Task<ActionResult> GetEnvelopeCategoryAsync(int envelopeCategoryId)
        {
            // Declare and initialize
            GetEnvelopeCategoryResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id in the request
            request.UserId = userId;
            request.EntityId = envelopeCategoryId;
            // Call the logic method
            response = await _envelopeCategoryLogic.GetEnvelopeCategoryAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    envelopeCategoryId = envelopeCategoryId
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
        /// Update an envelope category based on the provided fields
        /// </summary>
        /// <param name="envelopeCategoryId"></param>
        /// <param name="envelopeCategory"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{envelopeCategoryId}")]
        public async Task<ActionResult> UpdateEnvelopeCategoryAsync(int envelopeCategoryId, EditEnvelopeCategoryRequest envelopeCategory)
        {
            // Check to make sure the envelope category is not null
            if (envelopeCategory == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update an envelope category");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            envelopeCategory.UserId = userId;
            // Add the envelope category id to the request model
            envelopeCategory.EnvelopeCategoryId = envelopeCategoryId;
            // Call the logic method
            response = await _envelopeCategoryLogic.UpdateEnvelopeCategoryAsync(envelopeCategory);

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

        /// <summary>
        /// Delete an envelope category
        /// </summary>
        /// <param name="envelopeCategoryId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{envelopeCategoryId}")]
        public async Task<ActionResult> DeleteEnvelopeCategoryAsync(int envelopeCategoryId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = envelopeCategoryId;
            // Call the logic method to delete the envelope category
            response = await _envelopeCategoryLogic.DeleteEnvelopeCategoryAsync(request);

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