/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Envelope Model
 * Sources: 
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an envelope
    /// </summary>
    public class EnvelopeModel
    {
        // Class Level Properties
        public int EnvelopeId { get; set; } = 0;
        public int EnvelopeCategoryId { get; set; } = 0;
        public string EnvelopeName { get; set; } = "";
        public decimal PlannedAmount { get; set; } = 0m;
        public decimal RemainingAmount { get; set; } = 0m;
        public bool IsSinkingFund { get; set; } = true;

        // Only if IsSinkingFund is true
        public decimal? GoalAmount { get; set; } = 0m;
        public DateOnly? GoalEndDate { get; set; } = new DateOnly();

        // Only if IsSinkingFun is true
        public int? TransferEnvelopeId { get; set; } = null;


        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();
    }
}
