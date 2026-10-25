/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using MakesCentsBackend.Models;
using MySqlConnector;
using System.Data;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class BudgetDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        private readonly BudgetTemplateDAO _budgetTemplateDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        /// <param name="budgetTemplateDAO"></param>
        public BudgetDAO(MySqlConnection connection, AuthorizationService authService, BudgetTemplateDAO budgetTemplateDAO)
        {
            _connection = connection;
            _authService = authService;
            _budgetTemplateDAO = budgetTemplateDAO;
        }

        /// <summary>
        /// DAO method to create the persistent budget entity and link the creating user to it.
        /// Does not create a month - call CreateBudgetMonthAsync separately for that.
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBudgetAsync(CreateBudgetRequest budget)
        {
            // Declare and initialize
            int budgetId;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                try
                {
                    // Query for insert for the persistent budget
                    query = """
                        INSERT INTO budget (budget_name)
                        VALUES (@BudgetName);
                        SELECT LAST_INSERT_ID();
                        """;
                    budgetId = await _connection.QuerySingleAsync<int>(query, budget, dbTransaction);

                    // Link the creating user to the new budget
                    query = """
                        INSERT INTO budget_users (budget_id, user_id)
                        VALUES (@BudgetId, @UserId);
                        """;
                    await _connection.ExecuteAsync(query, new { BudgetId = budgetId, budget.UserId }, dbTransaction);
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new BaseIdResponse(500, $"{ex.Message}");
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Return the new id
            return new BaseIdResponse(201, "Budget created successfully", budgetId);
        }

        /// <summary>
        /// DAO method to create a new month under an existing persistent budget
        /// </summary>
        /// <param name="budgetMonth"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBudgetMonthAsync(CreateBudgetMonthRequest budgetMonth)
        {
            // Declare and initialize
            query = """
                INSERT INTO budget_month (budget_id, month_id, year)
                VALUES (@BudgetId, @Month, @Year);
                SELECT LAST_INSERT_ID();
                """;
            int budgetMonthId;
            BaseResponse applyResult;

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(budgetMonth.BudgetId, budgetMonth.UserId))
            {
                return new BaseIdResponse(403, "Budget does not belong to the current user");
            }

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction - creating the month and applying its required template
            // has to be one atomic operation, so a failed template application can't leave a
            // budget month sitting around with no template
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Execute the request and get the new id for the budget month
                try
                {
                    // Execute
                    budgetMonthId = await _connection.ExecuteScalarAsync<int>(query, budgetMonth, dbTransaction);
                }
                catch (MySqlException ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Check if the error is 1062
                    if (ex.Number == 1062)
                    {
                        // Check if the error is due to a non-unique month/year for this budget
                        if (ex.Message.Contains("'unique_budget_month'"))
                        {
                            return new BaseIdResponse(400, "Budget already exists for this month and year");
                        }
                    }
                    return new BaseIdResponse(500, $"{ex.Number}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new BaseIdResponse(500, $"{ex.Message}");
                }

                // Apply the required template to the month just created, in the same
                // transaction - the ownership check inside this call correctly sees the
                // not-yet-committed budget_month row, since it's the same transaction
                applyResult = await _budgetTemplateDAO.ApplyBudgetTemplateWithinTransactionAsync(new ApplyBudgetTemplateRequest
                {
                    UserId = budgetMonth.UserId,
                    BudgetTemplateId = budgetMonth.BudgetTemplateId,
                    BudgetMonthId = budgetMonthId
                }, dbTransaction);
                if (applyResult.HttpStatus != 200)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new BaseIdResponse(applyResult.HttpStatus, applyResult.Message);
                }

                // Commit the transaction
                dbTransaction.Commit();
            }

            // Return the new id
            return new BaseIdResponse(201, "Budget month created successfully", budgetMonthId);
        }

        /// <summary>
        /// DAO method to get every budget a user has access to - just the id and name, since
        /// this is meant to be usable before any month or budget id is known (e.g. right after login).
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetUserBudgetsResponse> GetUserBudgetsAsync(int userId)
        {
            // Declare and initialize
            GetUserBudgetsResponse response = new GetUserBudgetsResponse();

            // Set up the query to get every budget this user belongs to
            query = """
                SELECT 
                    budget.budget_id AS BudgetId,
                    budget.budget_name AS BudgetName
                FROM budget
                INNER JOIN budget_users ON budget.budget_id = budget_users.budget_id
                WHERE budget_users.user_id = @UserId
                ORDER BY budget.budget_name;
                """;
            // Execute the query
            response.Budgets = (await _connection.QueryAsync<SummaryBudgetResponse>(query, new { UserId = userId })).ToList();

            // Set the status and message for the response
            response.HttpStatus = 200;
            response.Message = "Budgets found";
            // Return the response
            return response;
        }

        /// <summary>
        /// Get a budget month - with envelope categories and envelopes - based on the
        /// userId, month, and year
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<GetBudgetMonthResponse> GetBudgetMonthAsync(GetBudgetRequest budget)
        {
            // Declare and initialize
            GetBudgetMonthResponse budgetResponse = new GetBudgetMonthResponse();
            GetBudgetMonthDTOModel? budgetDTO;

            // Set up the query to get the budget for this month
            query = """
                SELECT 
                    budget.budget_id AS BudgetId,
                    budget_month.budget_month_id AS BudgetMonthId,
                    budget_month.month_id AS Month,
                    budget_month.year AS Year,
                    budget.budget_name AS BudgetName,
                    budget_month.budget_template_id AS BudgetTemplateId
                FROM budget_month
                INNER JOIN budget ON budget_month.budget_id = budget.budget_id
                INNER JOIN budget_users ON budget.budget_id = budget_users.budget_id
                WHERE budget_users.user_id = @UserId
                    AND budget.budget_id = @BudgetId
                    AND budget_month.year = @Year
                    AND budget_month.month_id = @Month;
                """;
            // Execute the query
            budgetDTO = await _connection.QuerySingleOrDefaultAsync<GetBudgetMonthDTOModel>(query, budget);
            // Make sure the budget is not null
            if (budgetDTO == null)
            {
                return new GetBudgetMonthResponse(404, "Budget not found");
            }
            // Set the dto for the response
            budgetResponse.GetBudgetMonthDTO = budgetDTO;
            // Get the envelope categories (with their envelopes) for this budget/month
            budgetResponse.GetBudgetMonthDTO.EnvelopeCategories = await GetEnvelopeCategoriesWithEnvelopesAsync(budgetDTO.BudgetMonthId);
            // Set the status and message for the budget response
            budgetResponse.HttpStatus = 200;
            budgetResponse.Message = "Budget found";
            // Return the budget 
            return budgetResponse;
        }

        /// <summary>
        /// Get a budget month - with envelope categories and envelopes - based on its budget_month_id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetBudgetMonthResponse> GetBudgetMonthAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetBudgetMonthResponse budgetResponse = new GetBudgetMonthResponse();
            GetBudgetMonthDTOModel? budgetDTO;

            // Make sure this budget month belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetMonthAsync(request.EntityId, request.UserId))
            {
                return new GetBudgetMonthResponse(403, "Budget does not belong to the current user");
            }
            // Set up the query to get the budget
            query = """
                SELECT 
                    budget.budget_id AS BudgetId,
                    budget_month.budget_month_id AS BudgetMonthId,
                    budget_month.month_id AS Month,
                    budget_month.year AS Year,
                    budget.budget_name AS BudgetName,
                    budget_month.budget_template_id AS BudgetTemplateId
                FROM budget_month
                INNER JOIN budget ON budget_month.budget_id = budget.budget_id
                WHERE budget_month.budget_month_id = @BudgetMonthId;
                """;
            // Execute the query
            budgetDTO = await _connection.QuerySingleOrDefaultAsync<GetBudgetMonthDTOModel>(query, new { BudgetMonthId = request.EntityId });
            // Make sure the budget is not null
            if (budgetDTO == null)
            {
                return new GetBudgetMonthResponse(404, "Budget not found");
            }
            // Set the dto for the response
            budgetResponse.GetBudgetMonthDTO = budgetDTO;
            // Get the envelope categories (with their envelopes) for this budget/month
            budgetResponse.GetBudgetMonthDTO.EnvelopeCategories = await GetEnvelopeCategoriesWithEnvelopesAsync(budgetDTO.BudgetMonthId);
            // Set the status and message for the budget response
            budgetResponse.HttpStatus = 200;
            budgetResponse.Message = "Budget found";
            // Return the budget 
            return budgetResponse;
        }

        /// <summary>
        /// Shared helper: reads a budget month's envelope categories, then reads sinking fund
        /// envelopes (persistent) and rollover envelopes (for the given month) as one merged,
        /// alphabetized list per category - each envelope carries IsSinkingFund so the frontend
        /// knows which table/endpoint its EnvelopeId belongs to. budget_id is derived via a join
        /// to budget_month rather than taken as a separate parameter.
        /// </summary>
        /// <param name="budgetMonthId"></param>
        /// <returns></returns>
        private async Task<List<SummaryEnvelopeCategoryResponse>> GetEnvelopeCategoriesWithEnvelopesAsync(int budgetMonthId)
        {
            // Declare and initialize
            List<SummaryEnvelopeCategoryResponse> envelopeCategoryResponses;
            List<SummaryEnvelopeResponse> sinkingFundResponses;
            List<SummaryEnvelopeResponse> rolloverResponses;
            Dictionary<int, SummaryEnvelopeCategoryResponse> envelopeCategoryLookup;

            // Set up the query to get the envelope categories
            query = """
                SELECT 
                    envelope_category.envelope_category_id as EnvelopeCategoryId,
                    envelope_category.budget_id as BudgetId,
                    envelope_category.envelope_category_name as EnvelopeCategoryName
                FROM envelope_category
                INNER JOIN budget_month ON budget_month.budget_id = envelope_category.budget_id
                WHERE budget_month.budget_month_id = @BudgetMonthId;
                """;
            // Read the envelope categories
            envelopeCategoryResponses = (await _connection.QueryAsync<SummaryEnvelopeCategoryResponse>(query, new { BudgetMonthId = budgetMonthId })).ToList();

            // Set up the query for reading the sinking fund envelopes (persistent, no month context needed)
            query = """
                SELECT 
                    sinking_fund_envelope.sinking_fund_envelope_id AS EnvelopeId,
                    sinking_fund_envelope.envelope_category_id AS EnvelopeCategoryId,
                    sinking_fund_envelope.envelope_name AS EnvelopeName,
                    sinking_fund_envelope.remaining_amount AS RemainingAmount,
                    true AS IsSinkingFund
                FROM sinking_fund_envelope
                INNER JOIN envelope_category ON envelope_category.envelope_category_id = sinking_fund_envelope.envelope_category_id
                INNER JOIN budget_month ON budget_month.budget_id = envelope_category.budget_id
                WHERE budget_month.budget_month_id = @BudgetMonthId;
                """;
            // Read the sinking fund envelopes
            sinkingFundResponses = (await _connection.QueryAsync<SummaryEnvelopeResponse>(query, new { BudgetMonthId = budgetMonthId })).ToList();

            // Set up the query for reading the rollover envelopes, joined to this month's instance (if activated)
            query = """
                SELECT 
                    rollover_envelope.rollover_envelope_id AS EnvelopeId,
                    rollover_envelope.envelope_category_id AS EnvelopeCategoryId,
                    rollover_envelope.envelope_name AS EnvelopeName,
                    rollover_envelope_month.remaining_amount AS RemainingAmount,
                    false AS IsSinkingFund
                FROM rollover_envelope
                INNER JOIN envelope_category ON envelope_category.envelope_category_id = rollover_envelope.envelope_category_id
                INNER JOIN budget_month ON budget_month.budget_id = envelope_category.budget_id
                LEFT JOIN rollover_envelope_month 
                    ON rollover_envelope_month.rollover_envelope_id = rollover_envelope.rollover_envelope_id
                    AND rollover_envelope_month.budget_month_id = @BudgetMonthId
                WHERE budget_month.budget_month_id = @BudgetMonthId;
                """;
            // Read the rollover envelopes
            rolloverResponses = (await _connection.QueryAsync<SummaryEnvelopeResponse>(query, new { BudgetMonthId = budgetMonthId })).ToList();

            // Create a dictionary for the envelope categories
            envelopeCategoryLookup = envelopeCategoryResponses.ToDictionary(category => category.EnvelopeCategoryId);
            // Loop through both envelope lists together to put them in the correct categories
            foreach (SummaryEnvelopeResponse envelope in sinkingFundResponses.Concat(rolloverResponses))
            {
                if (envelopeCategoryLookup.TryGetValue(envelope.EnvelopeCategoryId, out SummaryEnvelopeCategoryResponse? category))
                {
                    category.Envelopes.Add(envelope);
                }
            }
            // Sort each category's envelopes alphabetically once, here, so every consumer gets them pre-sorted
            foreach (SummaryEnvelopeCategoryResponse category in envelopeCategoryResponses)
            {
                category.Envelopes = category.Envelopes
                    .OrderBy(envelope => envelope.EnvelopeName, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            // Return the merged list
            return envelopeCategoryResponses;
        }

        /// <summary>
        /// Get up to a year's worth of this user's past budget months, anchored at the given month/year
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetYearBudgetsResponse> GetYearBudgetsAsync(GetBudgetRequest request)
        {
            // Declare and initialize
            GetYearBudgetsResponse response = new GetYearBudgetsResponse();

            // Set up the query to get the budgets
            query = """
                SELECT 
                    budget.budget_id as BudgetId,
                    budget_month.budget_month_id as BudgetMonthId,
                    budget_month.month_id as Month,
                    budget_month.year as Year,
                    budget.budget_name as BudgetName,
                    budget_month.budget_template_id as BudgetTemplateId
                FROM budget_month
                INNER JOIN budget ON budget_month.budget_id = budget.budget_id
                INNER JOIN budget_users ON budget.budget_id = budget_users.budget_id
                WHERE budget_users.user_id = @UserId
                    AND budget.budget_id = @BudgetId
                    AND ((budget_month.year * 12) + (budget_month.month_id - 1))
                        BETWEEN (((@Year * 12) + (@Month - 1)) - 11)
                        AND ((@Year * 12) + (@Month - 1))
                ORDER BY budget_month.year, budget_month.month_id;
                """;
            // Execute the query
            response.Budgets = (await _connection.QueryAsync<GetYearBudgetDTOModel>(query, request)).ToList();
            // Set the status and message for the budget response
            response.HttpStatus = 200;
            response.Message = "Budgets found";
            // Return the budget 
            return response;
        }

        /// <summary>
        /// DAO method to update a budget's persistent fields (currently just the name)
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateBudgetAsync(UpdateBudgetRequest budget)
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
                """;
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, budget);

            // Make sure the row was affected
            if (rowsAffected == 1)
            {
                return new BaseIdResponse(200, "Update was successful", budget.BudgetId);
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
        /// DAO method to update a specific budget month's month/year
        /// (e.g. correcting a month that was created under the wrong month/year)
        /// </summary>
        /// <param name="budgetMonth"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateBudgetMonthAsync(UpdateBudgetMonthRequest budgetMonth)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure this budget month belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetMonthAsync(budgetMonth.BudgetMonthId, budgetMonth.UserId))
            {
                return new BaseIdResponse(403, "Budget does not belong to the current user");
            }
            // Check each nullable field to see if update is necessary
            // (written out explicitly rather than via reflection, since Month's property name
            // doesn't Underscore() into its actual column name, month_id)
            if (budgetMonth.Month != null)
            {
                updates.Add("month_id = @Month");
            }
            if (budgetMonth.Year != null)
            {
                updates.Add("year = @Year");
            }
            // If no fields to update, return early
            if (updates.Count == 0)
            {
                return new BaseIdResponse(400, "No fields to update");
            }
            // Assemble the query
            query = $"""
                UPDATE budget_month 
                SET {string.Join(", ", updates)}
                WHERE budget_month_id = @BudgetMonthId
                """;
            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, budgetMonth);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique month/year for this budget
                    if (ex.Message.Contains("'unique_budget_month'"))
                    {
                        return new BaseIdResponse(400, "Budget already exists for this month and year");
                    }
                }
                return new BaseIdResponse(500, $"{ex.Number}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Return the issue
                return new BaseIdResponse(500, $"{ex.Message}");
            }

            // Make sure the row was affected
            if (rowsAffected == 1)
            {
                return new BaseIdResponse(200, "Update was successful", budgetMonth.BudgetMonthId);
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
        /// DAO method to delete the persistent budget entity. Cascades to remove every month,
        /// every account, every envelope category (and its envelopes), every paycheck, and
        /// every planned expense that belongs to it.
        /// </summary>
        /// <param name="request"></param>
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
                // Return that the budget was not found
                return new BaseResponse(404, "Budget not found");
            }
            // Return the success
            return new BaseResponse(200, "Budget deleted successfully");
        }

        /// <summary>
        /// DAO method to delete a single month's budget. Only removes that month's budget_month
        /// row (and its transactions, via cascade) - accounts, envelope categories, and paychecks
        /// belong to the persistent budget and are shared across all of its months, so they
        /// survive the deletion of any single month.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteBudgetMonthAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM budget_month 
                WHERE budget_month_id = @BudgetMonthId
                """;
            int rowsAffected;

            // Make sure this budget month belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetMonthAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Budget does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { BudgetMonthId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the budget was not found
                return new BaseResponse(404, "Budget not found");
            }
            // Return the success
            return new BaseResponse(200, "Budget deleted successfully");
        }
    }
}