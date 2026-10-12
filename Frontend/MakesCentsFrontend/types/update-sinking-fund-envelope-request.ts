/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Optional } from "./optional";
import { DateOnly } from "./date-only";

/**
 * Request class for updating a sinking fund envelope
 */
export class UpdateSinkingFundEnvelopeRequest {
    sinkingFundEnvelopeId: number;
    userId: number;
    envelopeCategoryId: number;
    envelopeName: string;
    goalAmount: Optional<number>;
    goalEndDate: Optional<DateOnly>;
}
