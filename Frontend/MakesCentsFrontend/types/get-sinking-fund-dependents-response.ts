/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { RolloverEnvelopeDependentModel } from "./rollover-envelope-dependent-model";

/**
 * Response model for the rollover envelopes that use a sinking fund as their transfer target
 */
export interface GetSinkingFundDependentsResponse extends BaseResponse {
    rolloverEnvelopes: RolloverEnvelopeDependentModel[];
}
