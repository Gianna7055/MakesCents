/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a transaction split model
    /// </summary>
    public class TransactionSplitEntityModel
    {
        // Class Level Properties
        public int TransactionSplitId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal Amount { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Request model for creating a new transaction split
    /// </summary>
    public class CreateTransactionSplitRequest
    {
        public int TransactionId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal Amount { get; set; } = 0m;
    }


    public class GetTransactionSplitDTOModel
    {
        // Class properties
        public int TransactionSplitId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal Amount { get; set; } = 0m;
    }


    public class UpdateTransactionSplitRequest
    {
        // Class variables
        public int? TransactionId { get; set; }
        public int? TransactionSplitId { get; set; }
        public decimal? Amount { get; set; }
        public int? EnvelopeId { get; set; }
    }
}
