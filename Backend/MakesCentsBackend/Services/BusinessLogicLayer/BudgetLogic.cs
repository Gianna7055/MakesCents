/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
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
        public BudgetLogic(BudgetDAO budgetDAO)
        {
            _budgetDAO = budgetDAO;
        }

        /// <summary>
        /// Logic method to create a new persistent budget (no month yet - see CreateBudgetMonthAsync)
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBudgetAsync(CreateBudgetRequest budget)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (budget.UserId == 0 || budget.BudgetName == null)
            {
                return new BaseIdResponse(400, "Missing information for budget creation");
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
        /// Logic method to create a new month under an existing persistent budget
        /// </summary>
        /// <param name="budgetMonth"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBudgetMonthAsync(CreateBudgetMonthRequest budgetMonth)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (budgetMonth.UserId == 0 || budgetMonth.BudgetId == 0 || budgetMonth.BudgetTemplateId == 0 || budgetMonth.Month == Month.Unknown || budgetMonth.Year == 0)
            {
                return new BaseIdResponse(400, "Missing information for budget month creation");
            }
            // Check to make sure that month is okay
            if (!Enum.IsDefined(typeof(Month), budgetMonth.Month))
            {
                // Return an issue
                return new BaseIdResponse(400, "Month must be between 1 and 12");
            }
            // Check if the year is okay
            if (budgetMonth.Year < DateTime.Now.Year - 2 || budgetMonth.Year > DateTime.Now.Year + 1)
            {
                // Return the issue
                return new BaseIdResponse(400, "Incorrect year for budget month creation");
            }
            // Call the DAO method
            response = await _budgetDAO.CreateBudgetMonthAsync(budgetMonth);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get every budget a user has access to (id and name only) - meant for
        /// use right after login, before any month or budget id is known yet.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetUserBudgetsResponse> GetUserBudgetsAsync(int userId)
        {
            // Make sure the user id was sent
            if (userId == 0)
            {
                return new GetUserBudgetsResponse(400, "Missing information to get budgets");
            }
            // Call the DAO method
            return await _budgetDAO.GetUserBudgetsAsync(userId);
        }

        /// <summary>
        /// Logic method to get a budget month based on a year/month/user lookup
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<GetBudgetMonthResponse> GetBudgetMonthAsync(GetBudgetRequest budget)
        {
            // Declare and initialize
            GetBudgetMonthResponse response;

            // Make sure the budget has a year, month, and user id
            if (budget.Year == 0 || budget.UserId == 0 || budget.BudgetId == 0 || budget.Month == Month.Unknown)
            {
                // Return the not found status
                return new GetBudgetMonthResponse(404, "Missing information to get budget");
            }
            // Call the DAO method
            response = await _budgetDAO.GetBudgetMonthAsync(budget);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a budget month based on its budget_month_id
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<GetBudgetMonthResponse> GetBudgetMonthAsync(BaseIdRequest budget)
        {
            // Declare and initialize
            GetBudgetMonthResponse response;

            // Make sure the budget has an id and user id
            if (budget.EntityId == 0 || budget.UserId == 0)
            {
                // Return the not found status
                return new GetBudgetMonthResponse(404, "Missing information to get budget");
            }
            // Call the DAO method
            response = await _budgetDAO.GetBudgetMonthAsync(budget);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get up to a year's worth of a user's past budget months
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetYearBudgetsResponse> GetYearBudgetsAsync(GetBudgetRequest request)
        {
            // Declare and initialize
            GetYearBudgetsResponse response;

            // Make sure the budget has a year, month, and user id
            if (request.Year == 0 || request.UserId == 0 || request.BudgetId == 0 || request.Month == Month.Unknown)
            {
                // Return the not found status
                return new GetYearBudgetsResponse(404, "Missing information to get budgets");
            }
            // Call the DAO method
            response = await _budgetDAO.GetYearBudgetsAsync(request);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to update a budget's persistent fields (currently just the name)
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateBudgetAsync(UpdateBudgetRequest budget)
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
            // Call the DAO method
            response = await _budgetDAO.UpdateBudgetAsync(budget);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to update a specific budget month's month/year (e.g. fixing a mistake)
        /// </summary>
        /// <param name="budgetMonth"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateBudgetMonthAsync(UpdateBudgetMonthRequest budgetMonth)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (budgetMonth.BudgetMonthId == 0 || budgetMonth.UserId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Check the month, if given
            if (budgetMonth.Month.HasValue && !Enum.IsDefined(typeof(Month), budgetMonth.Month.Value))
            {
                return new BaseIdResponse(400, "Month must be between 1 and 12");
            }
            // Check the year, if given
            if (budgetMonth.Year.HasValue && (budgetMonth.Year < 1900 || budgetMonth.Year > 2200))
            {
                return new BaseIdResponse(400, "Incorrect year for budget month");
            }
            // Call the DAO method
            response = await _budgetDAO.UpdateBudgetMonthAsync(budgetMonth);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete a persistent budget entirely (cascades every month, account,
        /// envelope category, paycheck, and planned expense it has)
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
            // Return a call to the DAO method
            return await _budgetDAO.DeleteBudgetAsync(request);
        }

        /// <summary>
        /// Logic method to delete a single month's budget (that month's transactions go with it -
        /// accounts, envelope categories, and paychecks are unaffected since they're persistent)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteBudgetMonthAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for delete");
            }
            // Return a call to the DAO method
            return await _budgetDAO.DeleteBudgetMonthAsync(request);
        }
    }
}