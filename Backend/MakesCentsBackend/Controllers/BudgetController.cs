/*
 * Gianna Ross
 * File Created: 11/16/2025
 * File Last Updated: 11/16/2025
 * Makes Cents - Budget Controller
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.BusinessLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MakesCentsBackend.Controllers
{
    /// <summary>
    /// API controller for budgets
    /// </summary>
    [Route("api/budgets")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        // Class level variables
        private BudgetLogic _budgetLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="budgetLogic"></param>
        public BudgetController(BudgetLogic budgetLogic)
        {
            _budgetLogic = budgetLogic;
        }


        public async Task<ActionResult> CreateBudgetAsync(BudgetDTO budget)
        {

        }
    }
}
