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
    /// Model for a transfer transaction.
    /// v3's transfer_transaction table only carries account columns - an envelope-to-envelope
    /// transfer is a transfer_transaction row of type Envelope with null account columns, paired
    /// with two opposite-signed transaction_split rows that carry the actual envelope references.
    /// </summary>
    public class TransferTransactionEntityModel : TransactionEntityModel
    {
        // Class Level Properties
        public int TransferTransactionId { get; set; } = 0;
        public int? TransferFromAccountId { get; set; } = null;
        public int? TransferToAccountId { get; set; } = null;
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

        // Only meaningful when TransferTransactionType == Envelope - says whether TransferFromId/
        // TransferToId refer to a sinking_fund_envelope_id or the persistent rollover_envelope_id.
        public bool? TransferFromIsSinkingFund { get; set; } = null;
        public bool? TransferToIsSinkingFund { get; set; } = null;
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


    /// <summary>
    /// Entity model for a specific transfer transaction. TransferFromEnvelopeId/ToEnvelopeId and
    /// their IsSinkingFund flags are only populated (via a follow-up transaction_split lookup) when
    /// TransferTransactionType is Envelope - they aren't columns on transfer_transaction itself.
    /// </summary>
    public class GetTransferTransactionEntityModel : GetTransactionEntityModel
    {
        // Class properties
        public int TransferTransactionId { get; set; } = 0;
        public int? TransferFromAccountId { get; set; } = null;
        public int? TransferToAccountId { get; set; } = null;
        public TransferTransactionType TransferTransactionType { get; set; } = TransferTransactionType.Unknown;
        public int? TransferFromEnvelopeId { get; set; } = null;
        public int? TransferToEnvelopeId { get; set; } = null;
        public bool? TransferFromIsSinkingFund { get; set; } = null;
        public bool? TransferToIsSinkingFund { get; set; } = null;
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

        // Only meaningful when TransferTransactionType == Envelope
        public bool? TransferFromIsSinkingFund { get; set; } = null;
        public bool? TransferToIsSinkingFund { get; set; } = null;
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

        // Only meaningful when the (new or existing) TransferTransactionType is Envelope
        public bool? TransferFromIsSinkingFund { get; set; }
        public bool? TransferToIsSinkingFund { get; set; }
    }
}