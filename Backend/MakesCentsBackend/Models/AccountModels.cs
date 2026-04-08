/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Converters;
using MakesCentsBackend.Models.Enums;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;


namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an abstract account
    /// </summary>
    public abstract class AccountEntityModel
    {
        // Class Level Properties
        public int AccountId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public AccountType AccountType { get; set; } = AccountType.Unknown;
        public string AccountName { get; set; } = "";
        public string Institution { get; set; } = "";
        public decimal Balance { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
        public List<TransactionEntityModel> Transactions { get; set; } = new List<TransactionEntityModel>();
    }

    [ExportTsClass]
    public abstract class CreateAccountRequest
    {
        // Class properties
        [JsonConverter(typeof(SafeDefaultConverter<int>))]
        public int BudgetId { get; set; } = 0;

        public int UserId { get; set; } = 0;

        public int AccountId { get; set; } = 0;

        [JsonConverter(typeof(SafeDefaultConverter<string>))]
        public string AccountName { get; set; } = "";

        [JsonConverter(typeof(SafeDefaultConverter<string>))]
        public string Institution { get; set; } = "";

        [JsonConverter(typeof(SafeDefaultConverter<decimal>))]
        public decimal Balance { get; set; } = 0m;
    }

    /// <summary>
    /// Response model for an account
    /// </summary>
    [ExportTsInterface]
    public class CreateAccountResponse : BaseResponse
    {
        // Class level properties
        public int? AccountId { get; set; } = null;

        /// <summary>
        /// Parameterized constructor for an account response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreateAccountResponse(int httpStatus, string message) : base(httpStatus, message) { }

        /// <summary>
        /// Default constructor for an account response
        /// </summary>
        public CreateAccountResponse() : base() { }
    }


    [ExportTsInterface]
    public class SummaryAccountDTOModel
    {
        // Class properties
        public int AccountId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public AccountType AccountType { get; set; } = AccountType.Unknown;
        public string AccountName { get; set; } = "";
        public decimal Balance { get; set; } = 0m;
        public string Institution { get; set; } = "";
        public BankAccountType? BankAccountType { get; set; } = null;
        public DebtAccountType? DebtAccountType { get; set; } = null;
        public InvestmentAccountType? InvestmentAccountType { get; set; } = null;
    }

    /// <summary>
    /// Response model for getting all accounts
    /// </summary>
    [ExportTsInterface]
    public class GetAllAccountsResponse : BaseResponse
    {
        // Class properties
        public List<SummaryAccountDTOModel> Accounts { get; set; } = new List<SummaryAccountDTOModel>();

        /// <summary>
        /// Initializes a new instance of the GetAllAccountsResponse class with the specified HTTP status code and
        /// message
        /// </summary>
        /// <param name="httpStatus">The HTTP status code that represents the result of the request</param>
        /// <param name="message">A message that provides additional information about the response</param>
        public GetAllAccountsResponse(int httpStatus, string message) : base(httpStatus, message) { }

        /// <summary>
        /// Initializes a new instance of the GetAllAccountsResponse class
        /// </summary>
        public GetAllAccountsResponse() : base() { }
    }

    /// <summary>
    /// Base class for getting a specific account
    /// </summary>
    [ExportTsInterface]
    public class GetAccountBaseModel
    {
        // Class properties
        public int AccountId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public AccountType AccountType { get; set; } = AccountType.Unknown;
        public string AccountName { get; set; } = "";
        public string Institution { get; set; } = "";
        public decimal Balance { get; set; } = 0;
    }


    [ExportTsClass]
    public class UpdateAccountRequest
    {
        // Class properties
        public int UserId { get; set; } = 0;

        [JsonConverter(typeof(SafeDefaultConverter<int>))]
        public int AccountId { get; set; } = 0;
        public string? AccountName { get; set; } = null;
        public string? Institution { get; set; } = null;
        public decimal? Balance { get; set; } = null;
    }
}
