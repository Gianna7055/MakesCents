/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { SummaryEnvelopeResponse } from "./summary-envelope-response";

export interface SummaryEnvelopeCategoryResponse {
    envelopeCategoryId: number;
    budgetId: number;
    envelopeCategoryName: string;
    envelopes: SummaryEnvelopeResponse[];
}
