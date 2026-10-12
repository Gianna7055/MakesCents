/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a budget template - a named, reusable preset of envelope funding amounts
    /// (e.g. "Summer", "Holiday") that can be applied to a budget month to bulk-create its
    /// funding queue. Templates aren't tied to any specific month - their envelope entries
    /// reference persistent envelopes, not a month's instance of one.
    /// </summary>
    public class BudgetTemplateEntityModel
    {
        // Class Level Properties
        public int BudgetTemplateId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public string TemplateName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Request model to create a budget template. No envelopes here yet - use
    /// SetBudgetTemplateEnvelopesRequest to add/edit its entries afterward.
    /// </summary>
    [ExportTsClass]
    public class CreateBudgetTemplateRequest
    {
        public int BudgetId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string TemplateName { get; set; } = "";
    }

    /// <summary>
    /// Response model for a single template in a budget's template list
    /// </summary>
    [ExportTsInterface]
    public class SummaryBudgetTemplateResponse
    {
        // Class properties
        public int BudgetTemplateId { get; set; } = 0;
        public string TemplateName { get; set; } = "";
    }


    [ExportTsInterface]
    public class GetAllBudgetTemplatesResponse : BaseResponse
    {
        // Class properties
        public List<SummaryBudgetTemplateResponse> Templates { get; set; } = new List<SummaryBudgetTemplateResponse>();

        /// <summary>
        /// Default constructor for the get all budget templates response
        /// </summary>
        public GetAllBudgetTemplatesResponse() : base() { }

        /// <summary>
        /// Parameterized constructor with a status and a message
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public GetAllBudgetTemplatesResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }

    /// <summary>
    /// Model for a single envelope entry within a budget template.
    /// EnvelopeId is a display/API-level id - IsSinkingFund says whether it refers to a
    /// sinking_fund_envelope_id or the persistent rollover_envelope_id.
    /// </summary>
    [ExportTsInterface]
    public class GetBudgetTemplateEnvelopeDTOModel
    {
        // Class properties
        public int BudgetTemplateEnvelopeId { get; set; } = 0;
        public int BudgetTemplateId { get; set; } = 0;
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal PlannedAmount { get; set; } = 0m;
        public int OrderIndex { get; set; } = 0;
    }


    [ExportTsInterface]
    public class GetBudgetTemplateDTOModel
    {
        // Class properties
        public int BudgetTemplateId { get; set; } = 0;
        public int BudgetId { get; set; } = 0;
        public string TemplateName { get; set; } = "";
        public List<GetBudgetTemplateEnvelopeDTOModel> Envelopes { get; set; } = new List<GetBudgetTemplateEnvelopeDTOModel>();
    }


    [ExportTsInterface]
    public class GetBudgetTemplateResponse : BaseResponse
    {
        // Class properties
        public GetBudgetTemplateDTOModel Template { get; set; } = new GetBudgetTemplateDTOModel();

        public GetBudgetTemplateResponse() : base() { }
        public GetBudgetTemplateResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }

    /// <summary>
    /// Request model to rename a budget template
    /// </summary>
    [ExportTsClass]
    public class EditBudgetTemplateRequest
    {
        // Class properties
        public int BudgetTemplateId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string? TemplateName { get; set; } = null;
    }

    /// <summary>
    /// Request model for a single item within a template's envelope list being set.
    /// BudgetTemplateEnvelopeId is null for a brand new entry, or the existing entry's id
    /// to update it in place.
    /// </summary>
    [ExportTsClass]
    public class SetBudgetTemplateEnvelopeItemRequest
    {
        // Class properties
        public int? BudgetTemplateEnvelopeId { get; set; }
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal PlannedAmount { get; set; } = 0m;
    }

    /// <summary>
    /// Request model to set (create/reorder/update/remove all at once) a template's envelope
    /// entries. The list's order is the funding priority for whenever this template is applied.
    /// </summary>
    [ExportTsClass]
    public class SetBudgetTemplateEnvelopesRequest
    {
        // Class properties
        public int BudgetTemplateId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public List<SetBudgetTemplateEnvelopeItemRequest> Envelopes { get; set; } = new List<SetBudgetTemplateEnvelopeItemRequest>();
    }

    /// <summary>
    /// Request model to apply a template to a budget month, bulk-creating/updating that
    /// month's envelope funding queue from the template's entries
    /// </summary>
    [ExportTsClass]
    public class ApplyBudgetTemplateRequest
    {
        // Class properties
        public int BudgetTemplateId { get; set; } = 0;
        public int BudgetMonthId { get; set; } = 0;
        public int UserId { get; set; } = 0;
    }

    /// <summary>
    /// Internal helper: one template envelope entry as read back for applying it to a month -
    /// not exposed to the API directly.
    /// </summary>
    public class BudgetTemplateEnvelopeLookupModel
    {
        public int EnvelopeId { get; set; } = 0;
        public bool IsSinkingFund { get; set; } = true;
        public decimal PlannedAmount { get; set; } = 0m;
    }
}