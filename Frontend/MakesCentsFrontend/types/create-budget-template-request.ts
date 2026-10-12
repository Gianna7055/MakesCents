/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Request model to create a budget template. No envelopes here yet - use
 * SetBudgetTemplateEnvelopesRequest to add/edit its entries afterward.
 */
export class CreateBudgetTemplateRequest {
    budgetId: number;
    userId: number;
    templateName: string = "";
}
