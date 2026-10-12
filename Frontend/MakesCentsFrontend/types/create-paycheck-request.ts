/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { Optional } from "./optional";
import { PaycheckRegularity } from "./paycheck-regularity";

/**
 * Request model to create a paycheck
 */
export class CreatePaycheckRequest {
    budgetId: number;
    userId: number;
    paycheckName: string = "";
    startingDate: DateOnly;
    secondaryDate: Optional<DateOnly>;
    totalAmount: number;
    paycheckRegularity: PaycheckRegularity = 1;
}
