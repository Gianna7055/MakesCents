/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BudgetEntityModel } from "./budget-entity-model";

export interface UserDTOModel {
    userId: number;
    username: string;
    email: string;
    isDarkMode: boolean;
    budget: BudgetEntityModel;
}
