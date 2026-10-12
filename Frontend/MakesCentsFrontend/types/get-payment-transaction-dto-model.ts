/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GetTransactionDTOModel } from "./get-transaction-dto-model";
import { PaymentTransactionType } from "./payment-transaction-type";
import { GetTransactionSplitDTOModel } from "./get-transaction-split-dto-model";

export interface GetPaymentTransactionDTOModel extends GetTransactionDTOModel {
    paymentTransactionId: number;
    accountId: number;
    paymentTransactionType: PaymentTransactionType;
    merchantSourceName: string;
    checkNumber: number;
    transactionSplits: GetTransactionSplitDTOModel[];
}
