/*
 * Gianna Ross
 * File Created: 12/14/2025
 * File Last Updated: 12/14/2025
 * Makes Cents - Payment Transaction Type Enum
 * Sources: 
 */
namespace MakesCentsBackend.Models.Enums
{
    /// <summary>
    /// Enum for Transfer Transaction Types for the Transfer Transaction Model
    /// </summary>
    public enum TransferTransactionType
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
