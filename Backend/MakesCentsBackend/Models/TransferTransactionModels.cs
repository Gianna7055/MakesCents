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
    /// Model for a transfer transaction
    /// </summary>
    public class TransferTransactionEntityModel : TransactionEntityModel
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
    /// Request model to create a new transfer transaction
    /// </summary>
    [ExportTsClass]
    public class CreateTransferTransactionRequest : CreateTransactionRequest
    {
        // Class properties
        public int TransferFromId { get; set; } = 0;
        public int TransferToId { get; set; } = 0;
        public TransferTransactionType TransferTransactionType { get; set; } = TransferTransactionType.Unknown;
    }


    [ExportTsInterface]
    public class CreateTransferTransactionResponse : BaseIdResponse
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


    [ExportTsInterface]
    public class GetTransferTransactionDTOModel : GetTransactionDTOModel
    {
        // Class properties
        public int TransferTransactionId { get; set; } = 0;
        public int? TransferFromId { get; set; } = null;
        public int? TransferToId { get; set; } = null;
        public TransferTransactionType TransferTransactionType { get; set; } = TransferTransactionType.Unknown;
    }


    [ExportTsInterface]
    public class GetTransferTransactionDTOResponse : BaseResponse
    {
        // Class properties
        public GetTransferTransactionDTOModel TransferTransaction { get; set; } = new GetTransferTransactionDTOModel();

        public GetTransferTransactionDTOResponse() : base() { }
        public GetTransferTransactionDTOResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }


    [ExportTsClass]
    public class UpdateTransferTransactionRequest : UpdateTransactionRequest
    {
        // Class properties
        public int TransferTransactionId { get; set; } = 0;
        public int? TransferFromId { get; set; }
        public int? TransferToId { get; set; }
        public TransferTransactionType? TransferTransactionType { get; set; }
    }
}
