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
    /// API controller for budgets. Persistent budget routes live directly under /api/budgets;
    /// month-specific routes live under /api/budgets/months, since a persistent budget spans
    /// many months and needs its own id space separate from any one month's.
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

        // ============================================
        // Budget (the persistent household/plan)
        // ============================================

        /// <summary>
        /// POST method to create a new persistent budget (no month yet)
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
                return Created("", response);
            }
        }

        /// <summary>
        /// HTTP PUT method to update a budget's persistent fields (currently just the name)
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
                return Ok(response);
            }
        }

        /// <summary>
        /// HTTP DELETE method to delete a persistent budget entirely - cascades every month,
        /// account, envelope category, paycheck, and planned expense it has
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
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
                return Ok(response);
            }
        }

        /// <summary>
        /// HTTP GET method to get every budget the logged-in user has access to (id and name
        /// only) - meant for use right after login, before any month or budget id is known.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUserBudgetsAsync()
        {
            // Declare and initialize
            GetUserBudgetsResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Call the logic method
            response = await _budgetLogic.GetUserBudgetsAsync(userId);
            // Check the status
            if (response.HttpStatus != 200)
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

        // ============================================
        // BudgetMonth (a specific month's use of a budget)
        // ============================================

        /// <summary>
        /// POST method to create a new month under an existing persistent budget
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="budgetMonth"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{budgetId}/months")]
        public async Task<ActionResult> CreateBudgetMonthAsync(int budgetId, CreateBudgetMonthRequest budgetMonth)
        {
            // Check to make sure the budget month is not null
            if (budgetMonth == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new budget month");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget id for the budget month
            budgetMonth.UserId = userId;
            budgetMonth.BudgetId = budgetId;
            // Call the logic method
            response = await _budgetLogic.CreateBudgetMonthAsync(budgetMonth);
            // Check the status
            if (response.HttpStatus != 201)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            else
            {
                // Return the success
                return Created("", response);
            }
        }

        /// <summary>
        /// Get a budget month based on a budget id, year, month, and user id
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{budgetId}/months/year/{year}/month/{month}")]
        public async Task<ActionResult> GetBudgetMonthAsync(int budgetId, int year, Month month)
        {
            // Declare and initialize
            GetBudgetMonthResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);
            // Change Month=0 to Month.Unknown
            if (month == 0)
            {
                month = Month.Unknown;
            }
            GetBudgetRequest getBudgetRequest = new GetBudgetRequest(userId, budgetId, month, year);

            // Call the logic method
            response = await _budgetLogic.GetBudgetMonthAsync(getBudgetRequest);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    userId = userId,
                    budgetId = budgetId,
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
        /// Get up to a year's worth of a budget's past months, anchored at a year/month
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{budgetId}/all/year/{year}/month/{month}")]
        public async Task<ActionResult> GetYearBudgetsAsync(int budgetId, int year, Month month)
        {
            // Declare and initialize
            GetYearBudgetsResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);
            // Change Month=0 to Month.Unknown
            if (month == 0)
            {
                month = Month.Unknown;
            }
            GetBudgetRequest getYearBudgetsRequest = new GetBudgetRequest(userId, budgetId, month, year);

            // Call the logic method
            response = await _budgetLogic.GetYearBudgetsAsync(getYearBudgetsRequest);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    userId = userId,
                    budgetId = budgetId,
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
        /// Get a budget month based on its budget_month_id
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("months/{budgetMonthId}")]
        public async Task<ActionResult> GetBudgetMonthAsync(int budgetMonthId)
        {
            // Declare and initialize
            GetBudgetMonthResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id in the request
            request.UserId = userId;
            request.EntityId = budgetMonthId;
            // Call the logic method
            response = await _budgetLogic.GetBudgetMonthAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    userId = userId,
                    budgetMonthId = budgetMonthId
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
        /// HTTP PUT method to update a specific budget month's month/year (e.g. fixing a mistake)
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <param name="budgetMonth"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("months/{budgetMonthId}")]
        public async Task<ActionResult> UpdateBudgetMonthAsync(int budgetMonthId, EditBudgetMonthRequest budgetMonth)
        {
            // Check to make sure the budget month is not null
            if (budgetMonth == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update a budget month");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget month id for the budget month
            budgetMonth.UserId = userId;
            budgetMonth.BudgetMonthId = budgetMonthId;
            // Call the logic method
            response = await _budgetLogic.UpdateBudgetMonthAsync(budgetMonth);

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
        /// HTTP DELETE method to delete a single month's budget. Only that month's transactions
        /// go with it - accounts, envelope categories, and paychecks are unaffected.
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("months/{budgetMonthId}")]
        public async Task<ActionResult> DeleteBudgetMonthAsync(int budgetMonthId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = budgetMonthId;
            // Call the logic method to delete the budget month
            response = await _budgetLogic.DeleteBudgetMonthAsync(request);

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