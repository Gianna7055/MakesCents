/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GetAccountBaseModel } from "./get-account-base-model";
import { DebtAccountType } from "./debt-account-type";
import { DateOnly } from "./date-only";
import { DebtPaymentRegularity } from "./debt-payment-regularity";
import { SummaryTransactionDTOModel } from "./summary-transaction-dto-model";

export interface GetDebtAccountDTOModel extends GetAccountBaseModel {
    debtAccountId: number;
    debtAccountType: DebtAccountType;
    accountNumber: number;
    dateOfNextBill: DateOnly;
    amountOfNextBill: number;
    debtPaymentRegularity: DebtPaymentRegularity;
    transactions: SummaryTransactionDTOModel[];
}
