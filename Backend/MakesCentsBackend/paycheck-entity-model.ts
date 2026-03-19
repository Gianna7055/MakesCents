/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { PaycheckRegularity } from "./paycheck-regularity";
import { PaycheckSplitEntityModel } from "./paycheck-split-entity-model";

export class PaycheckEntityModel {
    paycheckId: number;
    budgetId: number;
    startingDate: DateOnly;
    secondaryDate: DateOnly;
    totalAmount: number;
    paycheckRegularity: PaycheckRegularity = 1;
    createdAt: Date;
    lastUpdatedAt: Date;
    paycheckSplits: PaycheckSplitEntityModel[] = [];
}
