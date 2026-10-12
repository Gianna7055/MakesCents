/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { SummaryAccountDTOModel } from "./summary-account-dto-model";

/**
 * Response model for getting all accounts
 */
export interface GetAllAccountsResponse extends BaseResponse {
    accounts: SummaryAccountDTOModel[];
}
