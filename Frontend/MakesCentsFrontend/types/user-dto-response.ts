/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { BaseResponse } from "./base-response";
import { UserDTOModel } from "./user-dto-model";

export interface UserDTOResponse extends BaseResponse {
    user: UserDTOModel;
    token: string;
}
