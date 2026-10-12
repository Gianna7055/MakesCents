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
    /// API controller for sinking fund envelopes
    /// </summary>
    [Route("api/sinking-fund-envelopes")]
    [ApiController]
    public class SinkingFundEnvelopeController : ControllerBase
    {
        // Class level variables
        private readonly SinkingFundEnvelopeLogic _sinkingFundEnvelopeLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="sinkingFundEnvelopeLogic"></param>
        public SinkingFundEnvelopeController(SinkingFundEnvelopeLogic sinkingFundEnvelopeLogic)
        {
            _sinkingFundEnvelopeLogic = sinkingFundEnvelopeLogic;
        }

        /// <summary>
        /// POST method to create a new sinking fund envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateSinkingFundEnvelopeAsync(CreateSinkingFundEnvelopeRequest envelope)
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
            response = await _sinkingFundEnvelopeLogic.CreateSinkingFundEnvelopeAsync(envelope);
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
        /// GET method to get a sinking fund envelope, including its transaction history
        /// </summary>
        /// <param name="sinkingFundEnvelopeId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{sinkingFundEnvelopeId}")]
        public async Task<ActionResult> GetSinkingFundEnvelopeAsync(int sinkingFundEnvelopeId)
        {
            // Declare and initialize
            GetSinkingFundEnvelopeDTOResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id in the request
            request.UserId = userId;
            request.EntityId = sinkingFundEnvelopeId;
            // Call the logic method
            response = await _sinkingFundEnvelopeLogic.GetSinkingFundEnvelopeAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    sinkingFundEnvelopeId = sinkingFundEnvelopeId
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
        /// PUT method to update a sinking fund envelope based on given fields
        /// </summary>
        /// <param name="sinkingFundEnvelopeId"></param>
        /// <param name="envelope"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{sinkingFundEnvelopeId}")]
        public async Task<ActionResult> UpdateSinkingFundEnvelopeAsync(int sinkingFundEnvelopeId, UpdateSinkingFundEnvelopeRequest envelope)
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
            envelope.SinkingFundEnvelopeId = sinkingFundEnvelopeId;
            // Call the logic method
            response = await _sinkingFundEnvelopeLogic.UpdateSinkingFundEnvelopeAsync(envelope);

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
        /// GET method to get the rollover envelopes that sweep into this sinking fund envelope
        /// </summary>
        /// <param name="sinkingFundEnvelopeId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{sinkingFundEnvelopeId}/rollover-dependents")]
        public async Task<ActionResult> GetRolloverDependentsAsync(int sinkingFundEnvelopeId)
        {
            // Declare and initialize
            GetSinkingFundDependentsResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = sinkingFundEnvelopeId;
            // Call the logic method
            response = await _sinkingFundEnvelopeLogic.GetRolloverDependentsAsync(request);

            // Check the status
            if (response.HttpStatus != 200)
            {
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the OK response
            return Ok(response);
        }

        /// <summary>
        /// DELETE method to delete a sinking fund envelope
        /// </summary>
        /// <param name="sinkingFundEnvelopeId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{sinkingFundEnvelopeId}")]
        public async Task<ActionResult> DeleteSinkingFundEnvelopeAsync(int sinkingFundEnvelopeId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = sinkingFundEnvelopeId;
            // Call the logic method to delete the envelope
            response = await _sinkingFundEnvelopeLogic.DeleteSinkingFundEnvelopeAsync(request);

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