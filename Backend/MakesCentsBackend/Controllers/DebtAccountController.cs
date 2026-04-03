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
            // Check to make sure the debt account is not null
            if (debtAccount == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new debt account");
            }
            // Declare and initialize
            CreateDebtAccountResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            debtAccount.UserId = userId;
            // Call the logic method
            response = await _debtAccountLogic.CreateDebtAccountAsync(debtAccount);
            // Check the status
            if (response.HttpStatus != 201)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Response HttpStatus is 201
            else
            {
                // Return the success
                return Created("", response);
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


        [Authorize]
        [HttpPut("{debtAccountId}")]
        public async Task<ActionResult> UpdateDebtAccountAsync(int debtAccountId, UpdateDebtAccountRequest debtAccount)
        {
            // Check to make sure the debt account is not null
            if (debtAccount == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update a debt account");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the envelope id and the user id in the request
            debtAccount.UserId = userId;
            debtAccount.DebtAccountId = debtAccountId;
            // Call the logic method
            response = await _debtAccountLogic.UpdateDebtAccountAsync(debtAccount);

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
    }
}
