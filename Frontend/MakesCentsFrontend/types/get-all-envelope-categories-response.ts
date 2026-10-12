/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { SummaryEnvelopeCategoryResponse } from "./summary-envelope-category-response";

/**
 * Response model for getting all envelope categories
 */
export interface GetAllEnvelopeCategoriesResponse extends BaseResponse {
    envelopeCategories: SummaryEnvelopeCategoryResponse[];
}
