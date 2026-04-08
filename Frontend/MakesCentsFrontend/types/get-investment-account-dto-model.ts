/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GetAccountBaseModel } from "./get-account-base-model";
import { InvestmentAccountType } from "./investment-account-type";
import { SummaryTransactionDTOModel } from "./summary-transaction-dto-model";

export interface GetInvestmentAccountDTOModel extends GetAccountBaseModel {
    investmentAccountId: number;
    investmentAccountType: InvestmentAccountType;
    accountNumber: number;
    isTaxDeferred: boolean;
    isTaxExempt: boolean;
    transactions: SummaryTransactionDTOModel[];
}
