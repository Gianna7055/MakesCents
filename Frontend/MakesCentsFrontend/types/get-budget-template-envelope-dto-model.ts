/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Model for a single envelope entry within a budget template.
 * EnvelopeId is a display/API-level id - IsSinkingFund says whether it refers to a
 * sinking_fund_envelope_id or the persistent rollover_envelope_id.
 */
export interface GetBudgetTemplateEnvelopeDTOModel {
    budgetTemplateEnvelopeId: number;
    budgetTemplateId: number;
    envelopeId: number;
    isSinkingFund: boolean;
    plannedAmount: number;
    orderIndex: number;
}
