/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a transfer transaction
    /// </summary>
    public class TransferTransactionEntity : TransactionEntity
    {
        // Class Level Properties
        public int TransferTransactionId { get; set; } = 0;
        public int? TransferFromAccountId { get; set; } = null;
        public int? TransferToAccountId { get; set; } = null;
        public int? TransferFromEnvelopeId { get; set; } = null;
        public int? TransferToEnvelopeId { get; set; } = null;
        public TransferTransactionType TransferTransactionType { get; set; } = TransferTransactionType.Unknown;
    }

    /// <summary>
    /// Request model ot create a new transfer transaction
    /// </summary>
    public class CreateTransferTransactionRequest : CreateTransactionRequest
    {
        // Class properties
        public int? TransferFromId { get; set; } = null;
        public int? TransferToId { get; set; } = null;
        public TransferTransactionType TransferTransactionType { get; set; } = TransferTransactionType.Unknown;
    }


    public class CreateTransferTransactionResponse : CreateTransactionResponse
    {
        // Class level properties
        public int? TransferTransactionId { get; set; } = null;

        /// <summary>
        /// Parameterized constructor for Create Transfer Transaction Response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreateTransferTransactionResponse(int httpStatus, string message) : base(httpStatus, message) { }

        /// <summary>
        /// Default constructor for Create Transfer Transaction Response
        /// </summary>
        public CreateTransferTransactionResponse() : base() { }
    }


    public class GetTransferTransactionEntityModel : GetTransactionEntityModel
    {
        // Class properties
        public int TransferTransactionId { get; set; } = 0;
        public int? TransferFromAccountId { get; set; } = null;
        public int? TransferToAccountId { get; set; } = null;
        public int? TransferFromEnvelopeId { get; set; } = null;
        public int? TransferToEnvelopeId { get; set; } = null;
        public TransferTransactionType TransferTransactionType { get; set; } = TransferTransactionType.Unknown;
    }


    public class GetTransferTransactionEntityResponse : BaseResponse
    {
        // Class properties
        public GetTransferTransactionEntityModel TransferTransaction { get; set; } = new GetTransferTransactionEntityModel();

        public GetTransferTransactionEntityResponse() : base() { }
        public GetTransferTransactionEntityResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }


    public class GetTransferTransactionDTOModel : GetTransactionDTOModel
    {
        // Class properties
        public int TransferTransactionId { get; set; } = 0;
        public int? TransferFromId { get; set; } = null;
        public int? TransferToId { get; set; } = null;
        public TransferTransactionType TransferTransactionType { get; set; } = TransferTransactionType.Unknown;
    }


    public class GetTransferTransactionDTOResponse : BaseResponse
    {
        // Class properties
        public GetTransferTransactionDTOModel TransferTransaction { get; set; } = new GetTransferTransactionDTOModel();

        public GetTransferTransactionDTOResponse() : base() { }
        public GetTransferTransactionDTOResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }
}
