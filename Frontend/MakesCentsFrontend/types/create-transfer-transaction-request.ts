/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { CreateTransactionRequest } from "./create-transaction-request";
import { TransferTransactionType } from "./transfer-transaction-type";

/**
 * Request model to create a new transfer transaction
 */
export class CreateTransferTransactionRequest extends CreateTransactionRequest {
    transferFromId: number;
    transferToId: number;
    transferTransactionType: TransferTransactionType = 1;
    transferFromIsSinkingFund: boolean;
    transferToIsSinkingFund: boolean;
}
