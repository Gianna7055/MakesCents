/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/5/2025
 * Makes Cents - Envelope Category Model
 * Sources: 
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for an envelope category
    /// </summary>
    public class EnvelopeCategoryModel
    {
        // Class Level Properties
        public int EnvelopeCategoryId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public string EnvelopeCategoryName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public List<EnvelopeModel> Envelopes { get; set; } = new List<EnvelopeModel>();
    }
}
