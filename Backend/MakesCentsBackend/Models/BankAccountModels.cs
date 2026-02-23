/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a bank account
    /// </summary>
    public class BankAccountEntity : AccountEntity
    {
        // Class Level Properties
        public int BankAccountId { get; set; } = 0;
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
    }

    /// <summary>
    /// Request model for creating a bank account model
    /// </summary>
    public class CreateBankAccountRequest : CreateAccountRequest
    {
        // Class properties
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
    }

    /// <summary>
    /// Response model for creating a bank account
    /// </summary>
    public class CreateBankAccountResponse : CreateAccountResponse
    {
        // Class properties
        public int? BankAccountId { get; set; } = null;

        /// <summary>
        /// Parameterized constructor for a bank account response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreateBankAccountResponse(int httpStatus, string message) : base(httpStatus, message) { }

        /// <summary>
        /// Default constructor for a bank account response
        /// </summary>
        public CreateBankAccountResponse() : base() { }
    }

    /// <summary>
    /// DTO model for getting all accounts
    /// </summary>
    public class BankAccountSummaryDTO : AccountSummaryDTO
    {
        // Class properties
        public int BankAccountId { get; set; } = 0;
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
    }

    /// <summary>
    /// DTO model to get a specific bank account
    /// </summary>
    public class GetBankAccountDTO : GetAccountModel
    {
        // Class properties
        public int BankAccountId { get; set; } = 0;
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
        public List<SummaryTransactionDTOModel> Transactions { get; set; } = new List<SummaryTransactionDTOModel>();

        /// <summary>
        /// Default constructor for the get bank account DTO
        /// </summary>
        public GetBankAccountDTO() { }
    }

    /// <summary>
    /// Response model for getting a specific bank account
    /// </summary>
    public class GetBankAccountDTOResponse : BaseResponse
    {
        public GetBankAccountDTO BankAccount { get; set; } = new GetBankAccountDTO();
    }

    /// <summary>
    /// Entity model to get a specific bank account
    /// </summary>
    public class GetBankAccountEntity : GetAccountModel
    {
        // Class properties
        public int BankAccountId { get; set; } = 0;
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
        public List<SummaryTransactionEntityModel> Transactions { get; set; } = new List<SummaryTransactionEntityModel>();
    }

    /// <summary>
    /// Response model for getting a specific bank account
    /// </summary>
    public class GetBankAccountEntityResponse : BaseResponse
    {
        public GetBankAccountEntity BankAccount { get; set; } = new GetBankAccountEntity();

        /// <summary>
        /// Default constructor
        /// </summary>
        public GetBankAccountEntityResponse() { }

        /// <summary>
        /// Parameterized constructor that takes a status and a message
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public GetBankAccountEntityResponse(int status, string message) : base(status, message) { }
    }
}
