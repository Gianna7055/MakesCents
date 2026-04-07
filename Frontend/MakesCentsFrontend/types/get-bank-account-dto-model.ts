/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GetAccountBaseModel } from "./get-account-base-model";
import { BankAccountType } from "./bank-account-type";
import { SummaryTransactionDTOModel } from "./summary-transaction-dto-model";

export interface GetBankAccountDTOModel extends GetAccountBaseModel {
    bankAccountId: number;
    bankAccountType: BankAccountType;
    transactions: SummaryTransactionDTOModel[];
}
