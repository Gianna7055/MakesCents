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
                    paycheckId = response.PaycheckId,
                    paycheckSplitIds = response.PaycheckSplitIds
                });
            }
        }
    }
}
