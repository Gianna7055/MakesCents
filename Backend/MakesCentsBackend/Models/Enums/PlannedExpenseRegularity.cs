/*
 * Gianna Ross
 * File Created: 12/15/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Planned Expense Regularity Enum
 * Sources: 
 */
namespace MakesCentsBackend.Models.Enums
{
    /// <summary>
    /// Enum for Planned Expense Regularities for the Planned Expense Model
    /// </summary>
    public enum PlannedExpenseRegularity
    {
        Unknown = 1,
        DayOfMonth = 2,
        LastDayOfMonth = 3,
        WeekdayOccurrence = 4
    }
}
