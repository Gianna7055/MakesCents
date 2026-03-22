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
            if (budget.UserId == 0 || budget.Month == Month.Unknown || budget.Year == 0 || budget.BudgetName == null)
            {
                return new BaseIdResponse(400, "Missing information for budget creation");
            }
            // Check to make sure that month is okay
            if (!Enum.IsDefined(typeof(Month), budget.Month))
            {
                // Return an issue
                return new BaseIdResponse(400, "Month must be between 1 and 12");
            }
            // Check if the year is okay
            if (budget.Year < DateTime.Now.Year - 2 || budget.Year > DateTime.Now.Year + 1)
            {
                // Return the issue
                return new BaseIdResponse(400, "Incorrect year for budget creation");
            }
            // Make sure the budget name is between 1 and 50 characters
            if (!(budget.BudgetName.Length > 1) || !(budget.BudgetName.Length <= 50))
            {
                // Return the issue
                return new BaseIdResponse(400, "Budget name must be between 1 and 50 characters");
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
            if (budget.Year == 0 || budget.UserId == 0 || budget.Month == Month.Unknown)
            {
                // Return the not found status
                return new GetBudgetResponse(404, "Missing information to get budget");
            }
            // Call the DAO method
            response = await _budgetDAO.GetBudgetAsync(budget);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a budget based on a get budget request
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<GetBudgetResponse> GetBudgetAsync(BaseIdRequest budget)
        {
            // Declare and initialize
            GetBudgetResponse response;

            // Make sure the budget has a year, month, and user id
            if (budget.EntityId == 0 || budget.UserId == 0)
            {
                // Return the not found status
                return new GetBudgetResponse(404, "Missing information to get budget");
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
            if (budget.BudgetId == 0 || budget.UserId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Make sure the budget name is between 1 and 50 characters
            if (budget.BudgetName != null && (!(budget.BudgetName.Length > 1) || !(budget.BudgetName.Length <= 50)))
            {
                // Return the issue
                return new BaseIdResponse(400, "Budget name must be between 1 and 50 characters");
            }
            // Call the Create User method in the DAO
            response = await _budgetDAO.UpdateBudgetAsync(budget);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete an budget
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteBudgetAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for delete");
            }
            // Return a call the the DAO method
            return await _budgetDAO.DeleteBudgetAsync(request);
        }
    }
}
