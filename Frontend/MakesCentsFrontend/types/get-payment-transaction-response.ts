/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { GetPaymentTransactionDTOModel } from "./get-payment-transaction-dto-model";

export interface GetPaymentTransactionResponse extends BaseResponse {
    paymentTransaction: GetPaymentTransactionDTOModel;
}
