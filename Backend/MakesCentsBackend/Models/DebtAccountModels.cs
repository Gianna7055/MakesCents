/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a debt account
    /// </summary>
    public class DebtAccountEntity : AccountEntity
    {
        // Class Level Properties
        public int DebtAccountId { get; set; } = 0;
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
        public string? DebtAccountNumber { get; set; } = null;
        public DateOnly? DateOfNextBill { get; set; } = null;
        public decimal? AmountOfNextBill { get; set; } = null;
        public DebtPaymentRegularity? DebtPaymentRegularity { get; set; } = null;
    }

    /// <summary>
    /// Request model for creating a debt account model
    /// </summary>
    public class CreateDebtAccountRequest : CreateAccountRequest
    {
        // Class properties
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
        public Optional<int?> AccountNumber { get; set; } = null;
        public Optional<DateOnly?> DateOfNextBill { get; set; } = null;
        public Optional<decimal?> AmountOfNextBill { get; set; } = null;
        public Optional<DebtPaymentRegularity?> DebtPaymentRegularity { get; set; } = null;
    }
}
