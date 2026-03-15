/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */

using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an envelope category
    /// </summary>
    public class EnvelopeCategoryEntityModel
    {
        // Class Level Properties
        public int EnvelopeCategoryId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public string EnvelopeCategoryName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
        public List<EnvelopeEntityModel> Envelopes { get; set; } = new List<EnvelopeEntityModel>();
    }

    /// <summary>
    /// Response model for an envelope category for the Get Budget method
    /// </summary>
    [ExportTsInterface]
    public class SummaryEnvelopeCategoryResponse
    {
        // Class Level Properties
        public int EnvelopeCategoryId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public string EnvelopeCategoryName { get; set; } = "";
        public List<SummaryEnvelopeResponse> Envelopes { get; set; } = new List<SummaryEnvelopeResponse>();
    }

    /// <summary>
    /// Request model for creating an envelope category
    /// </summary>
    [ExportTsClass]
    public class CreateEnvelopeCategoryRequest
    {
        // Class level properties
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string EnvelopeCategoryName { get; set; } = "";
    }

    /// <summary>
    /// Response model for getting all envelope categories
    /// </summary>
    [ExportTsInterface]
    public class GetAllEnvelopeCategoriesResponse : BaseResponse
    {
        // Class properties
        public List<SummaryEnvelopeCategoryResponse> EnvelopeCategories { get; set; } = new List<SummaryEnvelopeCategoryResponse>();

        /// <summary>
        /// Default constructor for GetEnvelopeCategoryResponse
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="allEnvelopeCategories"></param>
        public GetAllEnvelopeCategoriesResponse() : base() { }

        /// <summary>
        /// Parameterized constructor for GetEnvelopeCategoryResponse
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="allEnvelopeCategories"></param>
        public GetAllEnvelopeCategoriesResponse(int status, string message) : base(status, message) { }
    }

    /// <summary>
    /// Request model for editing an envelope category
    /// </summary>
    [ExportTsClass]
    public class EditEnvelopeCategoryRequest
    {
        // Class properties
        public int EnvelopeCategoryId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string? EnvelopeCategoryName { get; set; } = null;
    }
}
