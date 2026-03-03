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
    [Route("api/investment-accounts")]
    [ApiController]
    public class InvestmentAccountController : ControllerBase
    {
        // Class level variables
        private readonly InvestmentAccountLogic _investmentAccountLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="investmentAccountLogic"></param>
        public InvestmentAccountController(InvestmentAccountLogic investmentAccountLogic)
        {
            _investmentAccountLogic = investmentAccountLogic;
        }

        /// <summary>
        /// HTTP POST method to create a new investment account
        /// </summary>
        /// <param name="investmentAccount"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateInvestmentAccountAsync(CreateInvestmentAccountRequest investmentAccount)
        {
            // Declare and initialize
            CreateInvestmentAccountResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            investmentAccount.UserId = userId;
            // Call the logic method
            response = await _investmentAccountLogic.CreateInvestmentAccountAsync(investmentAccount);
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
                    investmentAccountId = response.InvestmentAccountId
                });
            }
        }

        [Authorize]
        [HttpGet("{investmentAccountId}")]
        public async Task<ActionResult> GetInvestmentAccountAsync(int investmentAccountId)
        {
            // Declare and initialize
            GetInvestmentAccountDTOResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the budget id in the request
            request.UserId = userId;
            request.EntityId = investmentAccountId;
            // Call the logic method
            response = await _investmentAccountLogic.GetInvestmentAccountAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    investmentAccountId = investmentAccountId
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
                investmentAccount = response.InvestmentAccount
            });
        }


        [Authorize]
        [HttpPut("{investmentAccountId}")]
        public async Task<ActionResult> UpdateInvestmentAccountAsync(int investmentAccountId, UpdateInvestmentAccountRequest request)
        {
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the envelope id and the user id in the request
            request.UserId = userId;
            request.InvestmentAccountId = investmentAccountId;
            // Call the logic method
            response = await _investmentAccountLogic.UpdateInvestmentAccountAsync(request);

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
                    investmentAccountId = response.Id
                });
            }
        }
    }
}
