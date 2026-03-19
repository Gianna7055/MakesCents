/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { AccountType } from "./account-type";
import { TransactionEntityModel } from "./transaction-entity-model";

export class AccountEntityModel {
    accountId: number;
    budgetId: number;
    accountType: AccountType;
    accountName: string;
    institution: string;
    balance: number;
    createdAt: Date;
    lastUpdatedAt: Date;
    transactions: TransactionEntityModel[];
}
