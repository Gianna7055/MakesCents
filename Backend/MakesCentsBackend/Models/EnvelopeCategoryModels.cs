/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an envelope category
    /// </summary>
    public class EnvelopeCategoryEntity
    {
        // Class Level Properties
        public int EnvelopeCategoryId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public string EnvelopeCategoryName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<EnvelopeEntity> Envelopes { get; set; } = new List<EnvelopeEntity>();
    }

    /// <summary>
    /// Response model for an envelope category for the Get Budget method
    /// </summary>
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
    public class CreateEnvelopeCategoryRequest
    {
        // Class level properties
        public int? BudgetId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public string? EnvelopeCategoryName { get; set; } = null;
    }

    /// <summary>
    /// Response model for getting all envelope categories
    /// </summary>
    public class GetAllEnvelopeCategoriesResponse
    {
        // Class properties
        public int HttpStatus { get; set; } = 0;
        public string Message { get; set; } = "";
        public List<SummaryEnvelopeCategoryResponse> AllEnvelopeCategories { get; set; } = new List<SummaryEnvelopeCategoryResponse>();

        /// <summary>
        /// Default constructor for GetEnvelopeCategoryResponse
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="allEnvelopeCategories"></param>
        public GetAllEnvelopeCategoriesResponse()
        {
        }

        /// <summary>
        /// Parameterized constructor for GetEnvelopeCategoryResponse
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="allEnvelopeCategories"></param>
        public GetAllEnvelopeCategoriesResponse(int status, string message)
        {
            HttpStatus = status;
            Message = message;
        }
    }

    /// <summary>
    /// Request model for editing an envelope category
    /// </summary>
    public class EditEnvelopeCategoryRequest
    {
        // Class properties
        public int? EnvelopeCategoryId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public string? EnvelopeCategoryName { get; set; } = null;
    }
}
