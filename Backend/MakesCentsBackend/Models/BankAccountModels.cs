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
    public class BankAccountEntityModel : AccountEntityModel
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
    public class BankAccountSummaryDTOModel : AccountSummaryDTOModel
    {
        // Class properties
        public int BankAccountId { get; set; } = 0;
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
    }

    /// <summary>
    /// DTO model to get a specific bank account
    /// </summary>
    public class GetBankAccountDTOModel : GetAccountBaseModel
    {
        // Class properties
        public int BankAccountId { get; set; } = 0;
        public BankAccountType BankAccountType { get; set; } = BankAccountType.Unknown;
        public List<SummaryTransactionDTOModel> Transactions { get; set; } = new List<SummaryTransactionDTOModel>();

        /// <summary>
        /// Default constructor for the get bank account DTO
        /// </summary>
        public GetBankAccountDTOModel() { }
    }

    /// <summary>
    /// Response model for getting a specific bank account
    /// </summary>
    public class GetBankAccountDTOResponse : BaseResponse
    {
        public GetBankAccountDTOModel BankAccount { get; set; } = new GetBankAccountDTOModel();

        public GetBankAccountDTOResponse(int httpStatus, string message) : base(httpStatus, message) { } 
    }

    /// <summary>
    /// Entity model to get a specific bank account
    /// </summary>
    public class GetBankAccountEntityModel : GetAccountBaseModel
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
        public GetBankAccountEntityModel BankAccount { get; set; } = new GetBankAccountEntityModel();

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


    public class UpdateBankAccountRequest : UpdateAccountRequest
    {
        // Class properties
        public int BankAccountId { get; set; } = 0;
        public BankAccountType? BankAccountType { get; set; } = null;
    }
}
