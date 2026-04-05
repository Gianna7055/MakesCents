/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";

export interface GetEnvelopeBaseModel {
    envelopeId: number;
    envelopeCategoryId: number;
    envelopeName: string;
    plannedAmount: number;
    remainingAmount: number;
    isSinkingFund: boolean;
    goalAmount: number;
    goalEndDate: DateOnly;
    transferEnvelopeId: number;
}
