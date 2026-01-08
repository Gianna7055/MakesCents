/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/5/2025
 * Makes Cents - Budget Model
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a budget
    /// </summary>
    public class BudgetModel
    {
        // Class Level Properties
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public Month Month { get; set; } = Month.Unknown;
        public int Year { get; set; } = 0;
        public string BudgetName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<EnvelopeCategoryModel> EnvelopeCategories { get; set; } = new List<EnvelopeCategoryModel>();
        public List<AccountModel> Accounts { get; set; } = new List<AccountModel>();
        public List<PaycheckModel> Paychecks { get; set; } = new List<PaycheckModel>();
        public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();
        public List<PlannedExpenseModel> PlannedExpenses { get; set; } = new List<PlannedExpenseModel>();
    }
}
