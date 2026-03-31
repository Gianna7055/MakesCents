/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { UpdateTransactionRequest } from "./update-transaction-request";
import { TransferTransactionType } from "./transfer-transaction-type";

export class UpdateTransferTransactionRequest extends UpdateTransactionRequest {
    transferTransactionId: number;
    transferFromId: number;
    transferToId: number;
    transferTransactionType: TransferTransactionType;
}
