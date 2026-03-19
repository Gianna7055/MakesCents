/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { UpdateAccountRequest } from "./update-account-request";
import { BankAccountType } from "./bank-account-type";

export interface UpdateBankAccountRequest {
    bankAccountId: number;
    bankAccountType: BankAccountType;
}
