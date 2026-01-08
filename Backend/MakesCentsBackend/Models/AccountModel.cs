/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/5/2025
 * Makes Cents - Account Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an abstract account
    /// </summary>
    public abstract class AccountModel
    {
        // Class Level Properties
        public int AccountId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public AccountType AccountType { get; set; } = AccountType.Unknown;
        public string AccountName { get; set; } = "";
        public string Institution { get; set; } = "";
        public decimal Balance { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();
    }
}
