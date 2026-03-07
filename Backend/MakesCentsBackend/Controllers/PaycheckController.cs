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
    /// API controller for paychecks
    /// </summary>
    [Route("api/paychecks")]
    [ApiController]
    public class PaycheckController : ControllerBase
    {
        // Class level variables
        private readonly PaycheckLogic _paycheckLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="paycheckLogic"></param>
        public PaycheckController(PaycheckLogic paycheckLogic)
        {
            _paycheckLogic = paycheckLogic;
        }

        /// <summary>
        /// HTTP POST method to create a new paycheck
        /// </summary>
        /// <param name="paycheck"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreatePaycheckAsync(CreatePaycheckRequest paycheck)
        {
            // Declare and initialize
            CreatePaycheckResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the paycheck
            paycheck.UserId = userId;
            // Call the logic method
            response = await _paycheckLogic.CreatePaycheckAsync(paycheck);
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
                return Created("", new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    paycheckId = response.Id,
                    paycheckSplitIds = response.PaycheckSplitIds
                });
            }
        }

        [Authorize]
        [HttpGet("budget/{budgetId}")]
        public async Task<ActionResult> GetAllPaychecksAsync(int budgetId)
        {
            // Declare and initialize
            GetAllPaychecksResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget id in the request
            request.UserId = userId;
            request.EntityId = budgetId;
            // Call the logic method
            response = await _paycheckLogic.GetAllPaychecksAsync(request);
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
            // Return the OK response
            return Ok(new
            {
                status = response.HttpStatus,
                message = response.Message,
                paychecks = response.Paychecks
            });
        }

        [Authorize]
        [HttpGet("{paycheckId}")]
        public async Task<ActionResult> GetPaycheck(int paycheckId)
        {
            // Declare and initialize
            GetPaycheckResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the users id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Add the user id and the paycheck id to the request
            request.UserId = userId;
            request.EntityId = paycheckId;
            // Call the logic method
            response = await _paycheckLogic.GetPaycheckAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    paycheckId = paycheckId
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
                paycheck = response.Paycheck
            });
        }


        [Authorize]
        [HttpPut("{paycheckId}")]
        public async Task<ActionResult> UpdatePaycheckAsync(int paycheckId, UpdatePaycheckRequest paycheck)
        {
            // Declare and initialize
            UpdatePaycheckResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the paycheck id and the user id in the request
            paycheck.UserId = userId;
            paycheck.PaycheckId = paycheckId;
            // Call the logic method
            response = await _paycheckLogic.UpdatePaycheckAsync(paycheck);

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
                    paycheckId = response.Id,
                    paycheckSplitsIds = response.PaycheckSplitIds
                });
            }
        }


        [Authorize]
        [HttpDelete("{paycheckId}")]
        public async Task<ActionResult> DeletePaycheckAsync(int paycheckId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = paycheckId;
            // Call the logic method to delete the paycheck
            response = await _paycheckLogic.DeletePaycheckAsync(request);

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
