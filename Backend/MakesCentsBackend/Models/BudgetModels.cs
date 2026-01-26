/*
 * Gianna Ross
 * File Created: 1/23/2026
 * File Last Updated: 1/23/2026
 * Makes Cents - Budget Models
 * Sources: 
 */
using MakesCentsBackend.Models.Entities;
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Entity model for a budget
    /// </summary>
    public class BudgetEntity
    {
        // Class Level Properties
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public Month Month { get; set; } = Month.Unknown;
        public int Year { get; set; } = 0;
        public string BudgetName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<EnvelopeCategoryEntity> EnvelopeCategories { get; set; } = new List<EnvelopeCategoryEntity>();
        public List<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
        public List<PaycheckEntity> Paychecks { get; set; } = new List<PaycheckEntity>();
        public List<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
        public List<PlannedExpenseEntity> PlannedExpenses { get; set; } = new List<PlannedExpenseEntity>();
    }

    /// <summary>
    /// DTO model for a budget
    /// </summary>
    public class BudgetDTO
    {
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public Month Month { get; set; } = Month.Unknown;
        public int Year { get; set; } = 0;
        public string BudgetName { get; set; } = "";
    }
}
