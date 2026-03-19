/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BudgetEntityModel } from "./budget-entity-model";

export class UserEntityModel {
    userId: number;
    username: string = "";
    email: string = "";
    passwordHash: string = "";
    isDarkMode: boolean;
    createdAt: Date;
    lastUpdatedAt: Date;
    budget: BudgetEntityModel;
}
