/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;
using MakesCentsBackend.Models.Enums;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class BudgetLogic
    {
        // Class level variables
        private readonly BudgetDAO _budgetDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="budgetDAO"></param>
        /// <param name="mapper"></param>
        public BudgetLogic(BudgetDAO budgetDAO)
        {
            _budgetDAO = budgetDAO;
        }

        /// <summary>
        /// Logic method to create a new budget
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBudgetAsync(CreateBudgetRequest budget)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (budget.UserId == null || budget.Month == Month.Unknown || budget.Year == null || string.IsNullOrEmpty(budget.BudgetName))
            {
                return new BaseIdResponse(400, "Missing information for budget creation");
            }
            // Call the DAO method
            response = await _budgetDAO.CreateBudgetAsync(budget);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a budget based on a get budget request
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<GetBudgetResponse> GetBudgetAsync(GetBudgetRequest budget)
        {
            // Declare and initialize
            GetBudgetResponse response;

            // Make sure the budget has a year, month, and user id
            if (budget.Year == null || budget.UserId == null || budget.Month == Month.Unknown)
            {
                // Return the not found status
                return new GetBudgetResponse(404, "Budget not found");
            }
            // Call the DAO method
            response = await _budgetDAO.GetBudgetAsync(budget);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to update a budget based on provided fields
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateBudgetAsync(EditBudgetRequest budget)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (budget.BudgetId == null)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Call the Create User method in the DAO
            response = await _budgetDAO.UpdateBudgetAsync(budget);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete a budget
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteBudgetAsync(int budgetId, int userId)
        {
            // Return a call the the DAO method
            return await _budgetDAO.DeleteBudgetAsync(budgetId, userId);
        }
    }
}
