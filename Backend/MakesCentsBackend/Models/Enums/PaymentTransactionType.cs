/*
 * Gianna Ross
 * Makes Cents
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
