/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an abstract account
    /// </summary>
    public abstract class AccountEntity
    {
        // Class Level Properties
        public int AccountId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public AccountType AccountType { get; set; } = AccountType.Unknown;
        public string AccountName { get; set; } = "";
        public string Institution { get; set; } = "";
        public decimal Balance { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
    }


    public abstract class CreateAccountRequest
    {
        // Class properties
        public int? BudgetId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public int? AccountId { get; set; } = null;
        public string? AccountName { get; set; } = null;
        public string? Institution { get; set; } = null;
        public decimal? Balance { get; set; } = null;
    }

    /// <summary>
    /// Response model for an account
    /// </summary>
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


    public class AccountSummaryDTO
    {
        // Class properties
        public int AccountId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public AccountType AccountType { get; set; } = AccountType.Unknown;
        public string AccountName { get; set; } = "";
        public decimal balance { get; set; } = 0m;
    }

    /// <summary>
    /// Response model for getting all accounts
    /// </summary>
    public class GetAllAccountsResponse : BaseResponse
    {
        // Class properties
        public List<AccountSummaryDTO> Accounts { get; set; } = new List<AccountSummaryDTO>();

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
    public class GetAccountModel
    {
        // Class properties
        public int AccountId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public AccountType AccountType { get; set; } = AccountType.Unknown;
        public string AccountName { get; set; } = "";
        public decimal Balance { get; set; } = 0;
    }
}
