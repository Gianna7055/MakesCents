/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Month } from "./month";

/**
 * Request model for updating a specific budget month's month/year
 * (e.g. correcting a month created under the wrong month/year)
 */
export class EditBudgetMonthRequest {
    budgetMonthId: number;
    userId: number;
    month: Month;
    year: number;
}
