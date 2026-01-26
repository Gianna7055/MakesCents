/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/5/2025
 * Makes Cents - Bank Account Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models.Entities
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
}
