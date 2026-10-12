/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { PaycheckRegularity } from "./paycheck-regularity";

/**
 * Model for a paycheck. Forecast-only - a paycheck no longer funds envelopes directly
 * (see envelope_funding for that) and isn't tied to any specific month. It exists purely
 * to represent expected income on a recurring schedule.
 */
export class PaycheckEntityModel {
    paycheckId: number;
    budgetId: number;
    paycheckName: string = "";
    startingDate: DateOnly;
    secondaryDate: DateOnly;
    totalAmount: number;
    paycheckRegularity: PaycheckRegularity = 1;
    createdAt: Date;
    lastUpdatedAt: Date;
}
