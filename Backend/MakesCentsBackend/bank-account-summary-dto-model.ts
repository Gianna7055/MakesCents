/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { AccountSummaryDTOModel } from "./account-summary-dto-model";
import { BankAccountType } from "./bank-account-type";

export interface BankAccountSummaryDTOModel extends AccountSummaryDTOModel {
    bankAccountId: number;
    bankAccountType: BankAccountType;
}
