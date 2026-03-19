/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { CreateAccountRequest } from "./create-account-request";
import { BankAccountType } from "./bank-account-type";

export class CreateBankAccountRequest extends CreateAccountRequest {
    bankAccountType: BankAccountType = 1;
}
