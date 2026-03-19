/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { AccountSummaryDTOModel } from "./account-summary-dto-model";
import { DebtAccountType } from "./debt-account-type";

export interface DebtAccountSummaryDTOModel extends AccountSummaryDTOModel {
    debtAccountId: number;
    debtAccountType: DebtAccountType;
}
