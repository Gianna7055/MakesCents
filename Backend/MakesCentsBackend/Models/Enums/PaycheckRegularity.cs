/*
 * Gianna Ross
 * File Created: 12/14/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Paycheck Regularity Enum
 * Sources: 
 */
namespace MakesCentsBackend.Models.Enums
{
    /// <summary>
    /// Enum for Paycheck Regularities for the Paycheck Model
    /// </summary>
    public enum PaycheckRegularity
    {
        Unknown = 1,
        Weekly = 2, 
        BiWeeklyEveryTwoWeeks = 3,
        BiMonthlyTwiceAMonth = 4,
        Monthly = 5
    }
}
