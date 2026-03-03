/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using MakesCentsBackend.Models;
using MySqlConnector;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    /// <summary>
    /// Data access object for envelope categories
    /// </summary>
    public class EnvelopeCategoryDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public EnvelopeCategoryDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create an envelope category
        /// </summary>
        /// <param name="envelopeCategory"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateEnvelopeCategoryAsync(CreateEnvelopeCategoryRequest envelopeCategory)
        {
            // Declare and initialize
            query = """
                INSERT INTO envelope_category (budget_id, envelope_category_name)
                VALUES (@BudgetId, @EnvelopeCategoryName);
                SELECT LAST_INSERT_ID();
                """;
            int envelopeCategoryId;

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(envelopeCategory.BudgetId, envelopeCategory.UserId))
            {
                return new BaseIdResponse(403, "Budget does not belong to the current user.");
            }
            // Execute the request and get the new id for the envelope category
            try
            {
                // Execute the async scalar method
                envelopeCategoryId = await _connection.ExecuteScalarAsync<int>(query, envelopeCategory);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_category_name'"))
                    {
                        return new BaseIdResponse(400, "Category name already exists in this budget");
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
            return new BaseIdResponse(201, "Envelope category created successfully", envelopeCategoryId);
        }


        public async Task<GetAllEnvelopeCategoriesResponse> GetAllEnvelopeCategoriesAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllEnvelopeCategoriesResponse allEnvelopeCategoriesResponse = new GetAllEnvelopeCategoriesResponse();
            List<SummaryEnvelopeCategoryResponse> envelopeCategoryResponses;
            List<SummaryEnvelopeResponse> envelopeResponses;
            Dictionary<int, SummaryEnvelopeCategoryResponse> envelopeCategoryLookup;

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(request.EntityId, request.UserId))
            {
                return new GetAllEnvelopeCategoriesResponse(403, "Budget does not belong to the current user.");
            }
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
            envelopeCategoryResponses = (await _connection.QueryAsync<SummaryEnvelopeCategoryResponse>(query, new { BudgetId = request.EntityId })).ToList();
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
            envelopeResponses = (await _connection.QueryAsync<SummaryEnvelopeResponse>(query, new { BudgetId = request.EntityId })).ToList();
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

            // Set the envelope list of the envelope category to the existing list
            allEnvelopeCategoriesResponse.EnvelopeCategories = envelopeCategoryResponses;
            // Set the status and message for the envelope category response
            allEnvelopeCategoriesResponse.HttpStatus = 200;
            allEnvelopeCategoriesResponse.Message = "Envelope categories found";
            // Return the envelope category
            return allEnvelopeCategoriesResponse;
        }

        /// <summary>
        /// DAO method to update an envelope category based on the given fields
        /// </summary>
        /// <param name="envelopeCategory"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateEnvelopeCategoryAsync(EditEnvelopeCategoryRequest envelopeCategory)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the envelope category belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeCategoryAsync(envelopeCategory.EnvelopeCategoryId, envelopeCategory.UserId))
            {
                return new BaseIdResponse(403, "Envelope category does not belong to the current user.", envelopeCategory.EnvelopeCategoryId);
            }
            // Check each nullable field to see if update is necessary
            if (envelopeCategory.EnvelopeCategoryName != null)
            {
                updates.Add("envelope_category_name = @EnvelopeCategoryName");
            }
            // If no fields to update, return early
            if (updates.Count == 0)
            {
                return new BaseIdResponse(400, "No fields to update");
            }
            // Assemble the query
            query = $"""
                UPDATE envelope_category 
                SET {string.Join(", ", updates)}
                WHERE envelope_category_id = @EnvelopeCategoryId
                """;
            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, envelopeCategory);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_category_name'"))
                    {
                        return new BaseIdResponse(400, "Category name already exists in this budget");
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
                return new BaseIdResponse(200, "Envelope category updated successfully", envelopeCategory.EnvelopeCategoryId);
            }
            else if (rowsAffected == 0)
            {
                return new BaseIdResponse(404, "Envelope category not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
            }
        }

        /// <summary>
        /// DAO method to delete an envelope category
        /// </summary>
        /// <param name="envelopeCategoryId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteEnvelopeCategoryAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM envelope_category 
                WHERE envelope_category_id = @EnvelopeCategoryId
                """;
            int rowsAffected;

            // Make sure the envelope category belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeCategoryAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Envelope category does not belong to the current user.");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { EnvelopeCategoryId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the user was not found
                return new BaseResponse(404, "Envelope category not found");
            }
            // Return the success
            return new BaseResponse(200, "Envelope category deleted successfully");
        }
    }
}
