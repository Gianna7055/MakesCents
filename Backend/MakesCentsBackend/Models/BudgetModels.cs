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
    /// Response model for a single budget in a user's budget list - just enough to let them
    /// pick which one to open, before any month/id is known.
    /// </summary>
    [ExportTsInterface]
    public class SummaryBudgetResponse
    {
        public int BudgetId { get; set; } = 0;
        public string BudgetName { get; set; } = "";
    }

    /// <summary>
    /// Response model for every budget a user has access to
    /// </summary>
    [ExportTsInterface]
    public class GetUserBudgetsResponse : BaseResponse
    {
        // Class properties
        public List<SummaryBudgetResponse> Budgets { get; set; } = new List<SummaryBudgetResponse>();

        public GetUserBudgetsResponse() : base() { }
        public GetUserBudgetsResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }

    /// <summary>
    /// Request model for creating the persistent budget entity (no month yet - see CreateBudgetMonthRequest)
    /// </summary>
    [ExportTsClass]
    public class CreateBudgetRequest
    {
        public int UserId { get; set; } = 0;

        [JsonConverter(typeof(SafeDefaultConverter<string>))]
        public string BudgetName { get; set; } = "";
    }

    /// <summary>
    /// Request model for creating a new month under an existing persistent budget
    /// </summary>
    [ExportTsClass]
    public class CreateBudgetMonthRequest
    {
        public int UserId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        // Required - every budget month must be created with a template applied to it
        public int BudgetTemplateId { get; set; } = 0;

        [JsonConverter(typeof(SafeDefaultConverter<Month>))]
        public Month Month { get; set; } = Month.Unknown;

        [JsonConverter(typeof(SafeDefaultConverter<int>))]
        public int Year { get; set; } = 0;
    }

    /// <summary>
    /// Shared lookup key: a user id plus a year/month anchor point. Used both to fetch a single
    /// budget month (GetBudgetMonthAsync) and as the anchor for a rolling 12-month range
    /// (GetYearBudgetsAsync) - not itself a response, just the query key for both.
    /// </summary>
    [ExportTsClass]
    public class GetBudgetRequest
    {
        public int UserId { get; set; } = 0;
        // Required - a user can now belong to more than one budget (budget_users), so year/month
        // alone is ambiguous without saying which budget
        public int BudgetId { get; set; } = 0;

        [JsonConverter(typeof(SafeDefaultConverter<Month>))]
        public Month Month { get; set; } = Month.Unknown;

        [JsonConverter(typeof(SafeDefaultConverter<int>))]
        public int Year { get; set; } = 0;

        /// <summary>
        /// Parameterized constructor for a Get Budget DTO
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="budgetId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        public GetBudgetRequest(int userId, int budgetId, Month month, int year)
        {
            UserId = userId;
            BudgetId = budgetId;
            Month = month;
            Year = year;
        }
    }

    /// <summary>
    /// DTO model for sending a budget month to the API
    /// </summary>
    [ExportTsInterface]
    public class GetBudgetMonthDTOModel
    {
        // Class Level Properties
        public int BudgetId { get; set; } = 0;
        // The specific month's budget_month_id - transactions and rollover envelope amounts
        // are keyed off this, not BudgetId, since a persistent budget spans many months.
        public int BudgetMonthId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public Month Month { get; set; } = Month.Unknown;
        public int Year { get; set; } = 0;
        public string BudgetName { get; set; } = "";
        // Which template was applied to this month's funding queue
        public int? BudgetTemplateId { get; set; } = null;
        public List<SummaryEnvelopeCategoryResponse> EnvelopeCategories { get; set; } = new List<SummaryEnvelopeCategoryResponse>();

        /// <summary>
        /// Default constructor for a GetBudgetMonthDTO
        /// </summary>
        public GetBudgetMonthDTOModel() { }
    }

    /// <summary>
    /// Response model for getting a budget month
    /// </summary>
    [ExportTsInterface]
    public class GetBudgetMonthResponse : BaseResponse
    {
        // Class Level Properties
        [JsonPropertyName("budget")]
        [TsMemberName("budget")]
        public GetBudgetMonthDTOModel GetBudgetMonthDTO { get; set; } = new GetBudgetMonthDTOModel();

        public GetBudgetMonthResponse(int status, string message) : base(status, message) { }
        public GetBudgetMonthResponse() : base() { }
    }


    [ExportTsInterface]
    public class GetYearBudgetsResponse : BaseIdResponse
    {
        // Properties
        public List<GetYearBudgetDTOModel> Budgets { get; set; } = new List<GetYearBudgetDTOModel>();

        public GetYearBudgetsResponse(int status, string message) : base(status, message) { }
        public GetYearBudgetsResponse() : base() { }
    }

    [ExportTsClass]
    public class GetYearBudgetDTOModel
    {
        public int BudgetId { get; set; } = 0;
        // The specific month's budget_month_id - needed so selecting a past month from this list
        // can actually identify which month to load (BudgetId alone can't anymore).
        public int BudgetMonthId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public Month Month { get; set; } = Month.Unknown;
        public int Year { get; set; } = 0;
        public string BudgetName { get; set; } = "";
        // Which template (if any) was applied to this month's funding queue - null if none
        public int? BudgetTemplateId { get; set; } = null;
    }

    /// <summary>
    /// Request model for updating a budget's persistent fields (currently just the name)
    /// </summary>
    [ExportTsClass]
    public class EditBudgetRequest
    {
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string? BudgetName { get; set; } = null;
    }

    /// <summary>
    /// Request model for updating a specific budget month's month/year
    /// (e.g. correcting a month created under the wrong month/year)
    /// </summary>
    [ExportTsClass]
    public class EditBudgetMonthRequest
    {
        public int BudgetMonthId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public Month? Month { get; set; } = null;
        public int? Year { get; set; } = null;
    }
}