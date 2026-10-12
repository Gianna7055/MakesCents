/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { SummaryBudgetResponse } from "./summary-budget-response";

/**
 * Response model for every budget a user has access to
 */
export interface GetUserBudgetsResponse extends BaseResponse {
    budgets: SummaryBudgetResponse[];
}
