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
    public class PaycheckSplitEntity
    {
        // Class Level Properties
        public int PaycheckSplitId { get; set; } = 0;
        public int PaycheckId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal Amount { get; set; } = 0m;
        public int OrderIndex { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
    }

    /// <summary>
    /// Request model to create a paycheck split
    /// </summary>
    public class CreatePaycheckSplitRequest
    {
        public int? PaycheckId { get; set; } = null;
        public int? EnvelopeId { get; set; } = null;
        public decimal? Amount { get; set; } = null;
    }


    public class GetPaycheckSplitDTOModel
    {
        // Class properties
        public int PaycheckSplitId { get; set; } = 0;
        public int PaycheckId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal? Amount { get; set; } = 0m;
    }
}