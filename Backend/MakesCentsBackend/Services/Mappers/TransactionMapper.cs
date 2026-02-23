/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/697b1f88-33e8-8332-8560-f5fa7b8c624e
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Services.Mappers
{
    public static class TransactionMapper
    {
        /// <summary>
        /// Mapper for summary transaction responses
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static SummaryTransactionDTOModel ToSummaryResponse(SummaryTransactionEntityModel db)
        {
            SummaryTransactionDTOModel response = new()
            {
                Date = db.Date,
                Location = GetLocation(db),
                Envelopes = GetEnvelopeDisplay(db),
                Amount = db.TotalAmount
            };

            return response;
        }

        /// <summary>
        /// Method to format the location for a transaction
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static string GetLocation(SummaryTransactionEntityModel db)
        {
            if (db.TransactionType == TransactionType.Payment)
            {
                if (!string.IsNullOrEmpty(db.MerchantSourceName))
                {
                    return db.MerchantSourceName;
                }

                return "Payment";
            }

            if (db.TransactionType == TransactionType.Transfer && db.TransferTransactionType == TransferTransactionType.Envelope)
            {
                return "Envelope Transfer";
            }

            if (db.TransactionType == TransactionType.Transfer && db.TransferTransactionType == TransferTransactionType.Account)
            {
                return "Account Transfer";
            }

            return "Transaction";
        }

        /// <summary>
        /// Method to format the envelope for a transaction
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static string GetEnvelopeDisplay(SummaryTransactionEntityModel db)
        {
            // Return the envelopes name if the transaction is a payment
            if (db.TransactionType == TransactionType.Payment && !string.IsNullOrEmpty(db.EnvelopeNames))
            {
                return db.EnvelopeNames;
            }
            // Return the envelope names if the transaction is an envelope transaction
            if (db.TransactionType == TransactionType.Transfer && db.TransferTransactionType == TransferTransactionType.Envelope)
            {
                return db.TransferFromEnvelope + " -> " + db.TransferToEnvelope;
            }
            // Return the account names if the transaction is an account transaction
            if (db.TransactionType == TransactionType.Transfer && db.TransferTransactionType == TransferTransactionType.Account)
            {
                return db.TransferFromAccount + " -> " + db.TransferToAccount;
            }

            return string.Empty;
        }
    }

}
