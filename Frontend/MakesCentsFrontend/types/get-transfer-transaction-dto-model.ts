/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GetTransactionDTOModel } from "./get-transaction-dto-model";
import { TransferTransactionType } from "./transfer-transaction-type";

export interface GetTransferTransactionDTOModel extends GetTransactionDTOModel {
    transferTransactionId: number;
    transferFromId: number;
    transferToId: number;
    transferTransactionType: TransferTransactionType;
}
