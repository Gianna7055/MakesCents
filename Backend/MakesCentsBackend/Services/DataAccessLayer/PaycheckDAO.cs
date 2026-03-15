/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using Humanizer;
using MakesCentsBackend.Models;
using MySqlConnector;
using System.Data;
using System.Reflection;
using System.Security.Principal;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class PaycheckDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        public PaycheckDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create a new paycheck
        /// </summary>
        /// <param name="paycheck"></param>
        /// <returns></returns>
        public async Task<CreatePaycheckResponse> CreatePaycheckAsync(CreatePaycheckRequest paycheck)
        {
            // Declare and initialize
            CreatePaycheckResponse response = new CreatePaycheckResponse();
            int paycheckId;
            int paycheckSplitId;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                try
                {
                    // Make sure the budget belongs to the user
                    if (!await _authService.VerifyUserOwnsBudgetAsync(paycheck.BudgetId, paycheck.UserId, dbTransaction))
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        return new CreatePaycheckResponse(403, "Budget does not belong to the current user");
                    }
                    // Set up the query for adding the paycheck
                    query = """
                        INSERT INTO paycheck (budget_id, paycheck_name, starting_date, secondary_date, total_amount, paycheck_regularity_id)
                        VALUES (@BudgetId, @PaycheckName, @StartingDate, @SecondaryDate, @TotalAmount, @PaycheckRegularity);
                        SELECT LAST_INSERT_ID();
                        """;
                    // Execute the query and get the paycheck id
                    paycheckId = await _connection.QuerySingleAsync<int>(query, paycheck, dbTransaction);
                    // Set the paycheck id in the response
                    response.Id = paycheckId;
                    // Loop through the paycheck splits
                    foreach (CreatePaycheckSplitRequest split in paycheck.PaycheckSplits)
                    {
                        // Set the paycheck id in the split model
                        split.PaycheckId = paycheckId;
                        // Set up the query for adding a paycheck split
                        query = """
                        INSERT INTO paycheck_split (paycheck_id, envelope_id, amount, order_index)
                        VALUES (@PaycheckId, @EnvelopeId, @Amount, 0);
                        SELECT LAST_INSERT_ID();
                        """;
                        // Execute the query
                        paycheckSplitId = await _connection.QuerySingleAsync<int>(query, split, dbTransaction);
                        // Add the new id to the response split id list
                        response.PaycheckSplitIds.Add(paycheckSplitId);
                    }
                    // Commit the transaction
                    dbTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Check the specific SQL error
                    if (ex.Number == 1062)
                    {
                        // Check if the error is due to a non-unique name
                        if (ex.Message.Contains("'unique_paycheck_name'"))
                        {
                            return new CreatePaycheckResponse(400, "Paycheck name already exists in this budget");
                        }
                    }
                    return new CreatePaycheckResponse(500, $"{ex.Number}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new CreatePaycheckResponse(500, $"{ex.Message}");
                }
            }
            // Set the status and the message
            response.HttpStatus = 201;
            response.Message = "Paycheck created successfully";
            // Return the result
            return response;
        }


        public async Task<GetAllPaychecksResponse> GetAllPaychecksAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                SELECT 
                    paycheck.paycheck_id AS PaycheckId,
                    paycheck.budget_id AS BudgetId,
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
                return new GetAllPaychecksResponse(403, "Budget does not belong to the current user");
            }
            // Execute the query
            response.Paychecks = (await _connection.QueryAsync<SummaryPaycheckResponse>(query, new { BudgetId = request.EntityId })).ToList();

            // Set the http status and message for the response
            response.HttpStatus = 200;
            response.Message = "Paychecks found";
            // Return the response
            return response;
        }


        public async Task<GetPaycheckResponse> GetPaycheckAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetPaycheckResponse response = new GetPaycheckResponse();
            GetPaycheckDTOModel responseDTO;


            // Make sure the paycheck belongs to the user
            if (!await _authService.VerifyUserOwnsPaycheckAsync(request.EntityId, request.UserId))
            {
                return new GetPaycheckResponse(403, "Paycheck does not belong to the current user");
            }
            // Set up the query to get the paycheck
            query = """
                SELECT 
                    paycheck.paycheck_id AS PaycheckId,
                    paycheck.budget_id AS BudgetId,
                    paycheck.starting_date AS StartingDate,
                    paycheck.secondary_date AS SecondaryDate,
                    paycheck.total_amount AS TotalAmount,
                    paycheck.paycheck_regularity_id AS PaycheckRegularityId
                FROM paycheck
                WHERE paycheck.paycheck_id = @PaycheckId
                """;
            // Execute the query and get the DTO
            responseDTO = await _connection.QuerySingleAsync<GetPaycheckDTOModel>(query, new { PaycheckId = request.EntityId });
            // Make sure the paycheck is not null
            if (responseDTO == null)
            {
                return new GetPaycheckResponse(404, "Paycheck not found");
            }

            // Set up the query for the list of paycheck splits
            query = """
                SELECT 
                    paycheck_split.paycheck_split_id AS PaycheckSplitId,
                    paycheck_split.paycheck_id AS PaycheckId,
                    paycheck_split.envelope_id AS EnvelopeId,
                    paycheck_split.amount AS Amount,
                    paycheck_split.order_index AS OrderIndex
                FROM paycheck_split
                WHERE paycheck_split.paycheck_id = @PaycheckId;
                """;
            // Execute the query
            responseDTO.PaycheckSplits = (await _connection.QueryAsync<GetPaycheckSplitDTOModel>(query, new { PaycheckId = request.EntityId })).ToList();

            // Set the paycheck split in the response model
            response.Paycheck = responseDTO;
            // Set the status and the message for the response
            response.HttpStatus = 200;
            response.Message = "Debt account found";
            // Return the response
            return response;
        }


        public async Task<UpdatePaycheckResponse> UpdatePaycheckAsync(UpdatePaycheckRequest paycheck)
        {
            // Declare and initialize
            UpdatePaycheckResponse response = new UpdatePaycheckResponse();
            List<string> updates = new List<string>();
            List<int> existingSplitIds;
            List<int> incomingSplitIds;
            List<int> splitsToDelete;
            int rowsAffected;
            int paycheckSplitId;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Make sure the paycheck belongs to the user
                if (!await _authService.VerifyUserOwnsPaycheckAsync(paycheck.PaycheckId, paycheck.UserId))
                {
                    return new UpdatePaycheckResponse(403, "Paycheck does not belong to the current user", paycheck.PaycheckId);
                }

                // Update the paycheck table
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
                    return new UpdatePaycheckResponse(400, "No fields to update");
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
                    rowsAffected = await _connection.ExecuteAsync(query, paycheck, dbTransaction);
                }
                catch (MySqlException ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Check if the error is 1062
                    if (ex.Number == 1062)
                    {
                        // Check if the error is due to a non-unique name
                        if (ex.Message.Contains("'unique_paycheck_name'"))
                        {
                            return new UpdatePaycheckResponse(400, "Paycheck name already exists in this budget");
                        }
                    }
                    return new UpdatePaycheckResponse(500, $"{ex.Number}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new UpdatePaycheckResponse(500, $"{ex.Message}");
                }
                if (rowsAffected == 0)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new UpdatePaycheckResponse(404, "Paycheck not found");
                }

                // Update the paycheck splits table
                if (paycheck.PaycheckSplits != null)
                {
                    // Get the existing split ids from the database
                    query = """
                        SELECT paycheck_split_id AS PaycheckSplitId 
                        FROM paycheck_split 
                        WHERE paycheck_id = @PaycheckId
                        """;
                    try
                    {
                        // Return the ids to an int list
                        existingSplitIds = (await _connection.QueryAsync<int>(query, new { PaycheckId = paycheck.PaycheckId }, dbTransaction)).ToList();
                    }
                    catch (Exception ex)
                    {
                        // Roll the transaction back
                        dbTransaction.Rollback();
                        // Return the issue
                        return new UpdatePaycheckResponse(500, $"{ex.Message}");
                    }
                    // Get the incoming split ids
                    incomingSplitIds = paycheck.PaycheckSplits.Where(s => s.PaycheckSplitId != null).Select(s => s.PaycheckSplitId!.Value).ToList();

                    // Get the splits to delete by comparing the existing and incoming splits
                    splitsToDelete = existingSplitIds.Except(incomingSplitIds).ToList();
                    // Check if there are splits to delete
                    if (splitsToDelete.Count > 0)
                    {
                        query = """
                            DELETE FROM paycheck_split 
                            WHERE paycheck_split_id 
                            IN @SplitIds
                            """;
                        await _connection.QueryAsync(query, new { SplitIds = splitsToDelete }, dbTransaction);
                    }

                    // Loop through the splits
                    foreach (UpdatePaycheckSplitRequest split in paycheck.PaycheckSplits)
                    {
                        // Check if the paycheck split id is null
                        if (split.PaycheckSplitId.HasValue)
                        {
                            // Update the existing split
                            query = """
                                UPDATE paycheck_split 
                                SET envelope_id = @EnvelopeId, 
                                    amount = @Amount
                                WHERE paycheck_split_id = @PaycheckSplitId
                                """;
                            try
                            {
                                // Return the ids to an int list
                                existingSplitIds = (await _connection.QueryAsync<int>(query, split, dbTransaction)).ToList();
                            }
                            catch (Exception ex)
                            {
                                // Roll the transaction back
                                dbTransaction.Rollback();
                                // Return the issue
                                return new UpdatePaycheckResponse(500, $"{ex.Message}");
                            }
                        }
                        else
                        {
                            // Set the paycheck id
                            split.PaycheckId = paycheck.PaycheckId;
                            // Add a new split
                            query = """
                                INSERT INTO paycheck_split (paycheck_id, envelope_id, amount)
                                VALUES (@PaycheckId, @EnvelopeId, @Amount);
                                SELECT LAST_INSERT_ID();
                            """;
                            // Execute the query
                            paycheckSplitId = await _connection.QuerySingleAsync<int>(query, split, dbTransaction);
                            // Add the new id to the response split id list
                            response.PaycheckSplitIds.Add(paycheckSplitId);
                        }
                    }
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Set the status and the message
            response.HttpStatus = 201;
            response.Message = "Paycheck updated successfully";
            // Return the result
            return response;
        }


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
                // Return that the user was not found
                return new BaseResponse(404, "Paycheck not found");
            }
            // Return the success
            return new BaseResponse(200, "Paycheck deleted successfully");
        }
    }
}
