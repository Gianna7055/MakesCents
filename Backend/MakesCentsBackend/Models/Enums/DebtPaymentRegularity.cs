/*
 * Gianna Ross
 * Makes Cents
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
