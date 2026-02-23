/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Entities;
using MakesCentsBackend.Models.Enums;
using System.Text.Json.Serialization;

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
    /// Request model for creating a budget
    /// </summary>
    public class CreateBudgetRequest
    {
        public int? UserId { get; set; } = null;
        public Month Month { get; set; } = Month.Unknown;
        public int? Year { get; set; } = null;
        public string? BudgetName { get; set; } = null;
    }

    /// <summary>
    /// DTO model for getting a budget
    /// </summary>
    public class GetBudgetRequest
    {
        public int? UserId { get; set; } = null;
        public Month Month { get; set; } = Month.Unknown;
        public int? Year { get; set; } = null;

        /// <summary>
        /// Parameterized constructor for a Get Budget DTO
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        public GetBudgetRequest(int? userId, Month month, int? year)
        {
            UserId = userId;
            Month = month;
            Year = year;
        }
    }

    /// <summary>
    /// DTO model for sending a budget to the API for getting a budget
    /// </summary>
    public class GetBudgetDTO
    {
        // Class Level Properties
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;

        [JsonPropertyName("monthId")]
        public Month Month { get; set; } = Month.Unknown;
        public int Year { get; set; } = 0;
        public string BudgetName { get; set; } = "";
        public List<SummaryEnvelopeCategoryResponse> EnvelopeCategories { get; set; } = new List<SummaryEnvelopeCategoryResponse>();

        /// <summary>
        /// Default constructor for a GetBudgetDTO
        /// </summary>
        public GetBudgetDTO() { }

        /// <summary>
        /// Parameterized constructor for a Get Budget DTO
        /// </summary>
        /// <param name="budgetId"></param>
        public GetBudgetDTO(int budgetId, int userId, Month month, int year, string budgetName) : this(budgetId, userId, month, year)
        {
            BudgetName = budgetName;
        }


        /// <summary>
        /// Parameterized constructor for a Get Budget DTO
        /// </summary>
        /// <param name="budgetId"></param>
        public GetBudgetDTO(int budgetId, int userId, Month month, int year)
        {
            BudgetId = budgetId;
            UserId = userId;
            Month = month;
            Year = year;
        }
    }

    /// <summary>
    /// Response model for getting a budget
    /// </summary>
    public class GetBudgetResponse : BaseResponse
    {
        // Class Level Properties
        public GetBudgetDTO GetBudgetDTO { get; set; } = new GetBudgetDTO();

        public GetBudgetResponse(int status, string message) : base(status, message) { }
    }

    /// <summary>
    /// Request model for updating a budget
    /// </summary>
    public class EditBudgetRequest
    {
        public int? BudgetId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public string? BudgetName { get; set; } = null;
    }
}
