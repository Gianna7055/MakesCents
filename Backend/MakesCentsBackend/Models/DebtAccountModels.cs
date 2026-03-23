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
    /// Model for a debt account
    /// </summary>
    public class DebtAccountEntityModel : AccountEntityModel
    {
        // Class Level Properties
        public int DebtAccountId { get; set; } = 0;
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
        public string? DebtAccountNumber { get; set; } = null;
        public DateOnly? DateOfNextBill { get; set; } = null;
        public decimal? AmountOfNextBill { get; set; } = null;
        public DebtPaymentRegularity? DebtPaymentRegularity { get; set; } = null;
    }

    /// <summary>
    /// Request model for creating a debt account model
    /// </summary>
    [ExportTsClass]
    public class CreateDebtAccountRequest : CreateAccountRequest
    {
        // Class properties
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
        public Optional<int?> AccountNumber { get; set; }
        public Optional<DateOnly?> DateOfNextBill { get; set; }
        public Optional<decimal?> AmountOfNextBill { get; set; }
        public Optional<DebtPaymentRegularity?> DebtPaymentRegularity { get; set; }
    }

    /// <summary>
    /// Response model for creating a debt account
    /// </summary>
    [ExportTsInterface]
    public class CreateDebtAccountResponse : CreateAccountResponse
    {
        // Class properties
        public int? DebtAccountId { get; set; } = null;

        /// <summary>
        /// Parameterized constructor for a debt account response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreateDebtAccountResponse(int httpStatus, string message) : base(httpStatus, message) { }

        /// <summary>
        /// Default constructor for a debt account response
        /// </summary>
        public CreateDebtAccountResponse() : base() { }
    }


    [ExportTsInterface]
    public class DebtAccountSummaryDTOModel : SummaryAccountDTOModel
    {
        // Class properties
        public int DebtAccountId { get; set; } = 0;
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
    }

    /// <summary>
    /// DTO model to get a specific debt account
    /// </summary>
    [ExportTsInterface]
    public class GetDebtAccountDTOModel : GetAccountBaseModel
    {
        // Class properties
        public int DebtAccountId { get; set; } = 0;
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
        public int? AccountNumber { get; set; } = null;
        public DateOnly? DateOfNextBill { get; set; } = null;
        public decimal? AmountOfNextBill { get; set; } = null;
        public DebtPaymentRegularity DebtPaymentRegularity { get; set; } = DebtPaymentRegularity.Unknown;
        public List<SummaryTransactionDTOModel> Transactions { get; set; } = new List<SummaryTransactionDTOModel>();

        /// <summary>
        /// Default constructor for the get debt account DTO
        /// </summary>
        public GetDebtAccountDTOModel() { }
    }

    /// <summary>
    /// Response model for getting a specific debt account
    /// </summary>
    [ExportTsInterface]
    public class GetDebtAccountDTOResponse : BaseResponse
    {
        public GetDebtAccountDTOModel DebtAccount { get; set; } = new GetDebtAccountDTOModel();
        public GetDebtAccountDTOResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }

    /// <summary>
    /// Entity model to get a specific debt account
    /// </summary>
    public class GetDebtAccountEntityModel : GetAccountBaseModel
    {
        // Class properties
        public int DebtAccountId { get; set; } = 0;
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
        public int? AccountNumber { get; set; } = null;
        public DateOnly? DateOfNextBill { get; set; } = null;
        public decimal? AmountOfNextBill { get; set; } = null;
        public DebtPaymentRegularity DebtPaymentRegularity { get; set; } = DebtPaymentRegularity.Unknown;
        public List<SummaryTransactionEntityModel> Transactions { get; set; } = new List<SummaryTransactionEntityModel>();
    }

    /// <summary>
    /// Response model for getting a specific debt account
    /// </summary>
    public class GetDebtAccountEntityResponse : BaseResponse
    {
        public GetDebtAccountEntityModel DebtAccount { get; set; } = new GetDebtAccountEntityModel();

        /// <summary>
        /// Default constructor
        /// </summary>
        public GetDebtAccountEntityResponse() { }

        /// <summary>
        /// Parameterized constructor that takes a status and a message
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public GetDebtAccountEntityResponse(int status, string message) : base(status, message) { }
    }


    [ExportTsClass]
    public class UpdateDebtAccountRequest : UpdateAccountRequest
    {
        // Class properties
        public int DebtAccountId { get; set; } = 0;
        public DebtAccountType? DebtAccountType { get; set; } = null;
        public Optional<int?> AccountNumber { get; set; } = null;
        public Optional<DateOnly?> DateOfNextBill { get; set; }
        public Optional<DateOnly?> AmountOfNextBill { get; set; }
        public Optional<DebtPaymentRegularity?> DebtPaymentRegularity { get; set; }
    }
}
