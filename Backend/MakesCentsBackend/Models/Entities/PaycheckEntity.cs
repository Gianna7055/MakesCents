/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Paycheck Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models.Entities
{
    /// <summary>
    /// Model for a paycheck
    /// </summary>
    public class PaycheckEntity
    {
        // Class Level Properties
        public int PaycheckId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public DateOnly StartingDate { get; set; } = new DateOnly();
        public DateOnly? SecondaryDate { get; set; } = null;
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<PaycheckSplitEntity> PaycheckSplits { get; set; } = new List<PaycheckSplitEntity>();
    }
}
