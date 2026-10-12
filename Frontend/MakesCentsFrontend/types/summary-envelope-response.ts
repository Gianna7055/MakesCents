/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Response model for a single envelope for the Get Budget method.
 * Sinking fund and rollover envelopes come from two different tables with two different
 * id spaces, so IsSinkingFund tells the frontend which table EnvelopeId belongs to (same
 * pattern SummaryAccountDTOModel uses to distinguish bank/debt/investment accounts).
 * RemainingAmount is nullable because a rollover envelope may not be activated for the
 * month being viewed.
 */
export interface SummaryEnvelopeResponse {
    envelopeId: number;
    envelopeCategoryId: number;
    envelopeName: string;
    remainingAmount: number;
    isSinkingFund: boolean;
}
