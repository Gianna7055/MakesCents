/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Month } from "./month";

/**
 * Shared lookup key: a user id plus a year/month anchor point. Used both to fetch a single
 * budget month (GetBudgetMonthAsync) and as the anchor for a rolling 12-month range
 * (GetYearBudgetsAsync) - not itself a response, just the query key for both.
 */
export class GetBudgetRequest {
    userId: number;
    budgetId: number;
    month: Month;
    year: number;
}
