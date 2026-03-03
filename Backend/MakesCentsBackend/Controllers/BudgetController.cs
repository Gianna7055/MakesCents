/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/6973cd55-f264-832f-9be5-87a70c02450d
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Models.Enums;
using MakesCentsBackend.Services.BusinessLogicLayer;
using MakesCentsBackend.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakesCentsBackend.Controllers
{
    /// <summary>
    /// API controller for budgets
    /// </summary>
    [Route("api/budgets")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        // Class level variables
        private readonly BudgetLogic _budgetLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="budgetLogic"></param>
        public BudgetController(BudgetLogic budgetLogic)
        {
            _budgetLogic = budgetLogic;
        }

        /// <summary>
        /// POST method to create a new budget
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateBudgetAsync(CreateBudgetRequest budget)
        {
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            budget.UserId = userId;
            // Call the logic method
            response = await _budgetLogic.CreateBudgetAsync(budget);
            // Check the status
            if (response.HttpStatus == 400)
            {
                // Return the bad request
                return BadRequest(response.Message);
            }
            // Return the success
            return Created("", new
            {
                status = response.HttpStatus,
                message = response.Message,
                budgetId = response.Id
            });
        }

        /// <summary>
        /// Get a budget based on a year, month, and user id
        /// </summary>
        /// <param name="year"></param>
        /// <param name="monthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("year/{year}/month/{monthId}")]
        public async Task<ActionResult> GetBudgetAsync(int year, int monthId)
        {
            // Declare and initialize
            GetBudgetResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);
            // Get the month enum from the month id
            Month month = Enum.IsDefined(typeof(Month), monthId)
                ? (Month)monthId
                : Month.Unknown;
            GetBudgetRequest getBudgetRequest = new GetBudgetRequest(userId, month, year);

            // Call the logic method
            response = await _budgetLogic.GetBudgetAsync(getBudgetRequest);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    userId = response.GetBudgetDTO.UserId,
                    monthId = response.GetBudgetDTO.Month,
                    year = response.GetBudgetDTO.Year
                });    
            }
            // Return the OK response
            return Ok(new
            {
                status = response.HttpStatus,
                message = response.Message,
                budget = response.GetBudgetDTO
            });
        }

        /// <summary>
        /// HTTP PUT method to update a budget based on given fields
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="budget"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{budgetId}")]
        public async Task<ActionResult> UpdateBudgetAsync(int budgetId, EditBudgetRequest budget)
        {
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            budget.UserId = userId;
            // Add the budget id to the request model
            budget.BudgetId = budgetId;
            // Call the logic method
            response = await _budgetLogic.UpdateBudgetAsync(budget);

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
                    budgetId = response.Id
                });
            }
        }

        [Authorize]
        [HttpDelete("{budgetId}")]
        public async Task<ActionResult> DeleteBudgetAsync(int budgetId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = budgetId;
            // Call the logic method to delete the budget
            response = await _budgetLogic.DeleteBudgetAsync(request);

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
