/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Month } from "./month";
import { EnvelopeCategoryEntityModel } from "./envelope-category-entity-model";
import { AccountEntityModel } from "./account-entity-model";
import { PaycheckEntityModel } from "./paycheck-entity-model";
import { TransactionEntityModel } from "./transaction-entity-model";
import { PlannedExpenseEntity } from "./planned-expense-entity";

export class BudgetEntityModel {
    budgetId: number;
    userId: number;
    month: Month = 1;
    year: number;
    budgetName: string = "";
    createdAt: Date;
    lastUpdatedAt: Date;
    envelopeCategories: EnvelopeCategoryEntityModel[] = [];
    accounts: AccountEntityModel[] = [];
    paychecks: PaycheckEntityModel[] = [];
    transactions: TransactionEntityModel[] = [];
    plannedExpenses: PlannedExpenseEntity[] = [];
}
