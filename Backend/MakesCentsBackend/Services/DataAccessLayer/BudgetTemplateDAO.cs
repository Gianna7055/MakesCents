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
    /// Data access object for budget templates - named, reusable presets of envelope funding
    /// amounts that can be applied to a budget month to bulk-create its funding queue.
    /// </summary>
    public class BudgetTemplateDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;
        private readonly RolloverEnvelopeDAO _rolloverEnvelopeDAO;
        private readonly EnvelopeFundingDAO _envelopeFundingDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="authService"></param>
        /// <param name="rolloverEnvelopeDAO"></param>
        /// <param name="envelopeFundingDAO"></param>
        public BudgetTemplateDAO(MySqlConnection connection, AuthorizationService authService, RolloverEnvelopeDAO rolloverEnvelopeDAO, EnvelopeFundingDAO envelopeFundingDAO)
        {
            _connection = connection;
            _authService = authService;
            _rolloverEnvelopeDAO = rolloverEnvelopeDAO;
            _envelopeFundingDAO = envelopeFundingDAO;
        }

        /// <summary>
        /// DAO method to create a budget template. No envelope entries yet - use
        /// SetBudgetTemplateEnvelopesAsync to add them afterward.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBudgetTemplateAsync(CreateBudgetTemplateRequest template)
        {
            // Declare and initialize
            query = """
                INSERT INTO budget_template (budget_id, template_name)
                VALUES (@BudgetId, @TemplateName);
                SELECT LAST_INSERT_ID();
                """;
            int budgetTemplateId;

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(template.BudgetId, template.UserId))
            {
                // Return the issue
                return new BaseIdResponse(403, "Budget does not belong to the current user");
            }
            // Execute the request and get the new id for the template
            try
            {
                // Execute
                budgetTemplateId = await _connection.ExecuteScalarAsync<int>(query, template);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_template_name'"))
                    {
                        return new BaseIdResponse(400, "Template name already exists in this budget");
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
            return new BaseIdResponse(201, "Template created successfully", budgetTemplateId);
        }

        /// <summary>
        /// DAO method to get every template for a budget
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetAllBudgetTemplatesResponse> GetAllBudgetTemplatesAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                SELECT 
                    budget_template.budget_template_id AS BudgetTemplateId,
                    budget_template.template_name AS TemplateName
                FROM budget_template
                WHERE budget_template.budget_id = @BudgetId
                ORDER BY budget_template.template_name;
                """;
            GetAllBudgetTemplatesResponse response = new GetAllBudgetTemplatesResponse();

            // Make sure the budget belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetAsync(request.EntityId, request.UserId))
            {
                // Return the issue
                return new GetAllBudgetTemplatesResponse(403, "Budget does not belong to the current user");
            }
            // Execute the query
            response.Templates = (await _connection.QueryAsync<SummaryBudgetTemplateResponse>(query, new { BudgetId = request.EntityId })).ToList();

            // Set the status and message for the response
            response.HttpStatus = 200;
            response.Message = "Templates found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to get a single budget template, including its envelope entries
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetBudgetTemplateResponse> GetBudgetTemplateAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetBudgetTemplateResponse response = new GetBudgetTemplateResponse();
            GetBudgetTemplateDTOModel? responseDTO;

            // Make sure the template belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetTemplateAsync(request.EntityId, request.UserId))
            {
                // Return the false result
                return new GetBudgetTemplateResponse(403, "Template does not belong to the current user");
            }
            // Set up the query to get the template
            query = """
                SELECT 
                    budget_template.budget_template_id AS BudgetTemplateId,
                    budget_template.budget_id AS BudgetId,
                    budget_template.template_name AS TemplateName
                FROM budget_template
                WHERE budget_template.budget_template_id = @BudgetTemplateId
                """;
            // Execute the query and get the DTO
            responseDTO = await _connection.QuerySingleOrDefaultAsync<GetBudgetTemplateDTOModel>(query, new { BudgetTemplateId = request.EntityId });
            // Make sure the template is not null
            if (responseDTO == null)
            {
                // Return the issue
                return new GetBudgetTemplateResponse(404, "Template not found");
            }
            // Set up the query for the list of envelope entries. EnvelopeId is resolved with a
            // plain COALESCE - no join needed, since rollover_envelope_id here is already the
            // persistent id, not a month-specific one.
            query = """
                SELECT 
                    budget_template_envelope.budget_template_envelope_id AS BudgetTemplateEnvelopeId,
                    budget_template_envelope.budget_template_id AS BudgetTemplateId,
                    COALESCE(budget_template_envelope.sinking_fund_envelope_id, budget_template_envelope.rollover_envelope_id) AS EnvelopeId,
                    (budget_template_envelope.sinking_fund_envelope_id IS NOT NULL) AS IsSinkingFund,
                    budget_template_envelope.planned_amount AS PlannedAmount,
                    budget_template_envelope.order_index AS OrderIndex
                FROM budget_template_envelope
                WHERE budget_template_envelope.budget_template_id = @BudgetTemplateId
                ORDER BY budget_template_envelope.order_index;
                """;
            // Execute the query
            responseDTO.Envelopes = (await _connection.QueryAsync<GetBudgetTemplateEnvelopeDTOModel>(query, new { BudgetTemplateId = request.EntityId })).ToList();

            // Set the response DTO in the response
            response.Template = responseDTO;
            // Set the status and message for the response
            response.HttpStatus = 200;
            response.Message = "Template found";
            // Return the response
            return response;
        }

        /// <summary>
        /// DAO method to rename a budget template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateBudgetTemplateAsync(EditBudgetTemplateRequest template)
        {
            // Declare and initialize
            query = """
                UPDATE budget_template 
                SET template_name = @TemplateName
                WHERE budget_template_id = @BudgetTemplateId
                """;
            int rowsAffected;

            // Make sure the template belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetTemplateAsync(template.BudgetTemplateId, template.UserId))
            {
                // Return the issue
                return new BaseIdResponse(403, "Template does not belong to the current user", template.BudgetTemplateId);
            }
            // Make sure there's actually a name to update
            if (template.TemplateName == null)
            {
                return new BaseIdResponse(400, "No fields to update");
            }
            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, template);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_template_name'"))
                    {
                        return new BaseIdResponse(400, "Template name already exists in this budget");
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
                return new BaseIdResponse(200, "Template updated successfully", template.BudgetTemplateId);
            }
            else if (rowsAffected == 0)
            {
                return new BaseIdResponse(404, "Template not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
            }
        }

        /// <summary>
        /// DAO method to delete a budget template. Cascades to remove its envelope entries.
        /// Any budget_month that had this template applied keeps its funding queue, but its
        /// budget_template_id is set back to null (the record of "which template" is lost,
        /// the funding queue itself is not).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteBudgetTemplateAsync(BaseIdRequest request)
        {
            // Declare and initialize
            query = """
                DELETE FROM budget_template 
                WHERE budget_template_id = @BudgetTemplateId
                """;
            int rowsAffected;

            // Make sure the template belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetTemplateAsync(request.EntityId, request.UserId))
            {
                return new BaseIdResponse(403, "Template does not belong to the current user");
            }
            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { BudgetTemplateId = request.EntityId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the template was not found
                return new BaseResponse(404, "Template not found");
            }
            // Return the success
            return new BaseResponse(200, "Template deleted successfully");
        }

        /// <summary>
        /// DAO method to set (create/reorder/update/remove all at once) a template's envelope
        /// entries. 
        /// Existing rows not present in the incoming list are deleted.
        /// Rows with a BudgetTemplateEnvelopeId are updated in place
        /// Rows without one are inserted new.
        /// order_index is reassigned by each item's position in the incoming list - kept rows
        /// are first bumped into a high temporary range before being written back to their final
        /// low value, since unique_template_order is checked immediately (not deferred) and a
        /// straight reorder could otherwise collide with another row's still-current value.
        /// Also enforces that the same envelope can't appear twice in one template
        /// (unique_template_envelope_sinking / unique_template_envelope_rollover).
        /// </summary>
        /// <param name="templateEnvelopes"></param>
        /// <returns></returns>
        public async Task<GetBudgetTemplateResponse> SetBudgetTemplateEnvelopesAsync(SetBudgetTemplateEnvelopesRequest templateEnvelopes)
        {
            // Declare and initialize
            List<int> existingIds;
            List<int> incomingIds;
            List<int> idsToDelete;
            int bumpIndex;
            int orderIndex;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Make sure the template belongs to the user
                if (!await _authService.VerifyUserOwnsBudgetTemplateAsync(templateEnvelopes.BudgetTemplateId, templateEnvelopes.UserId, dbTransaction))
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new GetBudgetTemplateResponse(403, "Template does not belong to the current user");
                }

                // Get the existing envelope entry ids for this template
                query = """
                    SELECT budget_template_envelope_id 
                    FROM budget_template_envelope 
                    WHERE budget_template_id = @BudgetTemplateId
                    """;
                try
                {
                    // Return the ids to an int list
                    existingIds = (await _connection.QueryAsync<int>(query, new { templateEnvelopes.BudgetTemplateId }, dbTransaction)).ToList();
                }
                catch (Exception ex)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    // Return the issue
                    return new GetBudgetTemplateResponse(500, $"{ex.Message}");
                }
                // Get the incoming ids
                incomingIds = templateEnvelopes.Envelopes.Where(e => e.BudgetTemplateEnvelopeId != null).Select(e => e.BudgetTemplateEnvelopeId!.Value).ToList();

                // Get the entries to delete by comparing the existing and incoming ids
                idsToDelete = existingIds.Except(incomingIds).ToList();
                // Check if there are entries to delete
                if (idsToDelete.Count > 0)
                {
                    query = """
                        DELETE FROM budget_template_envelope 
                        WHERE budget_template_envelope_id 
                        IN @Ids
                        """;
                    await _connection.QueryAsync(query, new { Ids = idsToDelete }, dbTransaction);
                }

                // Phase 1: bump every KEPT entry's order_index into a safe temporary range
                bumpIndex = 0;
                foreach (SetBudgetTemplateEnvelopeItemRequest item in templateEnvelopes.Envelopes)
                {
                    if (item.BudgetTemplateEnvelopeId.HasValue)
                    {
                        bumpIndex++;
                        query = """
                            UPDATE budget_template_envelope 
                            SET order_index = @TempOrderIndex 
                            WHERE budget_template_envelope_id = @BudgetTemplateEnvelopeId
                            """;
                        await _connection.ExecuteAsync(query, new { TempOrderIndex = 100000 + bumpIndex, item.BudgetTemplateEnvelopeId }, dbTransaction);
                    }
                }

                // Phase 2: update/insert with the final order_index based on position in the list
                orderIndex = 0;
                foreach (SetBudgetTemplateEnvelopeItemRequest item in templateEnvelopes.Envelopes)
                {
                    orderIndex++;
                    int? sinkingFundEnvelopeId = null;
                    int? rolloverEnvelopeId = null;

                    if (item.IsSinkingFund)
                    {
                        // Makes sure the envelope belongs to the user
                        if (!await _authService.VerifyUserOwnsSinkingFundEnvelopeAsync(item.EnvelopeId, templateEnvelopes.UserId, dbTransaction))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new GetBudgetTemplateResponse(403, "Envelope does not belong to the current user");
                        }
                        sinkingFundEnvelopeId = item.EnvelopeId;
                    }
                    else
                    {
                        // Makes sure the envelope belongs to the user. No month context is
                        // needed here - templates reference the persistent rollover envelope.
                        if (!await _authService.VerifyUserOwnsRolloverEnvelopeAsync(item.EnvelopeId, templateEnvelopes.UserId, dbTransaction))
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            return new GetBudgetTemplateResponse(403, "Envelope does not belong to the current user");
                        }
                        rolloverEnvelopeId = item.EnvelopeId;
                    }

                    // Check if this is an existing entry being updated or a brand new one
                    if (item.BudgetTemplateEnvelopeId.HasValue)
                    {
                        // Update the existing entry
                        query = """
                            UPDATE budget_template_envelope 
                            SET sinking_fund_envelope_id = @SinkingFundEnvelopeId,
                                rollover_envelope_id = @RolloverEnvelopeId,
                                planned_amount = @PlannedAmount,
                                order_index = @OrderIndex
                            WHERE budget_template_envelope_id = @BudgetTemplateEnvelopeId
                            """;
                        try
                        {
                            // Execute
                            await _connection.ExecuteAsync(query, new
                            {
                                SinkingFundEnvelopeId = sinkingFundEnvelopeId,
                                RolloverEnvelopeId = rolloverEnvelopeId,
                                item.PlannedAmount,
                                OrderIndex = orderIndex,
                                item.BudgetTemplateEnvelopeId
                            }, dbTransaction);
                        }
                        catch (MySqlException ex)
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            // Check if the error is 1062
                            if (ex.Number == 1062)
                            {
                                // Check if the error is due to the same envelope appearing twice in this template
                                if (ex.Message.Contains("'unique_template_envelope_sinking'") || ex.Message.Contains("'unique_template_envelope_rollover'"))
                                {
                                    return new GetBudgetTemplateResponse(400, "This envelope is already in this template");
                                }
                            }
                            return new GetBudgetTemplateResponse(500, $"{ex.Number}: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            // Return the issue
                            return new GetBudgetTemplateResponse(500, $"{ex.Message}");
                        }
                    }
                    else
                    {
                        // Insert a brand new entry
                        query = """
                            INSERT INTO budget_template_envelope (budget_template_id, sinking_fund_envelope_id, rollover_envelope_id, planned_amount, order_index)
                            VALUES (@BudgetTemplateId, @SinkingFundEnvelopeId, @RolloverEnvelopeId, @PlannedAmount, @OrderIndex);
                            """;
                        try
                        {
                            // Execute
                            await _connection.ExecuteAsync(query, new
                            {
                                templateEnvelopes.BudgetTemplateId,
                                SinkingFundEnvelopeId = sinkingFundEnvelopeId,
                                RolloverEnvelopeId = rolloverEnvelopeId,
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
                                // Check if the error is due to the same envelope appearing twice in this template
                                if (ex.Message.Contains("'unique_template_envelope_sinking'") || ex.Message.Contains("'unique_template_envelope_rollover'"))
                                {
                                    return new GetBudgetTemplateResponse(400, "This envelope is already in this template");
                                }
                            }
                            return new GetBudgetTemplateResponse(500, $"{ex.Number}: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            // Roll the transaction back
                            dbTransaction.Rollback();
                            // Return the issue
                            return new GetBudgetTemplateResponse(500, $"{ex.Message}");
                        }
                    }
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Return the full, freshly-saved template (name + envelope entries) in one response
            return await GetBudgetTemplateAsync(new BaseIdRequest { UserId = templateEnvelopes.UserId, EntityId = templateEnvelopes.BudgetTemplateId });
        }

        /// <summary>
        /// Core logic for applying a template to a budget month, runs inside a transaction the
        /// CALLER owns (doesn't commit/rollback itself) - so it can be reused by BudgetDAO to make
        /// "create a budget month" and "apply its template" one atomic operation. This ALWAYS
        /// fully replaces that month's existing funding queue - it does not merge with or
        /// preserve whatever was there before. Records which template was applied on
        /// budget_month.budget_template_id.
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public async Task<BaseResponse> ApplyBudgetTemplateWithinTransactionAsync(ApplyBudgetTemplateRequest apply, MySqlTransaction dbTransaction)
        {
            // Declare and initialize
            List<BudgetTemplateEnvelopeLookupModel> templateItems;
            int orderIndex;

            // Make sure the template belongs to the user
            if (!await _authService.VerifyUserOwnsBudgetTemplateAsync(apply.BudgetTemplateId, apply.UserId, dbTransaction))
            {
                return new BaseResponse(403, "Template does not belong to the current user");
            }
            // Make sure this budget month belongs to the user. Since this can run right after
            // BudgetDAO inserts the budget_month row in the SAME transaction, this correctly
            // sees that uncommitted row too - a transaction always sees its own writes.
            if (!await _authService.VerifyUserOwnsBudgetMonthAsync(apply.BudgetMonthId, apply.UserId, dbTransaction))
            {
                return new BaseResponse(403, "Budget does not belong to the current user");
            }

            // Wipe this month's existing funding queue entirely - applying a template
            // always fully replaces it, it doesn't merge with whatever was there before
            query = """
                DELETE FROM envelope_funding 
                WHERE budget_month_id = @BudgetMonthId
                """;
            await _connection.ExecuteAsync(query, new { apply.BudgetMonthId }, dbTransaction);

            // Get the template's envelope entries, in order
            query = """
                SELECT 
                    COALESCE(sinking_fund_envelope_id, rollover_envelope_id) AS EnvelopeId,
                    (sinking_fund_envelope_id IS NOT NULL) AS IsSinkingFund,
                    planned_amount AS PlannedAmount
                FROM budget_template_envelope
                WHERE budget_template_id = @BudgetTemplateId
                ORDER BY order_index;
                """;
            try
            {
                // Return the results to a list
                templateItems = (await _connection.QueryAsync<BudgetTemplateEnvelopeLookupModel>(query, new { apply.BudgetTemplateId }, dbTransaction)).ToList();
            }
            catch (Exception ex)
            {
                // Return the issue
                return new BaseResponse(500, $"{ex.Message}");
            }

            // Recreate the funding queue fresh, in the template's order
            orderIndex = 0;
            foreach (BudgetTemplateEnvelopeLookupModel item in templateItems)
            {
                orderIndex++;
                int? sinkingFundEnvelopeId = null;
                int? rolloverEnvelopeMonthId = null;

                if (item.IsSinkingFund)
                {
                    sinkingFundEnvelopeId = item.EnvelopeId;
                }
                else
                {
                    // Resolve (or activate) this month's instance of the rollover envelope
                    rolloverEnvelopeMonthId = await _rolloverEnvelopeDAO.GetOrActivateForMonthAsync(item.EnvelopeId, apply.BudgetMonthId, dbTransaction);
                    if (rolloverEnvelopeMonthId == null)
                    {
                        return new BaseResponse(500, "There was an issue activating a rollover envelope for this month");
                    }
                }

                // Set up the query to insert the new funding row
                query = """
                    INSERT INTO envelope_funding (budget_month_id, sinking_fund_envelope_id, rollover_envelope_month_id, planned_amount, order_index)
                    VALUES (@BudgetMonthId, @SinkingFundEnvelopeId, @RolloverEnvelopeMonthId, @PlannedAmount, @OrderIndex);
                    """;
                try
                {
                    // Execute
                    await _connection.ExecuteAsync(query, new
                    {
                        apply.BudgetMonthId,
                        SinkingFundEnvelopeId = sinkingFundEnvelopeId,
                        RolloverEnvelopeMonthId = rolloverEnvelopeMonthId,
                        item.PlannedAmount,
                        OrderIndex = orderIndex
                    }, dbTransaction);
                }
                catch (Exception ex)
                {
                    // Return the issue
                    return new BaseResponse(500, $"{ex.Message}");
                }
            }

            // Record which template was applied to this month
            query = """
                UPDATE budget_month 
                SET budget_template_id = @BudgetTemplateId
                WHERE budget_month_id = @BudgetMonthId
                """;
            await _connection.ExecuteAsync(query, new { apply.BudgetTemplateId, apply.BudgetMonthId }, dbTransaction);

            // No commit here - the caller owns the transaction
            return new BaseResponse(200, "Template applied successfully");
        }

        /// <summary>
        /// DAO method to apply a template to a budget month as a standalone call. Thin wrapper
        /// around ApplyBudgetTemplateWithinTransactionAsync that owns its own transaction - see
        /// that method for what actually happens.
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        public async Task<GetEnvelopeFundingQueueResponse> ApplyBudgetTemplateAsync(ApplyBudgetTemplateRequest apply)
        {
            // Declare and initialize
            BaseResponse result;

            // Check if the connection is open
            if (_connection.State != ConnectionState.Open)
                // Open the connection
                await _connection.OpenAsync();
            // Set up the transaction
            using (MySqlTransaction dbTransaction = _connection.BeginTransaction())
            {
                // Run the actual apply logic
                result = await ApplyBudgetTemplateWithinTransactionAsync(apply, dbTransaction);
                if (result.HttpStatus != 200)
                {
                    // Roll the transaction back
                    dbTransaction.Rollback();
                    return new GetEnvelopeFundingQueueResponse(result.HttpStatus, result.Message);
                }
                // Commit the transaction
                dbTransaction.Commit();
            }
            // Return the resulting funding queue for the month
            return await _envelopeFundingDAO.GetEnvelopeFundingQueueAsync(new BaseIdRequest { UserId = apply.UserId, EntityId = apply.BudgetMonthId });
        }
    }
}