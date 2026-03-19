/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { UserEntityModel } from "./user-entity-model";

export interface UserEntityResponse extends BaseResponse {
    user: UserEntityModel;
    token: string;
}
