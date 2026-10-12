/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { SummaryEnvelopeResponse } from "./summary-envelope-response";

/**
 * Model for an envelope category
 */
export class EnvelopeCategoryEntityModel {
    envelopeCategoryId: number;
    budgetId: number;
    envelopeCategoryName: string = "";
    createdAt: Date;
    lastUpdatedAt: Date;
    envelopes: SummaryEnvelopeResponse[] = [];
}
