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
            BaseIdRequest request = new BaseIdRequest();
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


        [Authorize]
        [HttpDelete("{accountId}")]
        public async Task<ActionResult> DeleteAccountAsync(int accountId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = accountId;
            // Call the logic method to delete the account
            response = await _accountLogic.DeleteAccountAsync(request);


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
