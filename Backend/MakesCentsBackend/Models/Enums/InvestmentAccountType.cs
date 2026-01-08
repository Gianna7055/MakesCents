/*
 * Gianna Ross
 * File Created: 12/5/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - Investment Account Type Enum
 * Sources: 
 */
namespace MakesCentsBackend.Models.Enums
{
    /// <summary>
    /// Enum for Investment Account Types for Investment Account Model
    /// </summary>
    public enum InvestmentAccountType
    {
        Unknown = 1,
        IRA = 2,
        Retirement401K403B = 3,
        Brokerage = 4,
        Other = 5,
    }
}
