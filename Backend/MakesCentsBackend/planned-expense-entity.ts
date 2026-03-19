/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { PlannedExpenseRegularity } from "./planned-expense-regularity";
import { Weekday } from "./weekday";
import { PlannedExpenseOccurrence } from "./planned-expense-occurrence";

export class PlannedExpenseEntity {
    plannedExpenseId: number;
    budgetId: number;
    envelopeId: number;
    plannedExpenseRegularity: PlannedExpenseRegularity = 1;
    dayOfMonth: number;
    weekday: Weekday;
    plannedExpenseOccurrence: PlannedExpenseOccurrence;
    amount: number;
    createdAt: Date;
    lastUpdatedAt: Date;
}
