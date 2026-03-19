/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { CreateTransactionRequest } from "./create-transaction-request";
import { PaymentTransactionType } from "./payment-transaction-type";
import { Optional } from "./optional";
import { CreateTransactionSplitRequest } from "./create-transaction-split-request";

export class CreatePaymentTransactionRequest extends CreateTransactionRequest {
    accountId: number;
    paymentTransactionType: PaymentTransactionType = 1;
    merchantSourceName: string = "";
    checkNumber: Optional<number>;
    transactionSplits: CreateTransactionSplitRequest[] = [];
}
