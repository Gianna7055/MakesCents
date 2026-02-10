/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a bank account
    /// </summary>
    public class BankAccountEntity : AccountEntity
    {
        // Class Level Properties
        public int BankAccountId { get; set; } = 0;
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
    }

    /// <summary>
    /// Request model for creating a bank account model
    /// </summary>
    public class CreateBankAccountRequest : CreateAccountRequest
    {
        // Class properties
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
    }
}
