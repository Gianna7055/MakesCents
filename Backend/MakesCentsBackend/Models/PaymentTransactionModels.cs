/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Entities;
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a payment transaction
    /// </summary>
    public class PaymentTransactionEntity : TransactionEntity
    {
        // Class Level Properties
        public int PaymentTransactionId { get; set; } = 0;
        public int AccountId { get; set; } = 0;
        public PaymentTransactionType PaymentTransactionType { get; set; } = PaymentTransactionType.Unknown;
        public string MerchantSourceName { get; set; } = "";
        public int? CheckNumber { get; set; } = null;
        public List<TransactionSplitEntity> TransactionSplits { get; set; } = new List<TransactionSplitEntity>();
    }

    /// <summary>
    /// Request model to create a new payment transaction
    /// </summary>
    public class CreatePaymentTransactionRequest : CreateTransactionRequest
    {
        // Class properties
        public int? AccountId { get; set; } = null;
        public PaymentTransactionType? PaymentTransactionType { get; set; } = Enums.PaymentTransactionType.Unknown;
        public string? MerchantSourceName { get; set; } = null;
        public Optional<int?> CheckNumber { get; set; } = null;
        public List<CreateTransactionSplitRequest> TransactionSplits { get; set; } = new List<CreateTransactionSplitRequest>();
    }

    /// <summary>
    /// Response model for creating a payment transaction
    /// </summary>
    public class CreatePaymentTransactionResponse
    {

        // Class level properties
        public int HttpStatus { get; set; } = 0;
        public string? Message { get; set; } = null;
        public int? PaymentTransactionId { get; set; } = null;
        public List<int> TransactionSplitIds { get; set; } = new List<int>();

        /// <summary>
        /// Parameterized constructor for Create Payment Transaction Response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreatePaymentTransactionResponse(int httpStatus, string? message)
        {
            HttpStatus = httpStatus;
            Message = message;
        }

        /// <summary>
        /// Default constructor for Create Payment Transaction Response
        /// </summary>
        public CreatePaymentTransactionResponse()
        {
        }
    }
}
