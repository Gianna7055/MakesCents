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
    /// API controller for transactions
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        // Class level variables
        private readonly TransactionLogic _transactionLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="transactionLogic"></param>
        public TransactionController(TransactionLogic transactionLogic)
        {
            _transactionLogic = transactionLogic;
        }

        [Authorize]
        [HttpGet("{budgetId}")]
        public async Task<ActionResult> GetAllTransactionsAsync(int budgetId)
        {
            // Declare or initialize
            GetAllTransactionsDTOResponse response;
            BaseGetRequest request = new BaseGetRequest();
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the user id and budget id in the request
            request.UserId = userId;
            request.EntityId = budgetId;
            // Call the logic method
            response = await _transactionLogic.GetAllTransactionsAsync(request);
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
                transactions = response.Transactions
            });
        }
    }
}
