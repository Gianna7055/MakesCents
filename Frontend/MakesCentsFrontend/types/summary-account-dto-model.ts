/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { AccountType } from "./account-type";
import { BankAccountType } from "./bank-account-type";
import { DebtAccountType } from "./debt-account-type";
import { InvestmentAccountType } from "./investment-account-type";

export interface SummaryAccountDTOModel {
    accountId: number;
    budgetId: number;
    userId: number;
    accountType: AccountType;
    accountName: string;
    balance: number;
    institution: string;
    bankAccountType: BankAccountType;
    debtAccountType: DebtAccountType;
    investmentAccountType: InvestmentAccountType;
}
