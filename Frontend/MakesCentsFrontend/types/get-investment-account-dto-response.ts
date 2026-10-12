/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { GetInvestmentAccountDTOModel } from "./get-investment-account-dto-model";

/**
 * Response model for getting a specific investment account
 */
export interface GetInvestmentAccountDTOResponse extends BaseResponse {
    investmentAccount: GetInvestmentAccountDTOModel;
}
