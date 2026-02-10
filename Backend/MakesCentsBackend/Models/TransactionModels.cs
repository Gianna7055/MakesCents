/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
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


    public class SummaryTransactionResponse
    {
        // Class properties
        public int TransactionId { get; set; } = 0;
        public DateOnly Date { get; set; } = new DateOnly(1, 1, 1);
        public string Location { get; set; } = "";
        public string Envelopes { get; set; } = "";
        public decimal Amount { get; set; } = 0m;
    }


    public class SummaryTransactionDbModel
    {
        // Class properties
        public int TransactionId { get; set; } = 0;
        public DateOnly Date { get; set; } = new DateOnly(1, 1, 1);
        public TransactionType TransactionType { get; set; } = TransactionType.Unknown;
        public TransferTransactionType? TransferTransactionType { get; set; } = Enums.TransferTransactionType.Unknown;

        // Payment property
        public string? MerchantSourceName { get; set; } = null;
        public string? Envelopes { get; set; } = "";

        // Account transfer properties
        public string? TransferFromAccount { get; set; } = null;
        public string? TransferToAccount { get; set; } = null;

        // Envelope transfer properties
        public string? TransferFromEnvelope { get; set; } = null;
        public string? TransferToEnvelope { get; set; } = null;

        public decimal TotalAmount { get; set; } = 0m;
    }

    /// <summary>
    /// Request model to create a new transaction
    /// </summary>
    public abstract class CreateTransactionRequest
    {
        public int? BudgetId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public DateOnly? TransactionDate { get; set; } = null;
        public decimal? TotalAmount { get; set; } = null;
        public Optional<string?> Notes { get; set; } = null;
    }
}
