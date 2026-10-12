/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { UpdateTransactionRequest } from "./update-transaction-request";
import { PaymentTransactionType } from "./payment-transaction-type";
import { Optional } from "./optional";
import { UpdateTransactionSplitRequest } from "./update-transaction-split-request";

export class UpdatePaymentTransactionRequest extends UpdateTransactionRequest {
    paymentTransactionId: number;
    accountId: number;
    paymentTransactionType: PaymentTransactionType;
    merchantSourceName: string;
    checkNumber: Optional<number>;
    transactionSplits: UpdateTransactionSplitRequest[] = [];
}
