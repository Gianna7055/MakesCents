/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a transaction split model.
    /// EnvelopeId is a display/API-level id - IsSinkingFund says whether it refers to a
    /// sinking_fund_envelope_id or the persistent rollover_envelope_id (not the month-specific
    /// rollover_envelope_month_id, which is an internal DAO detail).
    /// </summary>
    public class TransactionSplitEntityModel
    {
        // Class Level Properties
        public int TransactionSplitId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal Amount { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Request model for creating a new transaction split
    /// </summary>
    [ExportTsClass]
    public class CreateTransactionSplitRequest
    {
        public int TransactionId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal Amount { get; set; } = 0m;
    }


    [ExportTsInterface]
    public class GetTransactionSplitDTOModel
    {
        // Class properties
        public int TransactionSplitId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal Amount { get; set; } = 0m;
    }


    [ExportTsClass]
    public class UpdateTransactionSplitRequest
    {
        // Class variables
        public int? TransactionId { get; set; }
        public int? TransactionSplitId { get; set; }
        public decimal? Amount { get; set; }
        public int? EnvelopeId { get; set; }
        public bool? IsSinkingFund { get; set; }
    }

    /// <summary>
    /// Internal helper for reading one side of an envelope-to-envelope transfer back out of
    /// transaction_split (used by TransferTransactionDAO/TransactionDAO, not exposed to the API).
    /// </summary>
    public class TransactionSplitTransferSideModel
    {
        public decimal Amount { get; set; } = 0m;
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
    }

    /// <summary>
    /// Internal helper: the raw current state of one transaction_split row, used to reverse its
    /// old effect on an envelope's remaining amount before an update applies the new one.
    /// </summary>
    public class TransactionSplitSnapshotModel
    {
        public int TransactionSplitId { get; set; } = 0;
        public int? SinkingFundEnvelopeId { get; set; } = null;
        public int? RolloverEnvelopeMonthId { get; set; } = null;
        public decimal Amount { get; set; } = 0m;
    }
}