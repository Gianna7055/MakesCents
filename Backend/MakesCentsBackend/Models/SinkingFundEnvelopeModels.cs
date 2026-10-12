/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a sinking fund envelope.
    /// Sinking fund envelopes are persistent - one row holds a live remaining_amount
    /// that carries forward month to month, unlike rollover envelopes.
    /// </summary>
    public class SinkingFundEnvelopeEntityModel
    {
        // Class Level Properties
        public int SinkingFundEnvelopeId { get; set; } = 0;
        public int EnvelopeCategoryId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public decimal RemainingAmount { get; set; } = 0m;
        public decimal? GoalAmount { get; set; } = null;
        public DateOnly? GoalEndDate { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
        public List<TransactionEntityModel> Transactions { get; set; } = new List<TransactionEntityModel>();
    }

    /// <summary>
    /// Request class for creating a sinking fund envelope
    /// </summary>
    [ExportTsClass]
    public class CreateSinkingFundEnvelopeRequest
    {
        // Class properties
        public int EnvelopeCategoryId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public decimal RemainingAmount { get; set; } = 0m;
        public Optional<decimal?> GoalAmount { get; set; }
        public Optional<DateOnly?> GoalEndDate { get; set; }
    }

    /// <summary>
    /// Base fields returned for a sinking fund envelope
    /// </summary>
    [ExportTsInterface]
    public class GetSinkingFundEnvelopeBaseModel
    {
        // Class properties
        public int SinkingFundEnvelopeId { get; set; } = 0;
        public int EnvelopeCategoryId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public decimal RemainingAmount { get; set; } = -1m;
        public decimal? GoalAmount { get; set; } = null;
        public DateOnly? GoalEndDate { get; set; } = null;
    }

    /// <summary>
    /// DTO model to get a specific sinking fund envelope
    /// </summary>
    [ExportTsInterface]
    public class GetSinkingFundEnvelopeDTOModel : GetSinkingFundEnvelopeBaseModel
    {
        // Class properties
        public List<SummaryTransactionDTOModel> Transactions { get; set; } = new List<SummaryTransactionDTOModel>();
    }

    /// <summary>
    /// Response model for getting a specific sinking fund envelope
    /// </summary>
    [ExportTsInterface]
    public class GetSinkingFundEnvelopeDTOResponse : BaseResponse
    {
        // Class properties
        public GetSinkingFundEnvelopeDTOModel SinkingFundEnvelopeDTO { get; set; } = new GetSinkingFundEnvelopeDTOModel();

        public GetSinkingFundEnvelopeDTOResponse(int httpStatus, string message) : base(httpStatus, message) { }

        public GetSinkingFundEnvelopeDTOResponse() : base() { }
    }

    /// <summary>
    /// Entity model for getting a specific sinking fund envelope
    /// </summary>
    public class GetSinkingFundEnvelopeEntityModel : GetSinkingFundEnvelopeBaseModel
    {
        // Class properties
        public List<SummaryTransactionEntityModel> Transactions { get; set; } = new List<SummaryTransactionEntityModel>();
    }

    /// <summary>
    /// Entity response model for getting a specific sinking fund envelope
    /// </summary>
    public class GetSinkingFundEnvelopeEntityResponse : BaseResponse
    {
        // Class properties
        public GetSinkingFundEnvelopeEntityModel SinkingFundEnvelope { get; set; } = new GetSinkingFundEnvelopeEntityModel();

        /// <summary>
        /// Default constructor
        /// </summary>
        public GetSinkingFundEnvelopeEntityResponse() : base() { }

        /// <summary>
        /// Parameterized constructor that takes a status and a message
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public GetSinkingFundEnvelopeEntityResponse(int status, string message) : base(status, message) { }
    }

    /// <summary>
    /// Request class for updating a sinking fund envelope
    /// </summary>
    [ExportTsClass]
    public class UpdateSinkingFundEnvelopeRequest
    {
        // Class properties
        public int SinkingFundEnvelopeId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public int? EnvelopeCategoryId { get; set; } = null;
        public string? EnvelopeName { get; set; } = null;
        public Optional<decimal?> GoalAmount { get; set; }
        public Optional<DateOnly?> GoalEndDate { get; set; }
    }

    /// <summary>
    /// A rollover envelope that sweeps into a given sinking fund envelope
    /// </summary>
    [ExportTsInterface]
    public class RolloverEnvelopeDependentModel
    {
        public int RolloverEnvelopeId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
    }

    /// <summary>
    /// Response model for the rollover envelopes that use a sinking fund as their transfer target
    /// </summary>
    [ExportTsInterface]
    public class GetSinkingFundDependentsResponse : BaseResponse
    {
        public List<RolloverEnvelopeDependentModel> RolloverEnvelopes { get; set; } = new List<RolloverEnvelopeDependentModel>();

        public GetSinkingFundDependentsResponse() : base() { }
        public GetSinkingFundDependentsResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }
}