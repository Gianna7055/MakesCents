/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */

using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an envelope
    /// </summary>
    public class EnvelopeEntityModel
    {
        // Class Level Properties
        public int EnvelopeId { get; set; } = 0;
        public int EnvelopeCategoryId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public decimal PlannedAmount { get; set; } = -1m;
        public decimal RemainingAmount { get; set; } = -1m;
        public bool IsSinkingFund { get; set; } = true;

        // Only if IsSinkingFund is true
        public decimal? GoalAmount { get; set; } = null;
        public DateOnly? GoalEndDate { get; set; } = null;

        // Only if IsSinkingFun is true
        public int? TransferEnvelopeId { get; set; } = null;


        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
        public List<TransactionEntityModel> Transactions { get; set; } = new List<TransactionEntityModel>();
    }

    /// <summary>
    /// Response model for an envelope for the Get Budget method
    /// </summary>
    [ExportTsInterface]
    public class SummaryEnvelopeResponse
    {
        // Class Level Properties
        public int EnvelopeId { get; set; } = 0;
        public int EnvelopeCategoryId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public decimal RemainingAmount { get; set; } = 0m;
    }

    /// <summary>
    /// Request class for creating an envelope
    /// </summary>
    [ExportTsClass]
    public class CreateEnvelopeRequest
    {
        // Class properties
        public int EnvelopeCategoryId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public decimal PlannedAmount { get; set; } = 0m;
        public decimal RemainingAmount { get; set; } = 0m;
        public bool IsSinkingFund { get; set; } = false;
        public Optional<decimal?> GoalAmount { get; set; }
        public Optional<DateOnly?> GoalEndDate { get; set; }
        public Optional<int?> TransferEnvelopeId { get; set; }
    }

    [ExportTsInterface]
    public class GetEnvelopeBaseModel
    {
        // Class properties
        public int EnvelopeId { get; set; } = 0;
        public int EnvelopeCategoryId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public decimal PlannedAmount { get; set; } = -1m;
        public decimal RemainingAmount { get; set; } = -1m;

        /// <summary>
        /// Indicates whether the envelope is a sinking fund.
        /// True = sinking fund
        /// False = rollover
        /// </summary>
        public bool IsSinkingFund { get; set; }

        /// <summary>
        /// The target amount for a sinking fund.
        /// Required when <see cref="IsSinkingFund"/> is true.
        /// Null when <see cref="IsSinkingFund"/> is false (rollover envelope).
        /// </summary>
        public decimal? GoalAmount { get; set; } = null;

        /// <summary>
        /// The date by which the sinking fund goal should be reached.
        /// Required when <see cref="IsSinkingFund"/> is true.
        /// Null when <see cref="IsSinkingFund"/> is false (rollover envelope).
        /// </summary>
        public DateOnly? GoalEndDate { get; set; } = null;

        /// <summary>
        /// The envelope ID to which remaining funds will be transferred.
        /// Required when <see cref="IsSinkingFund"/> is false.
        /// Null when <see cref="IsSinkingFund"/> is true (sinking fund envelope).
        /// </summary>
        public int? TransferEnvelopeId { get; set; } = null;
    }

    [ExportTsInterface]
    public class GetEnvelopeDTOModel : GetEnvelopeBaseModel
    {
        // Class properties
        public List<SummaryTransactionDTOModel> Transactions { get; set; } = new List<SummaryTransactionDTOModel>();
    }

    /// <summary>
    /// Response model for getting a specific envelope
    /// </summary>
    [ExportTsInterface]
    public class GetEnvelopeDTOResponse : BaseResponse
    {
        // Class properties
        public GetEnvelopeDTOModel EnvelopeDTO { get; set; } = new GetEnvelopeDTOModel();

        public GetEnvelopeDTOResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }


    /// <summary>
    /// Entity model for getting a specific envelope
    /// </summary>
    public class GetEnvelopeEntityModel : GetEnvelopeBaseModel
    {
        // Class properties
        public List<SummaryTransactionEntityModel> Transactions { get; set; } = [];
    }

    /// <summary>
    /// Entity response model for getting a specific envelope
    /// </summary>
    public class GetEnvelopeEntityResponse : BaseResponse
    {
        // Class properties
        public GetEnvelopeEntityModel EnvelopeEntity { get; set; } = new GetEnvelopeEntityModel();


        /// <summary>
        /// Default constructor for GetEnvelopeEntityResponse
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="allEnvelopeCategories"></param>
        public GetEnvelopeEntityResponse() : base() { }

        /// <summary>
        /// Parameterized constructor for GetEnvelopeEntityResponse
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="allEnvelopeCategories"></param>
        public GetEnvelopeEntityResponse(int status, string message) : base(status, message) { }
    }


    [ExportTsClass]
    public class UpdateEnvelopeRequest
    {
        // Class properties
        public int EnvelopeId { get; set; } = 0;
        public int? EnvelopeCategoryId { get; set; } = null;
        public int UserId { get; set; } = 0;
        public string? EnvelopeName { get; set; } = null;
        public decimal? PlannedAmount { get; set; } = null;
        public bool? IsSinkingFund { get; set; } = null;
        public Optional<decimal?> GoalAmount { get; set; }
        public Optional<DateOnly?> GoalEndDate { get; set; }
        public Optional<int?> TransferEnvelopeId { get; set; }
    }
}
