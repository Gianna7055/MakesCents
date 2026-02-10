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
    /// API controller for envelopes
    /// </summary>
    /// <remarks>
    /// Parameterized constructor to bring in DI variables
    /// </remarks>
    /// <param name="envelopeLogic"></param>
    [Route("api/envelopes")]
    [ApiController]
    public class EnvelopeController : ControllerBase
    {
        // Class level variables
        private readonly EnvelopeLogic _envelopeLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="envelopeLogic"></param>
        public EnvelopeController(EnvelopeLogic envelopeLogic)
        {
            _envelopeLogic = envelopeLogic;
        }

        /// <summary>
        /// POST method to create a new envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateEnvelopeAsync(CreateEnvelopeRequest envelope)
        {
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the budget
            envelope.UserId = userId;
            // Call the logic method
            response = await _envelopeLogic.CreateEnvelopeAsync(envelope);
            // Check the status
            if (response.HttpStatus == 400)
            {
                // Return the bad request
                return BadRequest(response.Message);
            }
            else if (response.HttpStatus == 403)
            {
                // Return the forbidden response
                return Forbid(response.Message);
            }
            // Return the success
            return Created("", new
            {
                status = response.HttpStatus,
                message = response.Message,
                envelopeId = response.Id
            });
        }

        /*
        [Authorize]
        [HttpGet("{envelopeCategoryId}")]
        public async Task<ActionResult> GetEnvelopeAsync(int envelopeId)
        {

        }
        */
    }
}
