/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseIdResponse } from "./base-id-response";
import { GetYearBudgetDTOModel } from "./get-year-budget-dto-model";

export interface GetYearBudgetsResponse extends BaseIdResponse {
    budgets: GetYearBudgetDTOModel[];
}
