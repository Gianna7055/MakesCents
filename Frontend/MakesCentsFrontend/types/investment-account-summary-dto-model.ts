/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { AccountSummaryDTOModel } from "./account-summary-dto-model";
import { InvestmentAccountType } from "./investment-account-type";

export interface InvestmentAccountSummaryDTOModel extends AccountSummaryDTOModel {
    investmentAccountId: number;
    investmentAccountType: InvestmentAccountType;
}
