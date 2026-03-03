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
    [Route("api/debt-accounts")]
    [ApiController]
    public class DebtAccountController : ControllerBase
    {
        // Class level variables
        private readonly DebtAccountLogic _debtAccountLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="debtAccountLogic"></param>
        public DebtAccountController(DebtAccountLogic debtAccountLogic)
        {
            _debtAccountLogic = debtAccountLogic;
        }

        /// <summary>
        /// HTTP POST method to create a new debt account
        /// </summary>
        /// <param name="debtAccount"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateDebtAccountAsync(CreateDebtAccountRequest debtAccount)
        {
            // Declare and initialize
            CreateDebtAccountResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            debtAccount.UserId = userId;
            // Call the logic method
            response = await _debtAccountLogic.CreateDebtAccountAsync(debtAccount);
            // Check the status
            if (response.HttpStatus == 400)
            {
                // Return the bad request
                return BadRequest(response.Message);
            }
            else if (response.HttpStatus == 403)
            {
                // Return the forbidden response
                return StatusCode(StatusCodes.Status403Forbidden, response.Message);
            }
            // Response HttpStatus is 201
            else
            {
                // Return the success
                return Created("", new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    accountId = response.AccountId,
                    debtAccountId = response.DebtAccountId
                });
            }
        }
        

        [Authorize]
        [HttpGet("{debtAccountId}")]
        public async Task<ActionResult> GetDebtAccountAsync(int debtAccountId)
        {
            // Declare and initialize
            GetDebtAccountDTOResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the budget id in the request
            request.UserId = userId;
            request.EntityId = debtAccountId;
            // Call the logic method
            response = await _debtAccountLogic.GetDebtAccountAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    debtAccountId = debtAccountId
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
                debtAccount = response.DebtAccount
            });
        }


        [Authorize]
        [HttpPut("{debtAccountId}")]
        public async Task<ActionResult> UpdateDebtAccountAsync(int debtAccountId, UpdateDebtAccountRequest request)
        {
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the envelope id and the user id in the request
            request.UserId = userId;
            request.DebtAccountId = debtAccountId;
            // Call the logic method
            response = await _debtAccountLogic.UpdateDebtAccountAsync(request);

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
                    debtAccountId = response.Id
                });
            }
        }
    }
}
