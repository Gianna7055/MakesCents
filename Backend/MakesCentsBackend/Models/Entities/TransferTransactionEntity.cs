/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Transfer Transaction Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models.Entities
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
}
