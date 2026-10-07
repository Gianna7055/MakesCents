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
    /// API controller for paychecks. Paychecks are forecast-only - see EnvelopeFundingController
    /// for how envelopes actually get funded.
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
            // Check to make sure the paycheck is not null
            if (paycheck == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new paycheck");
            }
            // Declare and initialize
            BaseIdResponse response;
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
            else
            {
                // Return the success
                return Created("", response);
            }
        }

        /// <summary>
        /// HTTP GET method to get every paycheck for a budget
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
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
            return Ok(response);
        }

        /// <summary>
        /// HTTP GET method to get a single paycheck
        /// </summary>
        /// <param name="paycheckId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{paycheckId}")]
        public async Task<ActionResult> GetPaycheckAsync(int paycheckId)
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
            return Ok(response);
        }

        /// <summary>
        /// HTTP PUT method to update a paycheck
        /// </summary>
        /// <param name="paycheckId"></param>
        /// <param name="paycheck"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{paycheckId}")]
        public async Task<ActionResult> UpdatePaycheckAsync(int paycheckId, UpdatePaycheckRequest paycheck)
        {
            // Check to make sure the paycheck is not null
            if (paycheck == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update a paycheck");
            }
            // Declare and initialize
            BaseIdResponse response;
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
                return Ok(response);
            }
        }

        /// <summary>
        /// HTTP DELETE method to delete a paycheck
        /// </summary>
        /// <param name="paycheckId"></param>
        /// <returns></returns>
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
                return Ok(response);
            }
        }
    }
}