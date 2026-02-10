/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an investment account
    /// </summary>
    public class InvestmentAccountEntity : AccountEntity
    {
        // Class Level Properties
        public int InvestmentAccountId { get; set; } = 0;
        public InvestmentAccountType InvestmentAccountType { get; set; } = InvestmentAccountType.Unknown;
        public string? InvestmentAccountNumber { get; set; } = null;
        public bool IsTaxDeferred { get; set; } = false;
        public bool IsTaxExempt { get; set; } = false;
    }

    /// <summary>
    /// Request model to create an investment account
    /// </summary>
    public class CreateInvestmentAccountRequest : CreateAccountRequest
    {
        // Class variables
        public InvestmentAccountType InvestmentAccountType { get; set; } = InvestmentAccountType.Unknown;
        public Optional<int?> AccountNumber { get; set; } = null;
        public bool? IsTaxDeferred { get; set; } = null;
        public bool? IsTaxExempt { get; set; } = null;
    }
}
