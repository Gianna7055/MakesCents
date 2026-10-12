/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { PaycheckRegularity } from "./paycheck-regularity";

export interface SummaryPaycheckResponse {
    paycheckId: number;
    startingDate: DateOnly;
    secondaryDate: DateOnly;
    totalAmount: number;
    paycheckRegularity: PaycheckRegularity;
}
