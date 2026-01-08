/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/5/2025
 * Makes Cents - Transaction Split Model
 * Sources: 
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a transaction split model
    /// </summary>
    public class TransactionSplitModel
    {
        // Class Level Properties
        public int TransactionSplitId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public decimal Amount { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
    }
}
