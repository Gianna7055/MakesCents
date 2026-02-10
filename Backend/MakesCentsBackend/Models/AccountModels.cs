/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an abstract account
    /// </summary>
    public abstract class AccountEntity
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
        public List<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
    }


    public abstract class CreateAccountRequest
    {
        // Class properties
        public int? BudgetId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public int? AccountId { get; set; } = null;
        public string? AccountName { get; set; } = null;
        public string? Institution { get; set; } = null;
        public decimal? Balance { get; set; } = null;
    }
}
