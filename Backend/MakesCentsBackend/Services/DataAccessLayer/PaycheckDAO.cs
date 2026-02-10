/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using Dapper;
using MakesCentsBackend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MySqlConnector;
using System.Data;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    public class PaycheckDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>

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
                    response.PaycheckId = paycheckId;
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
                        // Add the new id to the DTO split id list
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
    }
}
