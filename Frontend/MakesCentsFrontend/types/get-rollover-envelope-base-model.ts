/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Base fields returned for a rollover envelope in the context of one budget month.
 * RemainingAmount/RolloverEnvelopeMonthId are null if not yet activated for that month.
 * Planned funding for the month lives on envelope_funding, not here.
 */
export interface GetRolloverEnvelopeBaseModel {
    rolloverEnvelopeId: number;
    envelopeCategoryId: number;
    envelopeName: string;
    transferEnvelopeId: number;
    rolloverEnvelopeMonthId: number;
    remainingAmount: number;
}
