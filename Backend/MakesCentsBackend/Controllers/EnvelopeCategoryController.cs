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
        /// HTTP POST method to create an envelope category
        /// </summary>
        /// <param name="envelopeCategory"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateEnvelopeCategoryAsync(CreateEnvelopeCategoryRequest envelopeCategory)
        {
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            envelopeCategory.UserId = userId;
            // Call the logic method
            response = await _envelopeCategoryLogic.CreateEnvelopeCategoryAsync(envelopeCategory);
            // Check the status
            if (response.HttpStatus == 400)
            {
                // Return the bad request
                return BadRequest(response.Message);
            }
            else if (response.HttpStatus == 403)
            {
                // Return the forbidden response
                return Forbid(response.Message);
            }
            // Return the success
            return Created("", new
            {
                status = response.HttpStatus,
                message = response.Message,
                envelopeCategoryId = response.Id
            });
        }

        /// <summary>
        /// HTTP GET method to get all envelope categories in a budget
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("budget/{budgetId}")]
        public async Task<ActionResult> GetAllEnvelopeCategoriesAsync(int budgetId)
        {
            // Declare and initialize
            GetAllEnvelopeCategoriesResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Call the logic method
            response = await _envelopeCategoryLogic.GetAllEnvelopeCategoriesAsync(budgetId, userId);
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
                envelopeCategories = response.AllEnvelopeCategories
            });
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
                    envelopeCategoryId = response.Id
                });
            }
        }


        [Authorize]
        [HttpDelete("{envelopeCategoryId}")]
        public async Task<ActionResult> DeleteEnvelopeCategoryAsync(int envelopeCategoryId)
        {
            // Declare and initialize
            BaseResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Call the logic method to delete the envelope category
            response = await _envelopeCategoryLogic.DeleteEnvelopeCategoryAsync(envelopeCategoryId, userId);

            if (response.HttpStatus == 404)
            {
                // Return a not found
                return NotFound(response.Message);
            }
            else if (response.HttpStatus == 403)
            {
                // Return the forbidden response
                return Forbid(response.Message);
            }
            else
            {
                // Return Ok
                return Ok(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                });
            }
        }
    }
}
