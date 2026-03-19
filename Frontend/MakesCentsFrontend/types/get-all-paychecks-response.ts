/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { SummaryPaycheckResponse } from "./summary-paycheck-response";

export interface GetAllPaychecksResponse extends BaseResponse {
    paychecks: SummaryPaycheckResponse[];
}
