/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a paycheck
    /// </summary>
    public class PaycheckEntityModel
    {
        // Class Level Properties
        public int PaycheckId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public DateOnly StartingDate { get; set; } = DateOnly.MinValue;
        public DateOnly? SecondaryDate { get; set; } = null;
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
        public List<PaycheckSplitEntityModel> PaycheckSplits { get; set; } = new List<PaycheckSplitEntityModel>();
    }

    /// <summary>
    /// Request method to create a paycheck
    /// </summary>
    public class CreatePaycheckRequest
    {
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string PaycheckName { get; set; } = "";
        public DateOnly StartingDate { get; set; } = DateOnly.MinValue;
        public Optional<DateOnly?> SecondaryDate { get; set; }
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
        public List<CreatePaycheckSplitRequest> PaycheckSplits { get; set; } = new List<CreatePaycheckSplitRequest>();
    }

    /// <summary>
    /// Response model for a DTO paycheck
    /// </summary>
    public class CreatePaycheckResponse : BaseIdResponse
    {
        // Class level properties
        public List<int> PaycheckSplitIds { get; set; } = new List<int>();

        /// <summary>
        /// Parameterized constructor for a paycheck DTO Response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreatePaycheckResponse(int httpStatus, string message) : base(httpStatus, message) { }

        /// <summary>
        /// Default constructor for a paycheck DTO Response
        /// </summary>
        public CreatePaycheckResponse() : base() { }
    }


    public class SummaryPaycheckResponse
    {
        // Class properties
        public int PaycheckId { get; set; } = 0;
        public DateOnly StartingDate { get; set; } = DateOnly.MinValue;
        public DateOnly? SecondaryDate { get; set; } = null;
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
    }


    public class GetAllPaychecksResponse : BaseResponse
    {
        // Class properties
        public List<SummaryPaycheckResponse> Paychecks { get; set; } = new List<SummaryPaycheckResponse>();

        /// <summary>
        /// Default constructor for the get all paychecks response
        /// </summary>
        public GetAllPaychecksResponse() : base() { }

        /// <summary>
        /// Parameterized constructor with a status and a message
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public GetAllPaychecksResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }


    public class GetPaycheckResponse : BaseResponse
    {
        // Class properties
        public GetPaycheckDTOModel Paycheck { get; set; } = new GetPaycheckDTOModel();

        public GetPaycheckResponse() { }

        public GetPaycheckResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }


    public class GetPaycheckDTOModel
    {
        // Class properties
        public int PaycheckId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public string PaycheckName { get; set; } = "";
        public DateOnly StartingDate { get; set; } = DateOnly.MinValue;
        public DateOnly? SecondaryDate { get; set; } = null;
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
        public List<GetPaycheckSplitDTOModel> PaycheckSplits { get; set; } = new List<GetPaycheckSplitDTOModel>();
    }


    public class UpdatePaycheckRequest
    {
        // Class properties
        public int PaycheckId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public DateOnly? StartingDate { get; set; }
        public Optional<DateOnly?> SecondaryDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public PaycheckRegularity? PaycheckRegularity { get; set; }
        public List<UpdatePaycheckSplitRequest> PaycheckSplits { get; set; } = new List<UpdatePaycheckSplitRequest>();
    }


    public class UpdatePaycheckResponse : BaseIdResponse
    {
        // Class properties
        public List<int> PaycheckSplitIds { get; set; } = new List<int>();

        public UpdatePaycheckResponse(int httpStatus, string message) : base(httpStatus, message) { }
        public UpdatePaycheckResponse(int httpStatus, string message, int id) : base(httpStatus, message, id) { }

        public UpdatePaycheckResponse() : base() { }
    }
}
