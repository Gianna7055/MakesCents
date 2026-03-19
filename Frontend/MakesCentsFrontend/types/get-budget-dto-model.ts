/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Month } from "./month";
import { SummaryEnvelopeCategoryResponse } from "./summary-envelope-category-response";

export interface GetBudgetDTOModel {
    budgetId: number;
    userId: number;
    month: Month;
    year: number;
    budgetName: string;
    envelopeCategories: SummaryEnvelopeCategoryResponse[];
}
