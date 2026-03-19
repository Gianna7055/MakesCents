/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { EnvelopeEntityModel } from "./envelope-entity-model";

export class EnvelopeCategoryEntityModel {
    envelopeCategoryId: number;
    budgetId: number;
    envelopeCategoryName: string = "";
    createdAt: Date;
    lastUpdatedAt: Date;
    envelopes: EnvelopeEntityModel[] = [];
}
