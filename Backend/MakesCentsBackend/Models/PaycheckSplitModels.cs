/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a paycheck split
    /// </summary>
    public class PaycheckSplitEntityModel
    {
        // Class Level Properties
        public int PaycheckSplitId { get; set; } = 0;
        public int PaycheckId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal Amount { get; set; } = 0m;
        public int OrderIndex { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Request model to create a paycheck split
    /// </summary>
    public class CreatePaycheckSplitRequest
    {
        public int PaycheckId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal Amount { get; set; } = 0m;
    }


    public class GetPaycheckSplitDTOModel
    {
        // Class properties
        public int PaycheckSplitId { get; set; } = 0;
        public int PaycheckId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal? Amount { get; set; } = 0m;
    }


    public class UpdatePaycheckSplitRequest
    {
        // Class variables
        public int? PaycheckId { get; set; }
        public int? PaycheckSplitId { get; set; }
        public decimal? Amount { get; set; }
        public int? EnvelopeId { get; set; }
    }
}