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
            // Check to make sure the budget is not null
            if (budget == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new budget");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            budget.UserId = userId;
            // Call the logic method
            response = await _budgetLogic.CreateBudgetAsync(budget);
            // Check the status
            if (response.HttpStatus != 201)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            else
            {
                // Return the success
                return Created("", new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    budgetId = response.Id
                });
            }
        }

        /// <summary>
        /// Get a budget based on a year, month, and user id
        /// </summary>
        /// <param name="year"></param>
        /// <param name="monthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("year/{year}/month/{month}")]
        public async Task<ActionResult> GetBudgetAsync(int year, Month month)
        {
            // Declare and initialize
            GetBudgetResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);
            // Change Month=0 to Month.Unknown
            if (month == 0)
            {
                month = Month.Unknown;
            }
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
                    userId = userId,
                    month = month,
                    year = year
                });
            }
            else if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            else
            {
                // Return the OK response
                return Ok(response);
            }
        }

        /// <summary>
        /// Get a budget based on a budget id and user id
        /// </summary>
        /// <param name="year"></param>
        /// <param name="monthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{budgetId}")]
        public async Task<ActionResult> GetBudgetAsync(int budgetId)
        {
            // Declare and initialize
            GetBudgetResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id in the request
            request.UserId = userId;
            request.EntityId = budgetId;
            // Call the logic method
            response = await _budgetLogic.GetBudgetAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    userId = userId,
                    budgetId = budgetId
                });
            }
            else if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            else
            {
                // Return the OK response
                return Ok(response);
            }
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
            // Check to make sure the budget is not null
            if (budget == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update a budget");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Make sure the budget is not null
            if (budget == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Missing information for budget update");
            }
            // Set the user id for the budget
            budget.UserId = userId;
            // Add the budget id to the request model
            budget.BudgetId = budgetId;
            // Call the logic method
            response = await _budgetLogic.UpdateBudgetAsync(budget);

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
                });
            }
        }
    }
}