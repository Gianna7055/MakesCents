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
using System.Security.Claims;

namespace MakesCentsBackend.Controllers
{
    /// <summary>
    /// API controller for accounts
    /// </summary>
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // Class level variables
        private readonly AccountLogic _accountLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="bankAccountLogic"></param>
        public AccountController(AccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        [Authorize]
        [HttpGet("budget/{budgetId}")]
        public async Task<ActionResult> GetAllAccounts(int budgetId)
        {
            // Declare and initialize
            GetAllAccountsResponse response;
            BaseGetRequest request = new BaseGetRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the budget id in the request
            request.UserId = userId;
            request.EntityId = budgetId;
            // Call the logic method
            response = await _accountLogic.GetAllAccountsAsync(request);
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
                accounts = response.Accounts
            });
        }
    }
}
