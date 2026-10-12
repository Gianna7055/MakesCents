/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { GetBankAccountDTOModel } from "./get-bank-account-dto-model";

/**
 * Response model for getting a specific bank account
 */
export interface GetBankAccountDTOResponse extends BaseResponse {
    bankAccount: GetBankAccountDTOModel;
}
