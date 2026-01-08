/*
 * Gianna Ross
 * File Created: 12/5/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Debt Payment Regularity Enum
 * Sources: 
 */

namespace MakesCentsBackend.Models.Enums
{
    /// <summary>
    /// Enum for Debt Payment Regularities for the Debt Account Model
    /// </summary>
    public enum DebtPaymentRegularity
    {
        Unknown = 1,
        Weekly = 2,
        BiWeeklyEveryTwoWeeks = 3,
        Monthly = 4,
        BiMonthlyEveryTwoMonths = 5,
        Quarterly = 6,
        TwiceAnnually = 7,
        Annually = 8,
    }
}
