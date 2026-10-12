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
    /// <summary>
    /// Data access object for the envelope funding queue - a budget month's plan for how much
    /// to put into each envelope and in what priority order. This is a plan only; it doesn't
    /// move money on its own the way a transaction does.
    /// </summary>
    public class EnvelopeFundingDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        private readonly RolloverEnvelopeDAO _rolloverEnvelopeDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        /// <param name="rolloverEnvelopeDAO"></param>
        public EnvelopeFundingDAO(MySqlConnection connection, AuthorizationService authService, RolloverEnvelopeDAO rolloverEnvelopeDAO)
        {
            _connection = connection;
            _authService = authService;
            _rolloverEnvelopeDAO = rolloverEnvelopeDAO;
        }

        /// <summary>
        /// DAO method to set (create/reorder/update/remove all at once) the funding queue for a
        /// budget month. 
        /// Existing rows not present in the incoming list are deleted.
        /// Rows with an EnvelopeFundingId are updated in place.
        /// Rows without one are inserted new. 
        /// order_index is reassigned by each item's position in the incoming list - kept rows are first bumped
        /// into a high temporary range before being written back to their final low value, since
        /// unique_envelope_funding_order is checked immediately (not deferred) and a straight
        /// reorder could otherwise collide with another row's still-current value.
        /// Also enforces that the same envelope can't have two funding rows in the same month
        /// (unique_envelope_funding_sinking / unique_envelope_funding_rollover).
        /// </summary>
        /// <param name="fundingQueue"></param>
        /// <returns></returns>
        public async Task<GetEnvelopeFundingQueueResponse> SetEnvelopeFundingQueueAsync(SetEnvelopeFundingQueueRequest fundingQueue)
        {
            // Declare and initialize
            GetEnvelopeFundingQueueResponse response = new GetEnvelopeFundingQueueResponse();
            List<int> existingIds;
            List<int> incomingIds;
            List<int> idsToDelete;
            int envelopeFundingId;
            int orderIndex;
            int bumpIndex;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Make sure this budget month belongs to the user
                if (!await _authService.VerifyUserOwnsBudgetMonthAsync(fundingQueue.BudgetMonthId, fundingQueue.UserId, dbTransaction))
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new GetEnvelopeFundingQueueResponse(403, "Budget does not belong to the current user");
                }

                // Get the existing funding row ids for this month
                query = """
                    SELECT envelope_funding_id 
                    FROM envelope_funding 
                    WHERE budget_month_id = @BudgetMonthId
                    """;
                try
                {
                    // Return the ids to an int list
                    existingIds = (await _connection.QueryAsync<int>(query, new { fundingQueue.BudgetMonthId }, dbTransaction)).ToList();
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new GetEnvelopeFundingQueueResponse(500, $"{ex.Message}");
                }
                // Get the incoming ids
                incomingIds = fundingQueue.FundingQueue.Where(f => f.EnvelopeFundingId != null).Select(f => f.EnvelopeFundingId!.Value).ToList();

                // Get the rows to delete by comparing the existing and incoming ids
                idsToDelete = existingIds.Except(incomingIds).ToList();
                // Check if there are rows to delete
                if (idsToDelete.Count > 0)
                {
                    query = """
                        DELETE FROM envelope_funding 
                        WHERE envelope_funding_id 
                        IN @Ids
                        """;
                    await _connection.QueryAsync(query, new { Ids = idsToDelete }, dbTransaction);
                }

                // Phase 1: bump every KEPT row's order_index into a safe temporary range
                bumpIndex = 0;
                foreach (SetEnvelopeFundingItemRequest item in fundingQueue.FundingQueue)
                {
                    if (item.EnvelopeFundingId.HasValue)
                    {
                        bumpIndex++;
                        query = """
                            UPDATE envelope_funding 
                            SET order_index = @TempOrderIndex 
                            WHERE envelope_funding_id = @EnvelopeFundingId
                            """;
                        await _connection.ExecuteAsync(query, new { TempOrderIndex = 100000 + bumpIndex, item.EnvelopeFundingId }, dbTransaction);
                    }
                }

                // Phase 2: update/insert with the final order_index based on position in the list
                orderIndex = 0;
                foreach (SetEnvelopeFundingItemRequest item in fundingQueue.FundingQueue)
                {
                    orderIndex++;
                    int? sinkingFundEnvelopeId = null;
                    int? rolloverEnvelopeMonthId = null;

                    if (item.IsSinkingFund)
                    {
                        // Makes sure the envelope belongs to the user
                        if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(item.EnvelopeId, fundingQueue.UserId, dbTransaction))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new GetEnvelopeFundingQueueResponse(403, "Envelope does not belong to the current user");
                        }
                        sinkingFundEnvelopeId = item.EnvelopeId;
                    }
                    else
                    {
                        // Makes sure the envelope belongs to the user
                        if (!await _authService.VerifyUserOwnsRolloverEnvelopeAsync(item.EnvelopeId, fundingQueue.UserId, dbTransaction))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new GetEnvelopeFundingQueueResponse(403, "Envelope does not belong to the current user");
                        }
                        // Resolve (or activate) this month's instance of the rollover envelope
                        rolloverEnvelopeMonthId = await _rolloverEnvelopeDAO.GetOrActivateForMonthAsync(item.EnvelopeId, fundingQueue.BudgetMonthId, dbTransaction);
                        if (rolloverEnvelopeMonthId == null)
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new GetEnvelopeFundingQueueResponse(500, "There was an issue activating the rollover envelope for this month");
                        }
                    }

                    // Check if this is an existing row being updated or a brand new one
                    if (item.EnvelopeFundingId.HasValue)
                    {
                        // Update the existing row
                        query = """
                            UPDATE envelope_funding 
                            SET sinking_fund_envelope_id = @SinkingFundEnvelopeId,
                                rollover_envelope_month_id = @RolloverEnvelopeMonthId,
                                planned_amount = @PlannedAmount,
                                order_index = @OrderIndex
                            WHERE envelope_funding_id = @EnvelopeFundingId
                            """;
                        try
                        {
                            // Execute
                            await _connection.ExecuteAsync(query, new
                            {
                                SinkingFundEnvelopeId = sinkingFundEnvelopeId,
                                RolloverEnvelopeMonthId = rolloverEnvelopeMonthId,
                                item.PlannedAmount,
                                OrderIndex = orderIndex,
                                item.EnvelopeFundingId
                            }, dbTransaction);
                        }
                        catch (MySqlException ex)
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            // Check if the error is 1062
                            if (ex.Number == 1062)
                            {
                                // Check if the error is due to the same envelope appearing twice in this month's queue
                                if (ex.Message.Contains("'unique_envelope_funding_sinking'") || ex.Message.Contains("'unique_envelope_funding_rollover'"))
                                {
                                    return new GetEnvelopeFundingQueueResponse(400, "This envelope already has a funding row for this month");
                                }
                            }
                            return new GetEnvelopeFundingQueueResponse(500, $"{ex.Number}: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            // Return the issue
                            return new GetEnvelopeFundingQueueResponse(500, $"{ex.Message}");
                        }
                        envelopeFundingId = item.EnvelopeFundingId.Value;
                    }
                    else
                    {
                        // Insert a brand new row
                        query = """
                            INSERT INTO envelope_funding (budget_month_id, sinking_fund_envelope_id, rollover_envelope_month_id, planned_amount, order_index)
                            VALUES (@BudgetMonthId, @SinkingFundEnvelopeId, @RolloverEnvelopeMonthId, @PlannedAmount, @OrderIndex);
                            SELECT LAST_INSERT_ID();
                            """;
                        try
                        {
                            // Execute
                            envelopeFundingId = await _connection.QuerySingleAsync<int>(query, new
                            {
                                fundingQueue.BudgetMonthId,
                                SinkingFundEnvelopeId = sinkingFundEnvelopeId,
                                RolloverEnvelopeMonthId = rolloverEnvelopeMonthId,
                                item.PlannedAmount,
                                OrderIndex = orderIndex
                            }, dbTransaction);
                        }
                        catch (MySqlException ex)
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            // Check if the error is 1062
                            if (ex.Number == 1062)
                            {
                                // Check if the error is due to the same envelope appearing twice in this month's queue
                                if (ex.Message.Contains("'unique_envelope_funding_sinking'") || ex.Message.Contains("'unique_envelope_funding_rollover'"))
                                {
                                    return new GetEnvelopeFundingQueueResponse(400, "This envelope already has a funding row for this month");
                                }
                            }
                            return new GetEnvelopeFundingQueueResponse(500, $"{ex.Number}: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            // Return the issue
                            return new GetEnvelopeFundingQueueResponse(500, $"{ex.Message}");
                        }
                    }
                    // Add this row to the response so the caller gets the full saved queue back
                    // without needing a second round trip
                    response.FundingQueue.Add(new GetEnvelopeFundingDTOModel
                    {
                        EnvelopeFundingId = envelopeFundingId,
                        BudgetMonthId = fundingQueue.BudgetMonthId,
                        EnvelopeId = item.EnvelopeId,
                        IsSinkingFund = item.IsSinkingFund,
                        PlannedAmount = item.PlannedAmount,
                        OrderIndex = orderIndex
                    });
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Set the status and the message
            response.HttpStatus = 200;
            response.Message = "Funding queue saved successfully";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to get the funding queue for a budget month, in priority order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetEnvelopeFundingQueueResponse> GetEnvelopeFundingQueueAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetEnvelopeFundingQueueResponse response = new GetEnvelopeFundingQueueResponse();

            // Make sure this budget month belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetMonthAsync(request.EntityId, request.UserId))
            {
                // Return the issue
                return new GetEnvelopeFundingQueueResponse(403, "Budget does not belong to the current user");
            }
            // Set up the query to get the funding queue. EnvelopeId is resolved back to whichever
            // table it came from - sinking_fund_envelope_id directly, or the persistent
            // rollover_envelope_id via the month instance - with IsSinkingFund saying which.
            query = """
                SELECT 
                    envelope_funding.envelope_funding_id AS EnvelopeFundingId,
                    envelope_funding.budget_month_id AS BudgetMonthId,
                    COALESCE(envelope_funding.sinking_fund_envelope_id, rollover_envelope_month.rollover_envelope_id) AS EnvelopeId,
                    (envelope_funding.sinking_fund_envelope_id IS NOT NULL) AS IsSinkingFund,
                    envelope_funding.planned_amount AS PlannedAmount,
                    envelope_funding.order_index AS OrderIndex
                FROM envelope_funding
                LEFT JOIN rollover_envelope_month ON envelope_funding.rollover_envelope_month_id = rollover_envelope_month.rollover_envelope_month_id
                WHERE envelope_funding.budget_month_id = @BudgetMonthId
                ORDER BY envelope_funding.order_index;
                """;
            // Execute the query
            response.FundingQueue = (await _connection.QueryAsync<GetEnvelopeFundingDTOModel>(query, new { BudgetMonthId = request.EntityId })).ToList();

            // Set the status and message for the response
            response.HttpStatus = 200;
            response.Message = "Funding queue found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to delete a single envelope funding row, without touching the rest of the queue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteEnvelopeFundingAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM envelope_funding 
                WHERE envelope_funding_id = @EnvelopeFundingId
                """;
            int rowsAffected;

            // Make sure the envelope funding row belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeFundingAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Envelope funding row does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { EnvelopeFundingId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the row was not found
                return new BaseResponse(404, "Envelope funding row not found");
            }
            // Return the success
            return new BaseResponse(200, "Envelope funding row deleted successfully");
        }
    }
}