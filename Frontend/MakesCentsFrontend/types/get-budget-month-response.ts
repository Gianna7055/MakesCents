/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { GetBudgetMonthDTOModel } from "./get-budget-month-dto-model";

/**
 * Response model for getting a budget month
 */
export interface GetBudgetMonthResponse extends BaseResponse {
    budget: GetBudgetMonthDTOModel;
}
