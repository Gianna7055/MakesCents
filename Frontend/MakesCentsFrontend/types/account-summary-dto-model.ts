/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { AccountType } from "./account-type";

export interface  AccountSummaryDTOModel {
    accountId: number;
    budgetId: number;
    userId: number;
    accountType: AccountType;
    accountName: string;
    balance: number;
}
