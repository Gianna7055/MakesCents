/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Month } from "./month";

/**
 * Request model for creating a new month under an existing persistent budget
 */
export class CreateBudgetMonthRequest {
    userId: number;
    budgetId: number;
    budgetTemplateId: number;
    month: Month = 1;
    year: number;
}
