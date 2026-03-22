/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/69774450-b594-8330-963c-0342dd63eac2
 */
using Dapper;
using MakesCentsBackend.Models;
using MySqlConnector;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class BudgetDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public BudgetDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create a budget
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBudgetAsync(CreateBudgetRequest budget)
        {
            // Declare and initialize
            query = """
                INSERT INTO budget (user_id, month_id, year, budget_name)
                VALUES (@UserId, @Month, @Year, @BudgetName);
                SELECT LAST_INSERT_ID();
                """;
            int budgetId;

            // Use dapper to execute the request and get the budgets new id
            try
            {
                // Execute the async scalar method using dapper
                budgetId = await _connection.ExecuteScalarAsync<int>(query, budget);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_user_budget'"))
                    {
                        return new BaseIdResponse(400, "Budget already exists for this user, month, and year");
                    }
                }
                return new BaseIdResponse(500, $"{ex.Number}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Return the issue
                return new BaseIdResponse(500, $"{ex.Message}");
            }
            // Return the new id
            return new BaseIdResponse(201, "Budget created successfully", budgetId);
        }

        /// <summary>
        /// Get the summary of a budget with envelope categories and envelopes
        /// Sources: https://chatgpt.com/c/69774450-b594-8330-963c-0342dd63eac2
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<GetBudgetResponse> GetBudgetAsync(GetBudgetRequest budget)
        {
            // Declare and initialize
            GetBudgetResponse budgetResponse = new GetBudgetResponse();
            GetBudgetDTOModel? budgetDTO;
            List<SummaryEnvelopeCategoryResponse> envelopeCategoryResponses;
            List<SummaryEnvelopeResponse> envelopeResponses;
            Dictionary<int, SummaryEnvelopeCategoryResponse> envelopeCategoryLookup;
            int budgetId = 0;

            // Set up the query to get the budget
            query = """
                SELECT 
                    budget.budget_id as BudgetId,
                    budget.user_id as UserId,
                    budget.month_id as Month,
                    budget.year as Year,
                    budget.budget_name as BudgetName
                FROM budget
                WHERE budget.user_id = @UserId
                    AND budget.year = @Year
                    AND budget.month_id = @Month;
                """;
            // Execute the query
            budgetDTO = await _connection.QuerySingleOrDefaultAsync<GetBudgetDTOModel>(query, budget);
            // Make sure the budget is not null
            if (budgetDTO == null)
            {
                return new GetBudgetResponse(404, "Budget not found");
            }
            // Set the dto for the response
            budgetResponse.GetBudgetDTO = budgetDTO;
            // Get the budget id
            budgetId = budgetResponse.GetBudgetDTO.BudgetId;
            // Set up the query to get the envelope categories
            query = """
                SELECT 
                    envelope_category.envelope_category_id as EnvelopeCategoryId,
                    envelope_category.budget_id as BudgetId,
                    envelope_category.envelope_category_name as EnvelopeCategoryName
                FROM envelope_category
                WHERE envelope_category.budget_id = @BudgetId;
                """;
            // Read the envelope categories
            envelopeCategoryResponses = (await _connection.QueryAsync<SummaryEnvelopeCategoryResponse>(query, new { BudgetId = budgetId })).ToList();
            // Set up the query for reading the envelopes
            query = """
                SELECT 
                    envelope.envelope_id as EnvelopeId,
                    envelope.envelope_name as EnvelopeName,
                    envelope.remaining_amount as RemainingAmount,
                    envelope.envelope_category_id as EnvelopeCategoryId
                FROM envelope
                JOIN envelope_category 
                    ON envelope_category.envelope_category_id = envelope.envelope_category_id
                WHERE envelope_category.budget_id = @BudgetId;
                """;
            // Read the envelopes
            envelopeResponses = (await _connection.QueryAsync<SummaryEnvelopeResponse>(query, new { BudgetId = budgetId })).ToList();
            // Create a dictionary for the envelope categories
            envelopeCategoryLookup = envelopeCategoryResponses.ToDictionary(category => category.EnvelopeCategoryId);
            // Loop through the envelopes to put them in the correct categories
            foreach (SummaryEnvelopeResponse envelope in envelopeResponses)
            {
                // Get the envelope category for each envelope
                if (envelopeCategoryLookup.TryGetValue(envelope.EnvelopeCategoryId, out SummaryEnvelopeCategoryResponse? singleEnvelopeCategory))
                {
                    // Add the envelope to the envelope category
                    singleEnvelopeCategory.Envelopes.Add(envelope);
                }
            }
            // Set the envelope category list of the budget to the existing list
            budgetResponse.GetBudgetDTO.EnvelopeCategories = envelopeCategoryResponses;
            // Set the status and message for the budget response
            budgetResponse.HttpStatus = 200;
            budgetResponse.Message = "Budget found";
            // Return the budget 
            return budgetResponse;
        }

        /// <summary>
        /// Get the summary of a budget with envelope categories and envelopes
        /// Sources: https://chatgpt.com/c/69774450-b594-8330-963c-0342dd63eac2
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<GetBudgetResponse> GetBudgetAsync(BaseIdRequest budget)
        {
            // Declare and initialize
            GetBudgetResponse budgetResponse = new GetBudgetResponse();
            GetBudgetDTOModel? budgetDTO;
            List<SummaryEnvelopeCategoryResponse> envelopeCategoryResponses;
            List<SummaryEnvelopeResponse> envelopeResponses;
            Dictionary<int, SummaryEnvelopeCategoryResponse> envelopeCategoryLookup;
            int budgetId = 0;

            // Set up the query to get the budget
            query = """
                SELECT 
                    budget.budget_id as BudgetId,
                    budget.user_id as UserId,
                    budget.month_id as Month,
                    budget.year as Year,
                    budget.budget_name as BudgetName
                FROM budget
                WHERE budget.user_id = @UserId
                    AND budget.budget_id = @BudgetId;
                """;
            // Execute the query
            budgetDTO = await _connection.QuerySingleOrDefaultAsync<GetBudgetDTOModel>(query, new { UserId = budget.UserId, BudgetId = budget.EntityId });
            // Make sure the budget is not null
            if (budgetDTO == null)
            {
                return new GetBudgetResponse(404, "Budget not found");
            }
            // Set the dto for the response
            budgetResponse.GetBudgetDTO = budgetDTO;
            // Set up the query to get the envelope categories
            query = """
                SELECT 
                    envelope_category.envelope_category_id as EnvelopeCategoryId,
                    envelope_category.budget_id as BudgetId,
                    envelope_category.envelope_category_name as EnvelopeCategoryName
                FROM envelope_category
                WHERE envelope_category.budget_id = @BudgetId;
                """;
            // Read the envelope categories
            envelopeCategoryResponses = (await _connection.QueryAsync<SummaryEnvelopeCategoryResponse>(query, new { BudgetId = budgetId })).ToList();
            // Set up the query for reading the envelopes
            query = """
                SELECT 
                    envelope.envelope_id as EnvelopeId,
                    envelope.envelope_name as EnvelopeName,
                    envelope.remaining_amount as RemainingAmount,
                    envelope.envelope_category_id as EnvelopeCategoryId
                FROM envelope
                JOIN envelope_category 
                    ON envelope_category.envelope_category_id = envelope.envelope_category_id
                WHERE envelope_category.budget_id = @BudgetId;
                """;
            // Read the envelopes
            envelopeResponses = (await _connection.QueryAsync<SummaryEnvelopeResponse>(query, new { BudgetId = budgetId })).ToList();
            // Create a dictionary for the envelope categories
            envelopeCategoryLookup = envelopeCategoryResponses.ToDictionary(category => category.EnvelopeCategoryId);
            // Loop through the envelopes to put them in the correct categories
            foreach (SummaryEnvelopeResponse envelope in envelopeResponses)
            {
                // Get the envelope category for each envelope
                if (envelopeCategoryLookup.TryGetValue(envelope.EnvelopeCategoryId, out SummaryEnvelopeCategoryResponse? singleEnvelopeCategory))
                {
                    // Add the envelope to the envelope category
                    singleEnvelopeCategory.Envelopes.Add(envelope);
                }
            }
            // Set the envelope category list of the budget to the existing list
            budgetResponse.GetBudgetDTO.EnvelopeCategories = envelopeCategoryResponses;
            // Set the status and message for the budget response
            budgetResponse.HttpStatus = 200;
            budgetResponse.Message = "Budget found";
            // Return the budget 
            return budgetResponse;
        }

        /// <summary>
        /// DAO method to update a budget based on given fields
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateBudgetAsync(EditBudgetRequest budget)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(budget.BudgetId, budget.UserId))
            {
                return new BaseIdResponse(403, "Budget does not belong to the current user");
            }
            // Check each nullable field to see if update is necessary
            if (budget.BudgetName != null)
            {
                updates.Add("budget_name = @BudgetName");
            }
            // If no fields to update, return early
            if (updates.Count == 0)
            {
                return new BaseIdResponse(400, "No fields to update");
            }
            // Assemble the query
            query = $"""
                UPDATE budget 
                SET
                {string.Join(", ", updates)}
                WHERE budget_id = @BudgetId
                AND user_Id = @UserId
                """;
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, budget);

            // Make sure the row was affected
            if (rowsAffected == 1)
            {
                if (budget.BudgetId is int budgetId)
                {
                    return new BaseIdResponse(200, "Update was successful", budgetId);
                }
                else
                {
                    return new BaseIdResponse(400, "An error occurred");
                }
            }
            else if (rowsAffected == 0)
            {
                return new BaseIdResponse(404, "Budget not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
            }
        }

        /// <summary>
        /// DAO method to delete a budget
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteBudgetAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM budget 
                WHERE budget_id = @BudgetId
                """;
            int rowsAffected;

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Budget does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { BudgetId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the user was not found
                return new BaseResponse(404, "Budget not found");
            }
            // Return the success
            return new BaseResponse(200, "Budget deleted successfully");
        }
    }
}
