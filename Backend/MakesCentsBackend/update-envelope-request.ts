/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Optional } from "./optional";
import { DateOnly } from "./date-only";

export class UpdateEnvelopeRequest {
    envelopeId: number;
    envelopeCategoryId: number;
    userId: number;
    envelopeName: string;
    plannedAmount: number;
    isSinkingFund: boolean;
    goalAmount: Optional<number>;
    goalEndDate: Optional<DateOnly>;
    transferEnvelopeId: Optional<number>;
}
