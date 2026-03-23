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
    /// API controller for bank accounts
    /// </summary>
    [Route("api/bank-accounts")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        // Class level variables
        private readonly BankAccountLogic _bankAccountLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="bankAccountLogic"></param>
        public BankAccountController(BankAccountLogic bankAccountLogic)
        {
            _bankAccountLogic = bankAccountLogic;
        }

        /// <summary>
        /// POST method to create a new bank account
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateBankAccountAsync(CreateBankAccountRequest bankAccount)
        {
            // Check to make sure the bank account is not null
            if (bankAccount == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new bank account");
            }
            // Declare and initialize
            CreateBankAccountResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            bankAccount.UserId = userId;
            // Call the logic method
            response = await _bankAccountLogic.CreateBankAccountAsync(bankAccount);
            // Check the status
            if (response.HttpStatus != 201)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            else // responses HttpStatus is 201
            {
                // Return the success
                return Created("", new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    accountId = response.AccountId,
                    bankAccountId = response.BankAccountId
                });
            }
        }

        [Authorize]
        [HttpGet("{bankAccountId}")]
        public async Task<ActionResult> GetBankAccountAsync(int bankAccountId)
        {
            // Declare and initialize
            GetBankAccountDTOResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the budget id in the request
            request.UserId = userId;
            request.EntityId = bankAccountId;
            // Call the logic method
            response = await _bankAccountLogic.GetBankAccountAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    bankAccountId = bankAccountId
                });
            }
            else if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the OK response
            return Ok(new
            {
                status = response.HttpStatus,
                message = response.Message,
                bankAccount = response.BankAccount
            });
        }

        [Authorize]
        [HttpPut("{bankAccountId}")]
        public async Task<ActionResult> UpdateBankAccountAsync(int bankAccountId, UpdateBankAccountRequest bankAccount)
        {
            // Check to make sure the bank account is not null
            if (bankAccount == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update a bank account");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the envelope id and the user id in the request
            bankAccount.UserId = userId;
            bankAccount.BankAccountId = bankAccountId;
            // Call the logic method
            response = await _bankAccountLogic.UpdateBankAccountAsync(bankAccount);

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
                    bankAccountId = response.Id
                });
            }
        }
    }
}
