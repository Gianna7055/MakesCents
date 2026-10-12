/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Request model for a single item within a template's envelope list being set.
 * BudgetTemplateEnvelopeId is null for a brand new entry, or the existing entry's id
 * to update it in place.
 */
export class SetBudgetTemplateEnvelopeItemRequest {
    budgetTemplateEnvelopeId: number;
    envelopeId: number;
    isSinkingFund: boolean = true;
    plannedAmount: number;
}
