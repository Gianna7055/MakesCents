/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseIdResponse } from "./base-id-response";

export interface CreatePaymentTransactionResponse extends BaseIdResponse {
    paymentTransactionId: number;
    transactionSplitIds: number[];
}
