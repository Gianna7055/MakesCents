/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Investment Account Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an investment account
    /// </summary>
    public class InvestmentAccountModel : AccountModel
    {
        // Class Level Properties
        public int InvestmentAccountId { get; set; } = 0;
        public InvestmentAccountType InvestmentAccountType { get; set; } = InvestmentAccountType.Unknown;
        public string? InvestmentAccountNumber { get; set; } = null;
        public bool IsTaxDeferred { get; set; } = false;
        public bool IsTaxExempt { get; set; } = false;
    }
}
