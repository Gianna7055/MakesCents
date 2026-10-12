/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GetBudgetTemplateEnvelopeDTOModel } from "./get-budget-template-envelope-dto-model";

export interface GetBudgetTemplateDTOModel {
    budgetTemplateId: number;
    budgetId: number;
    templateName: string;
    envelopes: GetBudgetTemplateEnvelopeDTOModel[];
}
