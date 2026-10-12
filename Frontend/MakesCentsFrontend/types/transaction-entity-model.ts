/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { TransactionType } from "./transaction-type";

/**
 * Model for an abstract transaction
 */
export class TransactionEntityModel {
    transactionId: number;
    budgetMonthId: number;
    transactionDate: DateOnly;
    totalAmount: number;
    isReconciled: boolean;
    notes: string;
    transactionType: TransactionType;
    createdAt: Date;
    lastUpdatedAt: Date;
    deletedAt: Date;
}
