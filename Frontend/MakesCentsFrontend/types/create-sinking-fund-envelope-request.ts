/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Optional } from "./optional";
import { DateOnly } from "./date-only";

/**
 * Request class for creating a sinking fund envelope
 */
export class CreateSinkingFundEnvelopeRequest {
    envelopeCategoryId: number;
    userId: number;
    envelopeName: string = "";
    remainingAmount: number;
    goalAmount: Optional<number>;
    goalEndDate: Optional<DateOnly>;
}
