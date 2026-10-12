/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";

/**
 * Base fields returned for a sinking fund envelope
 */
export interface GetSinkingFundEnvelopeBaseModel {
    sinkingFundEnvelopeId: number;
    envelopeCategoryId: number;
    envelopeName: string;
    remainingAmount: number;
    goalAmount: number;
    goalEndDate: DateOnly;
}
