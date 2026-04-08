import { AccountType } from "@/types/account-type";
import { BankAccountType } from "@/types/bank-account-type";
import { CreateBankAccountRequest } from "@/types/create-bank-account-request";
import { CreateDebtAccountRequest } from "@/types/create-debt-account-request";
import { CreateInvestmentAccountRequest } from "@/types/create-investment-account-request";
import { DebtAccountType } from "@/types/debt-account-type";
import { DebtPaymentRegularity } from "@/types/debt-payment-regularity";
import { InvestmentAccountType } from "@/types/investment-account-type";
import { UpdateBankAccountRequest } from "@/types/update-bank-account-request";
import { UpdateDebtAccountRequest } from "@/types/update-debt-account-request";
import { UpdateInvestmentAccountRequest } from "@/types/update-investment-account-request";
import { toOptional } from "../mappers/optionalMapper";
import { toDateOnly } from "../mappers/dateOnlyMapper";
import { GetBankAccountDTOModel } from "@/types/get-bank-account-dto-model";
import { GetDebtAccountDTOModel } from "@/types/get-debt-account-dto-model";
import { GetInvestmentAccountDTOModel } from "@/types/get-investment-account-dto-model";

export type AccountForm = {
  // Base envelope props
  accountType: AccountType | null;
  accountName: string | null;
  institution: string | null;
  balance: number | null;

  // Bank account props
  bankAccountType?: BankAccountType | null;

  // Debt account props
  debtAccountType?: DebtAccountType | null;
  dateOfNextBill?: Date | null;
  amountOfNextBill?: number | null;
  debtPaymentRegularity?: DebtPaymentRegularity | null;

  // Shared debt/investment prop
  accountNumber?: number | null;

  // Investment account props
  investmentAccountType?: InvestmentAccountType | null;
  isTaxDeferred?: boolean | null;
  isTaxExempt?: boolean | null;
};

// Empty account form
export const emptyAccountForm: AccountForm = {
  accountType: null,
  accountName: null,
  institution: null,
  balance: null,

  // Bank account props
  bankAccountType: undefined,

  // Debt account props
  debtAccountType: undefined,
  dateOfNextBill: undefined,
  amountOfNextBill: undefined,
  debtPaymentRegularity: undefined,

  // Shared debt/investment prop
  accountNumber: undefined,

  // Investment account props
  investmentAccountType: undefined,
  isTaxDeferred: undefined,
  isTaxExempt: undefined,
};

export const mapToCreateBankAccount = (
  form: AccountForm,
  budgetId: number,
): CreateBankAccountRequest => {
  return {
    ...new CreateBankAccountRequest(),

    // Base Account props
    budgetId: budgetId,
    userId: 0,
    accountName: form.accountName ?? "",
    institution: form.institution ?? "",
    balance: form.balance ?? 0,

    // Bank account props
    bankAccountType: form.bankAccountType ?? BankAccountType.Unknown,
  };
};

export const mapToCreateDebtAccount = (
  form: AccountForm,
  budgetId: number,
): CreateDebtAccountRequest => {
  return {
    ...new CreateDebtAccountRequest(),

    // Base Account props
    budgetId: budgetId,
    userId: 0,
    accountName: form.accountName ?? "",
    institution: form.institution ?? "",
    balance: form.balance ?? 0,

    // Debt account props
    debtAccountType: form.debtAccountType ?? DebtAccountType.Unknown,
    debtAccountNumber: toOptional(form.accountNumber),
    dateOfNextBill: toOptional(
      form.dateOfNextBill ? toDateOnly(form.dateOfNextBill) : null,
    ),
    amountOfNextBill: toOptional(form.amountOfNextBill),
    debtPaymentRegularity: toOptional(form.debtPaymentRegularity),
  };
};

export const mapToCreateInvestmentAccount = (
  form: AccountForm,
  budgetId: number,
): CreateInvestmentAccountRequest => {
  return {
    ...new CreateInvestmentAccountRequest(),

    // Base Account props
    budgetId: budgetId,
    userId: 0,
    accountName: form.accountName ?? "",
    institution: form.institution ?? "",
    balance: form.balance ?? 0,

    // Investment account props
    investmentAccountType:
      form.investmentAccountType ?? InvestmentAccountType.Unknown,
    investmentAccountNumber: toOptional(form.accountNumber),
    isTaxDeferred: form.isTaxDeferred ?? false,
    isTaxExempt: form.isTaxExempt ?? false,
  };
};

export const mapToUpdateBankAccount = (
  form: AccountForm,
  original: GetBankAccountDTOModel,
): Partial<UpdateBankAccountRequest> => {
  const update: Partial<UpdateBankAccountRequest> = {};

  // Required id
  update.accountId = original.accountId;

  // Base account props
  if (form.accountName && form.accountName !== original.accountName)
    update.accountName = form.accountName;

  if (form.institution && form.institution !== original.institution)
    update.institution = form.institution;

  // Bank account props
  if (form.bankAccountType && form.bankAccountType !== original.bankAccountType)
    update.bankAccountType = form.bankAccountType;

  return update;
};

export const mapToUpdateDebtAccount = (
  form: AccountForm,
  original: GetDebtAccountDTOModel,
): Partial<UpdateDebtAccountRequest> => {
  const update: Partial<UpdateDebtAccountRequest> = {};

  // Base account props
  if (form.accountName && form.accountName !== original.accountName)
    update.accountName = form.accountName;

  if (form.institution && form.institution !== original.institution)
    update.institution = form.institution;

  // Debt account props
  if (form.debtAccountType && form.debtAccountType !== original.debtAccountType)
    update.debtAccountType = form.debtAccountType;

  if (form.accountNumber && form.accountNumber !== original.accountNumber)
    update.accountNumber = toOptional(form.accountNumber);

  if (
    form.dateOfNextBill &&
    toDateOnly(form.dateOfNextBill) !== original.dateOfNextBill
  )
    update.dateOfNextBill = toOptional(
      form.dateOfNextBill ? toDateOnly(form.dateOfNextBill) : null,
    );

  if (
    form.amountOfNextBill &&
    form.amountOfNextBill !== original.amountOfNextBill
  )
    update.amountOfNextBill = toOptional(form.amountOfNextBill);

  if (
    form.debtPaymentRegularity &&
    form.debtPaymentRegularity !== original.debtPaymentRegularity
  )
    update.debtPaymentRegularity = toOptional(form.debtPaymentRegularity);

  return update;
};

export const mapToUpdateInvestmentAccount = (
  form: AccountForm,
  original: GetInvestmentAccountDTOModel,
): Partial<UpdateInvestmentAccountRequest> => {
  const update: Partial<UpdateInvestmentAccountRequest> = {};

  // Base account props
  if (form.accountName && form.accountName !== original.accountName)
    update.accountName = form.accountName;

  if (form.institution && form.institution !== original.institution)
    update.institution = form.institution;

  // Investment account props
  if (
    form.investmentAccountType &&
    form.investmentAccountType !== original.investmentAccountType
  )
    update.investmentAccountType = form.investmentAccountType;

  if (form.accountNumber && form.accountNumber != original.accountNumber)
    update.accountNumber = toOptional(form.accountNumber);

  if (
    form.isTaxDeferred !== null &&
    form.isTaxDeferred !== original.isTaxDeferred
  )
    update.isTaxDeferred = form.isTaxDeferred;

  if (form.isTaxExempt !== null && form.isTaxExempt !== original.isTaxExempt)
    update.isTextExempt = form.isTaxExempt;

  return update;
};
