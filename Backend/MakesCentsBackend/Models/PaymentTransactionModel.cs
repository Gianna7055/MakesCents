/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Payment Transaction Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a payment transaction
    /// </summary>
    public class PaymentTransactionModel : TransactionModel
    {
        // Class Level Properties
        public int PaymentTransactionId { get; set; } = 0;
        public int AccountId { get; set; } = 0;
        public PaymentTransactionType PaymentTransactionType { get; set; } = PaymentTransactionType.Unknown;
        public string MerchantSourceName { get; set; } = "";
        public int? CheckNumber { get; set; } = null;
        public List<TransactionSplitModel> TransactionSplits { get; set; } = new List<TransactionSplitModel>();
    }
}
