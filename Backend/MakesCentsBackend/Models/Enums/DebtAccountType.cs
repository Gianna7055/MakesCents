/*
 * Gianna Ross
 * File Created: 12/5/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Debt Account Type Enum
 * Sources: 
 */
namespace MakesCentsBackend.Models.Enums
{
    /// <summary>
    /// Enum for Debt Account Types for the Debt Account Model
    /// </summary>
    public enum DebtAccountType
    {
        Unknown = 1,
        CreditCard = 2,
        LineOfCreditLoan = 3,
        CarLoan = 4,
        Mortgage = 5
    }
}
