/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { SetEnvelopeFundingItemRequest } from "./set-envelope-funding-item-request";

/**
 * Request model to set (create/reorder/update/remove all at once) the funding queue for a
 * budget month. The list's order is the funding priority - item 0 is funded first.
 */
export class SetEnvelopeFundingQueueRequest {
    budgetMonthId: number;
    userId: number;
    fundingQueue: SetEnvelopeFundingItemRequest[] = [];
}
