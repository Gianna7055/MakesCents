/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a debt account
    /// </summary>
    public class DebtAccountEntity : AccountEntity
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
    public class CreateDebtAccountRequest : CreateAccountRequest
    {
        // Class properties
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
        public Optional<int?> AccountNumber { get; set; } = null;
        public Optional<DateOnly?> DateOfNextBill { get; set; } = null;
        public Optional<decimal?> AmountOfNextBill { get; set; } = null;
        public Optional<DebtPaymentRegularity?> DebtPaymentRegularity { get; set; } = null;
    }

    /// <summary>
    /// Response model for creating a debt account
    /// </summary>
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


    public class DebtAccountSummaryDTO : AccountSummaryDTO
    {
        // Class properties
        public int DebtAccountId { get; set; } = 0;
        public DebtAccountType DebtAccountType { get; set; } = DebtAccountType.Unknown;
    }

    /// <summary>
    /// DTO model to get a specific debt account
    /// </summary>
    public class GetDebtAccountDTO : GetAccountModel
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
        public GetDebtAccountDTO() { }
    }

    /// <summary>
    /// Response model for getting a specific debt account
    /// </summary>
    public class GetDebtAccountDTOResponse : BaseResponse
    {
        public GetDebtAccountDTO DebtAccount { get; set; } = new GetDebtAccountDTO();
    }

    /// <summary>
    /// Entity model to get a specific debt account
    /// </summary>
    public class GetDebtAccountEntity : GetAccountModel
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
        public GetDebtAccountEntity DebtAccount { get; set; } = new GetDebtAccountEntity();

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
}
