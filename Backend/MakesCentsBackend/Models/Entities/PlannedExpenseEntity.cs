/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Planned Expense Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models.Entities
{
    /// <summary>
    /// Model for a planned expense
    /// </summary>
    public class PlannedExpenseEntity
    {
        // Class Level Properties
        public int PlannedExpenseId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public PlannedExpenseRegularity PlannedExpenseRegularity { get; set; } = PlannedExpenseRegularity.Unknown;
        public int? DayOfMonth { get; set; } = null;
        public Weekday? Weekday { get; set; } = null;
        public PlannedExpenseOccurrence? PlannedExpenseOccurrence { get; set; } = null;
        public decimal Amount { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
    }
}
