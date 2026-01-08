/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/5/2025
 * Makes Cents - Paycheck Split Model
 * Sources: 
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a paycheck split
    /// </summary>
    public class PaycheckSplitModel
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
}
