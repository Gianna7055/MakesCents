/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Request model for a single item within a funding queue being set. EnvelopeFundingId is
 * null for a brand new row, or the existing row's id to update it in place.
 */
export class SetEnvelopeFundingItemRequest {
    envelopeFundingId: number;
    envelopeId: number;
    isSinkingFund: boolean = true;
    plannedAmount: number;
}
