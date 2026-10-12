/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a rollover envelope's persistent definition.
    /// No dollar amounts live here - a rollover envelope isn't guaranteed to be active
    /// every month, so amounts live on the per-month instance (see RolloverEnvelopeMonthEntityModel).
    /// </summary>
    public class RolloverEnvelopeEntityModel
    {
        // Class Level Properties
        public int RolloverEnvelopeId { get; set; } = 0;
        public int EnvelopeCategoryId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public int TransferEnvelopeId { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Model for a single month's instance of a rollover envelope.
    /// Absence of a row for a given (RolloverEnvelopeId, BudgetMonthId) pair means
    /// the envelope was inactive that month.
    /// </summary>
    public class RolloverEnvelopeMonthEntityModel
    {
        // Class Level Properties
        public int RolloverEnvelopeMonthId { get; set; } = 0;
        public int RolloverEnvelopeId { get; set; } = 0;
        public int BudgetMonthId { get; set; } = 0;
        public decimal RemainingAmount { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Request class for creating a rollover envelope's persistent definition
    /// </summary>
    [ExportTsClass]
    public class CreateRolloverEnvelopeRequest
    {
        // Class properties
        public int EnvelopeCategoryId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public int TransferEnvelopeId { get; set; } = 0;
    }

        /// <summary>
        /// Request used to look up a rollover envelope in the context of one specific budget month.
        /// Built server-side from the route/JWT, same as BaseIdRequest - not sent by the client directly.
        /// </summary>
        public class GetRolloverEnvelopeMonthRequest
    {
        // Class properties
        public int UserId { get; set; } = 0;
        public int RolloverEnvelopeId { get; set; } = 0;
        public int BudgetMonthId { get; set; } = 0;

        public GetRolloverEnvelopeMonthRequest() { }

        public GetRolloverEnvelopeMonthRequest(int userId, int rolloverEnvelopeId, int budgetMonthId)
        {
            UserId = userId;
            RolloverEnvelopeId = rolloverEnvelopeId;
            BudgetMonthId = budgetMonthId;
        }
    }

    /// <summary>
    /// Base fields returned for a rollover envelope in the context of one budget month.
    /// RemainingAmount/RolloverEnvelopeMonthId are null if not yet activated for that month.
    /// Planned funding for the month lives on envelope_funding, not here.
    /// </summary>
    [ExportTsInterface]
    public class GetRolloverEnvelopeBaseModel
    {
        // Class properties
        public int RolloverEnvelopeId { get; set; } = 0;
        public int EnvelopeCategoryId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public int TransferEnvelopeId { get; set; } = 0;
        public int? RolloverEnvelopeMonthId { get; set; } = null;
        public decimal? RemainingAmount { get; set; } = null;
    }

    /// <summary>
    /// DTO model to get a specific rollover envelope for a given month
    /// </summary>
    [ExportTsInterface]
    public class GetRolloverEnvelopeDTOModel : GetRolloverEnvelopeBaseModel
    {
        // Class properties
        public List<SummaryTransactionDTOModel> Transactions { get; set; } = new List<SummaryTransactionDTOModel>();
    }

    /// <summary>
    /// Response model for getting a specific rollover envelope
    /// </summary>
    [ExportTsInterface]
    public class GetRolloverEnvelopeDTOResponse : BaseResponse
    {
        // Class properties
        public GetRolloverEnvelopeDTOModel RolloverEnvelopeDTO { get; set; } = new GetRolloverEnvelopeDTOModel();

        public GetRolloverEnvelopeDTOResponse(int httpStatus, string message) : base(httpStatus, message) { }

        public GetRolloverEnvelopeDTOResponse() : base() { }
    }

    /// <summary>
    /// Entity model for getting a specific rollover envelope
    /// </summary>
    public class GetRolloverEnvelopeEntityModel : GetRolloverEnvelopeBaseModel
    {
        // Class properties
        public List<SummaryTransactionEntityModel> Transactions { get; set; } = new List<SummaryTransactionEntityModel>();
    }

    /// <summary>
    /// Entity response model for getting a specific rollover envelope
    /// </summary>
    public class GetRolloverEnvelopeEntityResponse : BaseResponse
    {
        // Class properties
        public GetRolloverEnvelopeEntityModel RolloverEnvelope { get; set; } = new GetRolloverEnvelopeEntityModel();

        /// <summary>
        /// Default constructor
        /// </summary>
        public GetRolloverEnvelopeEntityResponse() : base() { }

        /// <summary>
        /// Parameterized constructor that takes a status and a message
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public GetRolloverEnvelopeEntityResponse(int status, string message) : base(status, message) { }
    }

    /// <summary>
    /// Request class for updating a rollover envelope's persistent definition
    /// </summary>
    [ExportTsClass]
    public class UpdateRolloverEnvelopeRequest
    {
        // Class properties
        public int RolloverEnvelopeId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public int? EnvelopeCategoryId { get; set; } = null;
        public string? EnvelopeName { get; set; } = null;
        public int? TransferEnvelopeId { get; set; } = null;
    }
}