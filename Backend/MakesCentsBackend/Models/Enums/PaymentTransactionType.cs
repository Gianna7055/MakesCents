/*
 * Gianna Ross
 * File Created: 12/14/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Payment Transaction Type Enum
 * Sources: 
 */
namespace MakesCentsBackend.Models.Enums
{
    /// <summary>
    /// Enum for Payment Transaction Types for the Payment Transaction Model
    /// </summary>
    public enum PaymentTransactionType
    {
        Unknown = 1,
        ATM = 2,
        Check = 3,
        DebitCard = 4,
        CreditCard = 5,
        Deposit = 6,
        Paycheck = 7,
        Refund = 8,
        LoanDeposit = 9
    }
}
