/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an investment account
    /// </summary>
    public class InvestmentAccountEntityModel : AccountEntityModel
    {
        // Class Level Properties
        public int InvestmentAccountId { get; set; } = 0;
        public InvestmentAccountType InvestmentAccountType { get; set; } = InvestmentAccountType.Unknown;
        public string? InvestmentAccountNumber { get; set; } = null;
        public bool IsTaxDeferred { get; set; } = false;
        public bool IsTaxExempt { get; set; } = false;
    }

    /// <summary>
    /// Request model to create an investment account
    /// </summary>
    [ExportTsClass]
    public class CreateInvestmentAccountRequest : CreateAccountRequest
    {
        // Class variables
        public InvestmentAccountType InvestmentAccountType { get; set; } = InvestmentAccountType.Unknown;
        public Optional<int?> InvestmentAccountNumber { get; set; }
        public bool IsTaxDeferred { get; set; } = false;
        public bool IsTaxExempt { get; set; } = false;
    }

    /// <summary>
    /// Response model for creating an investment account
    /// </summary>
    [ExportTsInterface]
    public class CreateInvestmentAccountResponse : CreateAccountResponse
    {
        // Class properties
        public int? InvestmentAccountId { get; set; } = null;

        /// <summary>
        /// Parameterized constructor for an investment account response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreateInvestmentAccountResponse(int httpStatus, string message) : base(httpStatus, message) { }

        /// <summary>
        /// Default constructor for an investment account response
        /// </summary>
        public CreateInvestmentAccountResponse() : base() { }
    }


    [ExportTsInterface]
    public class InvestmentAccountSummaryDTOModel : SummaryAccountDTOModel
    {
        // Class properties
        public int InvestmentAccountId { get; set; } = 0;
    }

    /// <summary>
    /// DTO model to get a specific investment account
    /// </summary>
    [ExportTsInterface]
    public class GetInvestmentAccountDTOModel : GetAccountBaseModel
    {
        // Class properties
        public int InvestmentAccountId { get; set; } = 0;
        public InvestmentAccountType InvestmentAccountType { get; set; } = InvestmentAccountType.Unknown;
        public int? AccountNumber { get; set; } = null;
        public bool IsTaxDeferred { get; set; } = false;
        public bool IsTaxExempt { get; set; } = false;
        public List<SummaryTransactionDTOModel> Transactions { get; set; } = new List<SummaryTransactionDTOModel>();

        /// <summary>
        /// Default constructor for the get investment account DTO
        /// </summary>
        public GetInvestmentAccountDTOModel() { }
    }

    /// <summary>
    /// Response model for getting a specific investment account
    /// </summary>
    [ExportTsInterface]
    public class GetInvestmentAccountDTOResponse : BaseResponse
    {
        public GetInvestmentAccountDTOModel InvestmentAccount { get; set; } = new GetInvestmentAccountDTOModel();

        public GetInvestmentAccountDTOResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }

    /// <summary>
    /// Entity model to get a specific investment account
    /// </summary>
    public class GetInvestmentAccountEntityModel : GetAccountBaseModel
    {
        // Class properties
        public int InvestmentAccountId { get; set; } = 0;
        public InvestmentAccountType InvestmentAccountType { get; set; } = InvestmentAccountType.Unknown;
        public int? AccountNumber { get; set; } = null;
        public bool IsTaxDeferred { get; set; } = false;
        public bool IsTaxExempt { get; set; } = false;
        public List<SummaryTransactionEntityModel> Transactions { get; set; } = new List<SummaryTransactionEntityModel>();
    }

    /// <summary>
    /// Response model for getting a specific investment account
    /// </summary>
    public class GetInvestmentAccountEntityResponse : BaseResponse
    {
        public GetInvestmentAccountEntityModel InvestmentAccount { get; set; } = new GetInvestmentAccountEntityModel();

        /// <summary>
        /// Default constructor
        /// </summary>
        public GetInvestmentAccountEntityResponse() { }

        /// <summary>
        /// Parameterized constructor that takes a status and a message
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public GetInvestmentAccountEntityResponse(int status, string message) : base(status, message) { }
    }


    [ExportTsClass]
    public class UpdateInvestmentAccountRequest : UpdateAccountRequest
    {
        // Class properties
        public int InvestmentAccountId { get; set; } = 0;
        public InvestmentAccountType? InvestmentAccountType { get; set; } = null;
        public Optional<int?> AccountNumber { get; set; }
        public bool? IsTaxDeferred { get; set; } = null;
        public bool? IsTextExempt { get; set; } = null;
    }
}