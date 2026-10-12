/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { SetBudgetTemplateEnvelopeItemRequest } from "./set-budget-template-envelope-item-request";

/**
 * Request model to set (create/reorder/update/remove all at once) a template's envelope
 * entries. The list's order is the funding priority for whenever this template is applied.
 */
export class SetBudgetTemplateEnvelopesRequest {
    budgetTemplateId: number;
    userId: number;
    envelopes: SetBudgetTemplateEnvelopeItemRequest[] = [];
}
