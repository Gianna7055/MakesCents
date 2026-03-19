/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { TransactionEntityModel } from "./transaction-entity-model";

export class EnvelopeEntityModel {
    envelopeId: number;
    envelopeCategoryId: number;
    envelopeName: string = "";
    plannedAmount: number = -1.0;
    remainingAmount: number = -1.0;
    isSinkingFund: boolean = true;
    goalAmount: number;
    goalEndDate: DateOnly;
    transferEnvelopeId: number;
    createdAt: Date;
    lastUpdatedAt: Date;
    transactions: TransactionEntityModel[] = [];
}
