/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an abstract transaction
    /// </summary>
    public abstract class TransactionEntityModel
    {
        // Class Level Properties
        public int TransactionId { get; set; } = 0;
        public int BudgetMonthId { get; set; } = 0;
        public DateOnly TransactionDate { get; set; } = DateOnly.MinValue;
        public decimal TotalAmount { get; set; } = 0m;
        public bool IsReconciled { get; set; } = false;
        public string? Notes { get; set; } = null;
        public TransactionType TransactionType { get; set; } = TransactionType.Unknown;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
        public DateTime? DeletedAt { get; set; } = null;
    }


    [ExportTsInterface]
    public class SummaryTransactionDTOModel
    {
        // Class properties
        public int TransactionId { get; set; } = 0;
        public DateOnly Date { get; set; } = DateOnly.MinValue;
        public string Location { get; set; } = "";
        public string Envelopes { get; set; } = "";
        public decimal Amount { get; set; } = 0m;
    }


    public class SummaryTransactionEntityModel
    {
        // Class properties
        public int TransactionId { get; set; } = 0;
        public DateOnly Date { get; set; } = DateOnly.MinValue;
        public TransactionType TransactionType { get; set; } = TransactionType.Unknown;
        public TransferTransactionType? TransferTransactionType { get; set; } = Enums.TransferTransactionType.Unknown;

        // Payment property
        public string? MerchantSourceName { get; set; } = null;
        public string? EnvelopeNames { get; set; } = "";

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
    [ExportTsClass]
    public abstract class CreateTransactionRequest
    {
        public int BudgetMonthId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public DateOnly TransactionDate { get; set; } = DateOnly.MinValue;
        public decimal TotalAmount { get; set; } = 0m;
        public Optional<string?> Notes { get; set; }
    }


    public class GetAllTransactionsEntityResponse : BaseResponse
    {
        // Class properties
        public List<SummaryTransactionEntityModel> Transactions { get; set; } = new List<SummaryTransactionEntityModel>();

        public GetAllTransactionsEntityResponse() : base() { }
        public GetAllTransactionsEntityResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }


    [ExportTsInterface]
    public class GetAllTransactionsDTOResponse : BaseResponse
    {
        // Class properties
        public List<SummaryTransactionDTOModel> Transactions { get; set; } = new List<SummaryTransactionDTOModel>();

        public GetAllTransactionsDTOResponse(int httpStatus, string message) : base(httpStatus, message) { }
        public GetAllTransactionsDTOResponse() : base() { }
    }


    [ExportTsInterface]
    public class GetTransactionDTOModel
    {
        // Class variables
        public int TransactionId { get; set; } = 0;
        public int BudgetMonthId { get; set; } = 0;
        public DateOnly TransactionDate { get; set; } = DateOnly.MinValue;
        public decimal TotalAmount { get; set; } = 0m;
        public bool IsReconciled { get; set; } = false;
        public string? Notes { get; set; } = null;
        public TransactionType TransactionType { get; set; } = TransactionType.Unknown;
    }

    public class GetTransactionEntityModel
    {
        // Class variables
        public int TransactionId { get; set; } = 0;
        public int BudgetMonthId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public DateOnly TransactionDate { get; set; } = DateOnly.MinValue;
        public decimal TotalAmount { get; set; } = 0m;
        public bool IsReconciled { get; set; } = false;
        public string? Notes { get; set; } = null;
        public TransactionType TransactionType { get; set; } = TransactionType.Unknown;
    }


    [ExportTsClass]
    public class UpdateTransactionRequest
    {
        // Class properties
        public int UserId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public DateOnly? TransactionDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public Optional<string?> Notes { get; set; }
    }

    /// <summary>
    /// Internal helper: the current account_id/total_amount/budget_month_id for a payment
    /// transaction, read before an update is applied so the old account-balance effect can be
    /// reversed and the new one applied cleanly.
    /// </summary>
    public class PaymentTransactionSnapshotModel
    {
        public int AccountId { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0m;
        public int BudgetMonthId { get; set; } = 0;
        // True if the transaction is soft-deleted (updates are blocked while it is)
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// Internal helper: the current type/accounts/total_amount/budget_month_id for a transfer
    /// transaction, read before an update is applied so the old effect (account balances or
    /// envelope remaining amounts, depending on type) can be reversed and the new one applied.
    /// </summary>
    public class TransferTransactionSnapshotModel
    {
        public TransactionType TransactionType { get; set; } = TransactionType.Unknown;
        public TransferTransactionType TransferTransactionType { get; set; } = TransferTransactionType.Unknown;
        public int? TransferFromAccountId { get; set; } = null;
        public int? TransferToAccountId { get; set; } = null;
        public decimal TotalAmount { get; set; } = 0m;
        public int BudgetMonthId { get; set; } = 0;
        // True if the transaction is soft-deleted (updates are blocked while it is)
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// Internal helper: what a transaction currently does to account balances, plus whether it is
    /// soft-deleted. Used by the delete/restore methods to reverse or re-apply its effect.
    /// </summary>
    public class TransactionEffectSnapshotModel
    {
        public decimal TotalAmount { get; set; } = 0m;
        public bool IsDeleted { get; set; } = false;
        // Set for payment transactions
        public int? PaymentAccountId { get; set; } = null;
        // Set for account-to-account transfers
        public int? TransferFromAccountId { get; set; } = null;
        public int? TransferToAccountId { get; set; } = null;
    }
}