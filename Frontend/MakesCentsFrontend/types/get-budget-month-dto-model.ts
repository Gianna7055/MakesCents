/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Month } from "./month";
import { SummaryEnvelopeCategoryResponse } from "./summary-envelope-category-response";

/**
 * DTO model for sending a budget month to the API
 */
export interface GetBudgetMonthDTOModel {
    budgetId: number;
    budgetMonthId: number;
    userId: number;
    month: Month;
    year: number;
    budgetName: string;
    budgetTemplateId: number;
    envelopeCategories: SummaryEnvelopeCategoryResponse[];
}
