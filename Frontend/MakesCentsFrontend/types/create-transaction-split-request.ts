/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Request model for creating a new transaction split
 */
export class CreateTransactionSplitRequest {
    transactionId: number;
    envelopeId: number;
    isSinkingFund: boolean = true;
    amount: number;
}
