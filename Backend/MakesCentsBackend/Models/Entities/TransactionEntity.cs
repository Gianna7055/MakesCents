/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Transaction Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models.Entities
{
    /// <summary>
    /// Model for an abstract transaction
    /// </summary>
    public abstract class TransactionEntity
    {
        // Class Level Properties
        public int TransactionId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public DateOnly TransactionDate { get; set; } = new DateOnly();
        public decimal TotalAmount { get; set; } = 0m;
        public bool IsReconciled { get; set; } = false;
        public string? Notes { get; set; } = null;
        public TransactionType TransactionType { get; set; } = TransactionType.Unknown;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public DateTime? DeletedAt { get; set; } = null;
    }
}
