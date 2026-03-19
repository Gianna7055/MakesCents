/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { PaycheckRegularity } from "./paycheck-regularity";
import { GetPaycheckSplitDTOModel } from "./get-paycheck-split-dto-model";

export interface GetPaycheckDTOModel {
    paycheckId: number;
    budgetId: number;
    paycheckName: string;
    startingDate: DateOnly;
    secondaryDate: DateOnly;
    totalAmount: number;
    paycheckRegularity: PaycheckRegularity;
    paycheckSplits: GetPaycheckSplitDTOModel[];
}
