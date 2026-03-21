/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Converters;
using MakesCentsBackend.Models.Entities;
using MakesCentsBackend.Models.Enums;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Entity model for a budget
    /// </summary>
    public class BudgetEntityModel
    {
        // Class Level Properties
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public Month Month { get; set; } = Month.Unknown;
        public int Year { get; set; } = 0;
        public string BudgetName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
        public List<EnvelopeCategoryEntityModel> EnvelopeCategories { get; set; } = new List<EnvelopeCategoryEntityModel>();
        public List<AccountEntityModel> Accounts { get; set; } = new List<AccountEntityModel>();
        public List<PaycheckEntityModel> Paychecks { get; set; } = new List<PaycheckEntityModel>();
        public List<TransactionEntityModel> Transactions { get; set; } = new List<TransactionEntityModel>();
        public List<PlannedExpenseEntity> PlannedExpenses { get; set; } = new List<PlannedExpenseEntity>();
    }

    /// <summary>
    /// Request model for creating a budget
    /// </summary>
    [ExportTsClass]
    public class CreateBudgetRequest
    {
        public int UserId { get; set; } = 0;

        [JsonConverter(typeof(SafeDefaultConverter<Month>))]
        public Month Month { get; set; } = Month.Unknown;

        [JsonConverter(typeof(SafeDefaultConverter<int>))]
        public int Year { get; set; } = 0;

        [JsonConverter(typeof(SafeDefaultConverter<string>))]
        public string BudgetName { get; set; } = "";
    }

    /// <summary>
    /// DTO model for getting a budget
    /// </summary>
    [ExportTsClass]
    public class GetBudgetRequest
    {
        public int UserId { get; set; } = 0;

        [JsonConverter(typeof(SafeDefaultConverter<Month>))]
        public Month Month { get; set; } = Month.Unknown;

        [JsonConverter(typeof(SafeDefaultConverter<int>))]
        public int Year { get; set; } = 0;

        /// <summary>
        /// Parameterized constructor for a Get Budget DTO
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        public GetBudgetRequest(int userId, Month month, int year)
        {
            UserId = userId;
            Month = month;
            Year = year;
        }
    }

    /// <summary>
    /// DTO model for sending a budget to the API for getting a budget
    /// </summary>
    [ExportTsInterface]
    public class GetBudgetDTOModel
    {
        // Class Level Properties
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public Month Month { get; set; } = Month.Unknown;
        public int Year { get; set; } = 0;
        public string BudgetName { get; set; } = "";
        public List<SummaryEnvelopeCategoryResponse> EnvelopeCategories { get; set; } = new List<SummaryEnvelopeCategoryResponse>();

        /// <summary>
        /// Default constructor for a GetBudgetDTO
        /// </summary>
        public GetBudgetDTOModel() { }

        /// <summary>
        /// Parameterized constructor for a Get Budget DTO
        /// </summary>
        /// <param name="budgetId"></param>
        public GetBudgetDTOModel(int budgetId, int userId, Month month, int year, string budgetName) : this(budgetId, userId, month, year)
        {
            BudgetName = budgetName;
        }


        /// <summary>
        /// Parameterized constructor for a Get Budget DTO
        /// </summary>
        /// <param name="budgetId"></param>
        public GetBudgetDTOModel(int budgetId, int userId, Month month, int year)
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
    [ExportTsInterface]
    public class GetBudgetResponse : BaseResponse
    {
        // Class Level Properties
        [JsonPropertyName("budget")]
        [TsMemberName("budget")]
        public GetBudgetDTOModel GetBudgetDTO { get; set; } = new GetBudgetDTOModel();

        public GetBudgetResponse(int status, string message) : base(status, message) { }
        public GetBudgetResponse() : base() { }
    }

    /// <summary>
    /// Request model for updating a budget
    /// </summary>
    [ExportTsClass]
    public class EditBudgetRequest
    {
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string? BudgetName { get; set; } = null;
    }
}
