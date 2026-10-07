/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a single envelope funding row - one envelope's planned funding amount and
    /// priority order for a specific budget month. This is a plan, not a money movement -
    /// actual money still moves into envelopes via transactions.
    /// EnvelopeId is a display/API-level id - IsSinkingFund says whether it refers to a
    /// sinking_fund_envelope_id or the persistent rollover_envelope_id (not the month-specific
    /// rollover_envelope_month_id, which is an internal DAO detail).
    /// </summary>
    public class EnvelopeFundingEntityModel
    {
        // Class Level Properties
        public int EnvelopeFundingId { get; set; } = 0;
        public int BudgetMonthId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal PlannedAmount { get; set; } = 0m;
        public int OrderIndex { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Request model for a single item within a funding queue being set. EnvelopeFundingId is
    /// null for a brand new row, or the existing row's id to update it in place.
    /// </summary>
    [ExportTsClass]
    public class SetEnvelopeFundingItemRequest
    {
        // Class properties
        public int? EnvelopeFundingId { get; set; }
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal PlannedAmount { get; set; } = 0m;
    }

    /// <summary>
    /// Request model to set (create/reorder/update/remove all at once) the funding queue for a
    /// budget month. The list's order is the funding priority - item 0 is funded first.
    /// </summary>
    [ExportTsClass]
    public class SetEnvelopeFundingQueueRequest
    {
        // Class properties
        public int BudgetMonthId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public List<SetEnvelopeFundingItemRequest> FundingQueue { get; set; } = new List<SetEnvelopeFundingItemRequest>();
    }

    /// <summary>
    /// DTO model for a single envelope funding row
    /// </summary>
    [ExportTsInterface]
    public class GetEnvelopeFundingDTOModel
    {
        // Class properties
        public int EnvelopeFundingId { get; set; } = 0;
        public int BudgetMonthId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal PlannedAmount { get; set; } = 0m;
        public int OrderIndex { get; set; } = 0;
    }

    /// <summary>
    /// Response model for a budget month's funding queue, in priority order
    /// </summary>
    [ExportTsInterface]
    public class GetEnvelopeFundingQueueResponse : BaseResponse
    {
        // Class properties
        public List<GetEnvelopeFundingDTOModel> FundingQueue { get; set; } = new List<GetEnvelopeFundingDTOModel>();

        /// <summary>
        /// Default constructor for the get envelope funding queue response
        /// </summary>
        public GetEnvelopeFundingQueueResponse() : base() { }

        /// <summary>
        /// Parameterized constructor with a status and a message
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public GetEnvelopeFundingQueueResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }
}