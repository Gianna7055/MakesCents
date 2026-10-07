/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using Humanizer;
using MakesCentsBackend.Models;
using MySqlConnector;
using System.Reflection;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    /// <summary>
    /// Data access object for paychecks. Paychecks are forecast-only now - they don't fund
    /// envelopes directly (see EnvelopeFundingDAO for that) and aren't tied to any specific
    /// month, so this is plain single-table CRUD.
    /// </summary>
    public class PaycheckDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public PaycheckDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create a paycheck
        /// </summary>
        /// <param name="paycheck"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreatePaycheckAsync(CreatePaycheckRequest paycheck)
        {
            // Declare and initialize
            query = """
                INSERT INTO paycheck (budget_id, paycheck_name, starting_date, secondary_date, total_amount, paycheck_regularity_id)
                VALUES (@BudgetId, @PaycheckName, @StartingDate, @SecondaryDate, @TotalAmount, @PaycheckRegularity);
                SELECT LAST_INSERT_ID();
                """;
            int paycheckId;

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(paycheck.BudgetId, paycheck.UserId))
            {
                // Return the issue
                return new BaseIdResponse(403, "Budget does not belong to the current user");
            }
            // Execute the request and get the new id for the paycheck
            try
            {
                // Execute
                paycheckId = await _connection.ExecuteScalarAsync<int>(query, paycheck);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_paycheck_name'"))
                    {
                        return new BaseIdResponse(400, "Paycheck name already exists in this budget");
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
            return new BaseIdResponse(201, "Paycheck created successfully", paycheckId);
        }

        /// <summary>
        /// DAO method to get every paycheck for a budget
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetAllPaychecksResponse> GetAllPaychecksAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                SELECT 
                    paycheck.paycheck_id AS PaycheckId,
                    paycheck.starting_date AS StartingDate,
                    paycheck.secondary_date AS SecondaryDate,
                    paycheck.total_amount AS TotalAmount,
                    paycheck.paycheck_regularity_id AS PaycheckRegularityId
                FROM paycheck
                WHERE paycheck.budget_id = @BudgetId
                ORDER BY paycheck.starting_date DESC;
                """;
            GetAllPaychecksResponse response = new GetAllPaychecksResponse();

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(request.EntityId, request.UserId))
            {
                // Return the issue
                return new GetAllPaychecksResponse(403, "Budget does not belong to the current user");
            }
            // Execute the query
            response.Paychecks = (await _connection.QueryAsync<SummaryPaycheckResponse>(query, new { BudgetId = request.EntityId })).ToList();

            // Set the status and message for the response
            response.HttpStatus = 200;
            response.Message = "Paychecks found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to get a single paycheck
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetPaycheckResponse> GetPaycheckAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetPaycheckResponse response = new GetPaycheckResponse();

            // Make sure the paycheck belongs to the user
            if (!await _authService.VerifyUserOwnsPaycheckAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetPaycheckResponse(403, "Paycheck does not belong to the current user");
            }
            // Set up the query to get the paycheck
            query = """
                SELECT 
                    paycheck.paycheck_id AS PaycheckId,
                    paycheck.budget_id AS BudgetId,
                    paycheck.paycheck_name AS PaycheckName,
                    paycheck.starting_date AS StartingDate,
                    paycheck.secondary_date AS SecondaryDate,
                    paycheck.total_amount AS TotalAmount,
                    paycheck.paycheck_regularity_id AS PaycheckRegularityId
                FROM paycheck
                WHERE paycheck.paycheck_id = @PaycheckId
                """;
            // Execute the query and get the DTO
            response.Paycheck = await _connection.QuerySingleAsync<GetPaycheckDTOModel>(query, new { PaycheckId = request.EntityId });
            // Make sure the paycheck is not null
            if (response.Paycheck == null)
            {
                // Return the issue
                return new GetPaycheckResponse(404, "Paycheck not found");
            }
            // Set the status and the message for the response
            response.HttpStatus = 200;
            response.Message = "Paycheck found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to update a paycheck based on given fields
        /// </summary>
        /// <param name="paycheck"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdatePaycheckAsync(UpdatePaycheckRequest paycheck)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;

            // Make sure the paycheck belongs to the user
            if (!await _authService.VerifyUserOwnsPaycheckAsync(paycheck.PaycheckId, paycheck.UserId))
            {
                // Return the issue
                return new BaseIdResponse(403, "Paycheck does not belong to the current user", paycheck.PaycheckId);
            }
            // Loop through each field to see if an update is necessary
            foreach (PropertyInfo property in typeof(UpdatePaycheckRequest).GetProperties())
            {
                // Skip the ids
                if (property.Name == "PaycheckId" || property.Name == "UserId") continue;
                object? propertyValue = property.GetValue(paycheck);

                // Check if the property is IOptional and has a value
                if (propertyValue is IOptional optional && optional.HasValue)
                {
                    // Add the Optional<T> to the query
                    updates.Add($"{property.Name.Underscore()} = @{property.Name}");
                }
                // Check if the property is null
                else if (propertyValue != null)
                {
                    // Add the property to the query
                    updates.Add($"{property.Name.Underscore()} = @{property.Name}");
                }
            }
            // If no fields to update, return early
            if (updates.Count == 0)
            {
                return new BaseIdResponse(400, "No fields to update");
            }
            // Assemble the query
            query = $"""
                UPDATE paycheck 
                SET {string.Join(", ", updates)}
                WHERE paycheck_id = @PaycheckId
                """;
            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, paycheck);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_paycheck_name'"))
                    {
                        return new BaseIdResponse(400, "Paycheck name already exists in this budget");
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
                return new BaseIdResponse(200, "Paycheck updated successfully", paycheck.PaycheckId);
            }
            else if (rowsAffected == 0)
            {
                return new BaseIdResponse(404, "Paycheck not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
            }
        }

        /// <summary>
        /// DAO method to delete a paycheck
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeletePaycheckAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM paycheck 
                WHERE paycheck_id = @PaycheckId
                """;
            int rowsAffected;

            // Make sure the paycheck belongs to the user
            if (!await _authService.VerifyUserOwnsPaycheckAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Paycheck does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { PaycheckId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the paycheck was not found
                return new BaseResponse(404, "Paycheck not found");
            }
            // Return the success
            return new BaseResponse(200, "Paycheck deleted successfully");
        }
    }
}