/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Month } from "./month";

export class CreateBudgetRequest {
    userId: number;
    month: Month = 1;
    year: number;
    budgetName: string = "";
}
