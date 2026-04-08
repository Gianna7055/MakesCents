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
    /// Controller for generate Account APIs, including Get All Accounts and Delete Account
    /// </summary>
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // Class level variables
        private readonly AccountLogic _accountLogic;

        /// <summary>
        /// Parametrized constructor to bring in Dependency Injected Account Logic
        /// </summary>
        /// <param name="accountLogic"></param>
        public AccountController(AccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        /// <summary>
        /// Account API to get a list of summaries for all accounts in the given budget
        /// </summary>
        /// <param name="budgetId">The id for the budget in question</param>
        /// <returns></returns>
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

        /// <summary>
        /// Account API to delete an account based on the given account id
        /// </summary>
        /// <param name="accountId">The Id of the account to delete</param>
        /// <returns></returns>
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
