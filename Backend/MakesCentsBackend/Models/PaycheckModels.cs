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
    /// Model for a paycheck. Forecast-only - a paycheck no longer funds envelopes directly
    /// (see envelope_funding for that) and isn't tied to any specific month. It exists purely
    /// to represent expected income on a recurring schedule.
    /// </summary>
    public class PaycheckEntityModel
    {
        // Class Level Properties
        public int PaycheckId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public string PaycheckName { get; set; } = "";
        public DateOnly StartingDate { get; set; } = DateOnly.MinValue;
        public DateOnly? SecondaryDate { get; set; } = null;
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Request model to create a paycheck
    /// </summary>
    [ExportTsClass]
    public class CreatePaycheckRequest
    {
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string PaycheckName { get; set; } = "";
        public DateOnly StartingDate { get; set; } = DateOnly.MinValue;
        public Optional<DateOnly?> SecondaryDate { get; set; }
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
    }


    [ExportTsInterface]
    public class SummaryPaycheckResponse
    {
        // Class properties
        public int PaycheckId { get; set; } = 0;
        public DateOnly StartingDate { get; set; } = DateOnly.MinValue;
        public DateOnly? SecondaryDate { get; set; } = null;
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
    }


    [ExportTsInterface]
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


    [ExportTsInterface]
    public class GetPaycheckResponse : BaseResponse
    {
        // Class properties
        public GetPaycheckDTOModel Paycheck { get; set; } = new GetPaycheckDTOModel();

        public GetPaycheckResponse() { }

        public GetPaycheckResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }


    [ExportTsInterface]
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
    }


    [ExportTsClass]
    public class UpdatePaycheckRequest
    {
        // Class properties
        public int PaycheckId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public DateOnly? StartingDate { get; set; }
        public Optional<DateOnly?> SecondaryDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public PaycheckRegularity? PaycheckRegularity { get; set; }
    }
}