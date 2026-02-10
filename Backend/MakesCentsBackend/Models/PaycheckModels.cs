/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models.Entities;
using MakesCentsBackend.Models.Enums;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a paycheck
    /// </summary>
    public class PaycheckEntity
    {
        // Class Level Properties
        public int PaycheckId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public DateOnly StartingDate { get; set; } = new DateOnly();
        public DateOnly? SecondaryDate { get; set; } = null;
        public decimal TotalAmount { get; set; } = 0m;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<PaycheckSplitEntity> PaycheckSplits { get; set; } = new List<PaycheckSplitEntity>();
    }

    /// <summary>
    /// Request method to create a paycheck
    /// </summary>
    public class CreatePaycheckRequest
    {
        public int? BudgetId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public string? PaycheckName { get; set; } = null;
        public DateOnly? StartingDate { get; set; } = null;
        public Optional<DateOnly?> SecondaryDate { get; set; } = null;
        public decimal? TotalAmount { get; set; } = null;
        public PaycheckRegularity PaycheckRegularity { get; set; } = PaycheckRegularity.Unknown;
        public List<CreatePaycheckSplitRequest> PaycheckSplits { get; set; } = new List<CreatePaycheckSplitRequest>();
    }

    /// <summary>
    /// Response model for a DTO paycheck
    /// </summary>
    public class CreatePaycheckResponse
    {
        // Class level properties
        public int HttpStatus { get; set; } = 0;
        public string? Message { get; set; } = null;
        public int? PaycheckId { get; set; } = null;
        public List<int> PaycheckSplitIds { get; set; } = new List<int>();

        /// <summary>
        /// Parameterized constructor for a paycheck DTO Response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public CreatePaycheckResponse(int httpStatus, string? message)
        {
            HttpStatus = httpStatus;
            Message = message;
        }

        /// <summary>
        /// Default constructor for a paycheck DTO Response
        /// </summary>
        public CreatePaycheckResponse()
        {
        }
    }
}
