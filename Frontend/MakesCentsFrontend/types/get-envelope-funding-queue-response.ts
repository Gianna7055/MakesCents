/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { GetEnvelopeFundingDTOModel } from "./get-envelope-funding-dto-model";

/**
 * Response model for a budget month's funding queue, in priority order
 */
export interface GetEnvelopeFundingQueueResponse extends BaseResponse {
    fundingQueue: GetEnvelopeFundingDTOModel[];
}
