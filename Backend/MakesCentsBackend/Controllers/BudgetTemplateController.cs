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
    /// API controller for budget templates - named, reusable presets of envelope funding
    /// amounts that can be applied to a budget month to bulk-create its funding queue.
    /// </summary>
    [Route("api/budget-templates")]
    [ApiController]
    public class BudgetTemplateController : ControllerBase
    {
        // Class level variables
        private readonly BudgetTemplateLogic _budgetTemplateLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="budgetTemplateLogic"></param>
        public BudgetTemplateController(BudgetTemplateLogic budgetTemplateLogic)
        {
            _budgetTemplateLogic = budgetTemplateLogic;
        }

        /// <summary>
        /// HTTP POST method to create a new budget template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateBudgetTemplateAsync(CreateBudgetTemplateRequest template)
        {
            // Check to make sure the template is not null
            if (template == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to create a new template");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id for the template
            template.UserId = userId;
            // Call the logic method
            response = await _budgetTemplateLogic.CreateBudgetTemplateAsync(template);
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
        /// HTTP GET method to get every template for a budget
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("budget/{budgetId}")]
        public async Task<ActionResult> GetAllBudgetTemplatesAsync(int budgetId)
        {
            // Declare and initialize
            GetAllBudgetTemplatesResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget id in the request
            request.UserId = userId;
            request.EntityId = budgetId;
            // Call the logic method
            response = await _budgetTemplateLogic.GetAllBudgetTemplatesAsync(request);
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
        /// HTTP GET method to get a single budget template, including its envelope entries
        /// </summary>
        /// <param name="budgetTemplateId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{budgetTemplateId}")]
        public async Task<ActionResult> GetBudgetTemplateAsync(int budgetTemplateId)
        {
            // Declare and initialize
            GetBudgetTemplateResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id in the request
            request.UserId = userId;
            request.EntityId = budgetTemplateId;
            // Call the logic method
            response = await _budgetTemplateLogic.GetBudgetTemplateAsync(request);
            // Check if the response came back as not found
            if (response.HttpStatus == 404)
            {
                return NotFound(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    budgetTemplateId = budgetTemplateId
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
        /// HTTP PUT method to rename a budget template
        /// </summary>
        /// <param name="budgetTemplateId"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{budgetTemplateId}")]
        public async Task<ActionResult> UpdateBudgetTemplateAsync(int budgetTemplateId, EditBudgetTemplateRequest template)
        {
            // Check to make sure the template is not null
            if (template == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to update a template");
            }
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the template id and the user id in the request
            template.UserId = userId;
            template.BudgetTemplateId = budgetTemplateId;
            // Call the logic method
            response = await _budgetTemplateLogic.UpdateBudgetTemplateAsync(template);

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
        /// HTTP DELETE method to delete a budget template
        /// </summary>
        /// <param name="budgetTemplateId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{budgetTemplateId}")]
        public async Task<ActionResult> DeleteBudgetTemplateAsync(int budgetTemplateId)
        {
            // Declare and initialize
            BaseResponse response;
            BaseIdRequest request = new BaseIdRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and the entity id in the request
            request.UserId = userId;
            request.EntityId = budgetTemplateId;
            // Call the logic method to delete the template
            response = await _budgetTemplateLogic.DeleteBudgetTemplateAsync(request);

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
        /// HTTP PUT method to set (create/reorder/update/remove all at once) a template's
        /// envelope entries
        /// </summary>
        /// <param name="budgetTemplateId"></param>
        /// <param name="templateEnvelopes"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{budgetTemplateId}/envelopes")]
        public async Task<ActionResult> SetBudgetTemplateEnvelopesAsync(int budgetTemplateId, SetBudgetTemplateEnvelopesRequest templateEnvelopes)
        {
            // Check to make sure the request is not null
            if (templateEnvelopes == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong with the data transfer to set the template's envelopes");
            }
            // Declare and initialize
            GetBudgetTemplateResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and template id for the request
            templateEnvelopes.UserId = userId;
            templateEnvelopes.BudgetTemplateId = budgetTemplateId;
            // Call the logic method
            response = await _budgetTemplateLogic.SetBudgetTemplateEnvelopesAsync(templateEnvelopes);
            // Check the status
            if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the OK response
            return Ok(response);
        }

        /// <summary>
        /// HTTP POST method to apply a template to a budget month, fully replacing that
        /// month's funding queue with the template's entries
        /// </summary>
        /// <param name="budgetTemplateId"></param>
        /// <param name="budgetMonthId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{budgetTemplateId}/apply/budget-month/{budgetMonthId}")]
        public async Task<ActionResult> ApplyBudgetTemplateAsync(int budgetTemplateId, int budgetMonthId)
        {
            // Declare and initialize
            GetEnvelopeFundingQueueResponse response;
            ApplyBudgetTemplateRequest request = new ApplyBudgetTemplateRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id, template id, and budget month id in the request
            request.UserId = userId;
            request.BudgetTemplateId = budgetTemplateId;
            request.BudgetMonthId = budgetMonthId;
            // Call the logic method
            response = await _budgetTemplateLogic.ApplyBudgetTemplateAsync(request);
            // Check the status
            if (response.HttpStatus != 200)
            {
                // Return an issue
                return StatusCode(response.HttpStatus, response.Message);
            }
            // Return the OK response
            return Ok(response);
        }
    }
}