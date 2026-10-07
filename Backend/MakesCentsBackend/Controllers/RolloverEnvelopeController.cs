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
    /// API controller for rollover envelopes. Create/Update/Delete act on the persistent
    /// definition; Get is month-scoped, since a rollover envelope's planned/remaining amounts
    /// are specific to one budget month.
    /// </summary>
    [Route("api/rollover-envelopes")]
    [ApiController]
    public class RolloverEnvelopeController : ControllerBase
    {
        // Class level variables
        private readonly RolloverEnvelopeLogic _rolloverEnvelopeLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="rolloverEnvelopeLogic"></param>
        public RolloverEnvelopeController(RolloverEnvelopeLogic rolloverEnvelopeLogic)
        {
            _rolloverEnvelopeLogic = rolloverEnvelopeLogic;
        }

        /// <summary>
        /// POST method to create a new rollover envelope's persistent definition
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateRolloverEnvelopeAsync(CreateRolloverEnvelopeRequest envelope)
        {
            // Check to make sure the envelope is not null
            if (envelope == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new envelope");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the envelope
            envelope.UserId = userId;
            // Call the logic method
            response = await _rolloverEnvelopeLogic.CreateRolloverEnvelopeAsync(envelope);
            // Check the status
            if (response.HttpStatus != 201)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the success
            return Created("", response);
        }

        /// <summary>
        /// GET method to get a rollover envelope for a specific budget month, including its
        /// transaction history for that month
        /// </summary>
        /// <param name="rolloverEnvelopeId"></param>
        /// <param name="budgetMonthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{rolloverEnvelopeId}/budget-month/{budgetMonthId}")]
        public async Task<ActionResult> GetRolloverEnvelopeForMonthAsync(int rolloverEnvelopeId, int budgetMonthId)
        {
            // Declare and initialize
            GetRolloverEnvelopeDTOResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);
            GetRolloverEnvelopeMonthRequest request = new GetRolloverEnvelopeMonthRequest(userId, rolloverEnvelopeId, budgetMonthId);

            // Call the logic method
            response = await _rolloverEnvelopeLogic.GetRolloverEnvelopeForMonthAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    rolloverEnvelopeId = rolloverEnvelopeId,
                    budgetMonthId = budgetMonthId
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
        /// PUT method to update a rollover envelope's persistent definition fields
        /// </summary>
        /// <param name="rolloverEnvelopeId"></param>
        /// <param name="envelope"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{rolloverEnvelopeId}")]
        public async Task<ActionResult> UpdateRolloverEnvelopeAsync(int rolloverEnvelopeId, UpdateRolloverEnvelopeRequest envelope)
        {
            // Check to make sure the envelope is not null
            if (envelope == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update an envelope");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the envelope id and the user id in the request
            envelope.UserId = userId;
            envelope.RolloverEnvelopeId = rolloverEnvelopeId;
            // Call the logic method
            response = await _rolloverEnvelopeLogic.UpdateRolloverEnvelopeAsync(envelope);

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
        /// DELETE method to delete a rollover envelope's persistent definition (cascades every
        /// month it was ever activated for)
        /// </summary>
        /// <param name="rolloverEnvelopeId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{rolloverEnvelopeId}")]
        public async Task<ActionResult> DeleteRolloverEnvelopeAsync(int rolloverEnvelopeId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = rolloverEnvelopeId;
            // Call the logic method to delete the envelope
            response = await _rolloverEnvelopeLogic.DeleteRolloverEnvelopeAsync(request);

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