/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Debt Account Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a debt account
    /// </summary>
    public class DebtAccountModel : AccountModel
    {
        // Class Level Properties
        public int DebtAccountId { get; set; } = 0;
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
        public string? DebtAccountNumber { get; set; } = null;
        public DateOnly? DateOfNextBill { get; set; } = null;
        public decimal? AmountOfNextBill { get; set; } = null;
        public DebtPaymentRegularity? DebtPaymentRegularity { get; set; } = null;
    }
}
