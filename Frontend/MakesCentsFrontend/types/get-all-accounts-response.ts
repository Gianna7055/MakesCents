/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { AccountSummaryDTOModel } from "./account-summary-dto-model";

export interface GetAllAccountsResponse extends BaseResponse {
    accounts: AccountSummaryDTOModel[];
}
