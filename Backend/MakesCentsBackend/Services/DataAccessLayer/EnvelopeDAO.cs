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
    public class EnvelopeDAO
    {
        // Class level variables
        string query = "";
        private readonly MySqlConnection _connection;
        private readonly AuthorizationService _authService;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="connection"></param>
        public EnvelopeDAO(MySqlConnection connection, AuthorizationService authService)
        {
            _connection = connection;
            _authService = authService;
        }

        /// <summary>
        /// DAO method to create an envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateEnvelopeAsync(CreateEnvelopeRequest envelope)
        {
            // Declare and initialize
            query = """
                INSERT INTO envelope (envelope_category_id, envelope_name, planned_amount, remaining_amount, is_sinking_fund, goal_amount, goal_end_date, transfer_envelope_id)
                VALUES (@EnvelopeCategoryId, @EnvelopeName, @PlannedAmount, @RemainingAmount, @IsSinkingFund, @GoalAmount, @GoalEndDate, @TransferEnvelopeId);
                SELECT LAST_INSERT_ID();
                """;
            int envelopeId;

            // Make sure the envelope category belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeCategoryAsync(envelope.EnvelopeCategoryId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Envelope category does not belong to the current user.");
            }
            // Make sure the envelope category belongs to the user
            if (!await _authService.VerifyUserOwnsEnvelopeCategoryAsync(envelope.EnvelopeCategoryId, envelope.UserId))
            {
                return new BaseIdResponse(403, "Envelope category does not belong to the current user.");
            }

            // Execute the request and get the new id for the envelope
            try
            {
                // Execute
                envelopeId = await _connection.ExecuteScalarAsync<int>(query, envelope);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
                {
                    // Check if the error is due to a non-unique name
                    if (ex.Message.Contains("'unique_envelope_name'"))
                    {
                        return new BaseIdResponse(400, "Envelope name already exists in this category");
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
            return new BaseIdResponse(201, "Envelope created successfully", envelopeId);
        }
    }
}
