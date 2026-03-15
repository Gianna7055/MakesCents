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
    /// Model for a payment transaction
    /// </summary>
    public class PaymentTransactionEntityModel : TransactionEntityModel
    {
        // Class Level Properties
        public int PaymentTransactionId { get; set; } = 0;
        public int AccountId { get; set; } = 0;
        public PaymentTransactionType PaymentTransactionType { get; set; } = PaymentTransactionType.Unknown;
        public string MerchantSourceName { get; set; } = "";
        public int? CheckNumber { get; set; } = null;
        public List<TransactionSplitEntityModel> TransactionSplits { get; set; } = new List<TransactionSplitEntityModel>();
    }

    /// <summary>
    /// Request model to create a new payment transaction
    /// </summary>
    [ExportTsClass]
    public class CreatePaymentTransactionRequest : CreateTransactionRequest
    {
        // Class properties
        public int AccountId { get; set; } = 0;
        public PaymentTransactionType PaymentTransactionType { get; set; } = Enums.PaymentTransactionType.Unknown;
        public string MerchantSourceName { get; set; } = "";
        public Optional<int?> CheckNumber { get; set; }
        public List<CreateTransactionSplitRequest> TransactionSplits { get; set; } = new List<CreateTransactionSplitRequest>();
    }

    /// <summary>
    /// Response model for creating a payment transaction
    /// </summary>
    [ExportTsInterface]
    public class CreatePaymentTransactionResponse : BaseIdResponse
    {

        // Class level properties
        public int? PaymentTransactionId { get; set; } = null;
        public List<int> TransactionSplitIds { get; set; } = new List<int>();

        /// <summary>
        /// Parameterized constructor for Create Payment Transaction Response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreatePaymentTransactionResponse(int httpStatus, string message) : base(httpStatus, message) { }

        /// <summary>
        /// Default constructor for Create Payment Transaction Response
        /// </summary>
        public CreatePaymentTransactionResponse() : base() { }
    }


    [ExportTsInterface]
    public class GetPaymentTransactionDTOModel : GetTransactionDTOModel
    {
        // Class properties
        public int PaymentTransactionId { get; set; } = 0;
        public int AccountId { get; set; } = 0;
        public PaymentTransactionType PaymentTransactionType { get; set; } = PaymentTransactionType.Unknown;
        public string MerchantSourceName { get; set; } = "";
        public int? CheckNumber { get; set; } = null;
        public List<GetTransactionSplitDTOModel> TransactionSplits { get; set; } = new List<GetTransactionSplitDTOModel>();
    }


    [ExportTsInterface]
    public class GetPaymentTransactionResponse : BaseResponse
    {
        // Class properties
        public GetPaymentTransactionDTOModel PaymentTransaction { get; set; } = new GetPaymentTransactionDTOModel();

        public GetPaymentTransactionResponse() : base() { }
        public GetPaymentTransactionResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }


    [ExportTsClass]
    public class UpdatePaymentTransactionRequest : UpdateTransactionRequest
    {
        // Class properties
        public int PaymentTransactionId { get; set; } = 0;
        public int? AccountId { get; set; }
        public PaymentTransactionType? PaymentTransactionType { get; set; }
        public string? MerchantSourceName { get; set; }
        public Optional<int?> CheckNumber { get; set; }
        public List<UpdateTransactionSplitRequest> TransactionSplits { get; set; } = new List<UpdateTransactionSplitRequest>();
    }


    [ExportTsInterface]
    public class UpdatePaymentTransactionResponse : BaseIdResponse
    {
        // Class properties
        public List<int> TransactionSplitIds { get; set; } = new List<int>();

        public UpdatePaymentTransactionResponse(int httpStatus, string message) : base(httpStatus, message) { }
        public UpdatePaymentTransactionResponse(int httpStatus, string message, int id) : base(httpStatus, message, id) { }

        public UpdatePaymentTransactionResponse() : base() { }
    }
}   
