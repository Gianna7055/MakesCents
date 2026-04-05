import { AccountType } from "@/types/account-type";
import { BankAccountType } from "@/types/bank-account-type";
import { DateOnly } from "@/types/date-only";
import { DebtAccountType } from "@/types/debt-account-type";
import { DebtPaymentRegularity } from "@/types/debt-payment-regularity";
import { InvestmentAccountType } from "@/types/investment-account-type";
import { Month } from "@/types/month";
import { Optional } from "@/types/optional";
import { PaycheckRegularity } from "@/types/paycheck-regularity";
import { PaymentTransactionType } from "@/types/payment-transaction-type";
import { PlannedExpenseOccurrence } from "@/types/planned-expense-occurrence";
import { PlannedExpenseRegularity } from "@/types/planned-expense-regularity";
import { TransactionType } from "@/types/transaction-type";
import { TransferTransactionType } from "@/types/transfer-transaction-type";
import { Weekday } from "@/types/weekday";

const enumFieldMap = {
  accountType: AccountType,
  bankAccountType: BankAccountType,
  debtAccountType: DebtAccountType,
  debtPaymentRegularity: DebtPaymentRegularity,
  investmentAccountType: InvestmentAccountType,
  month: Month,
  paycheckRegularity: PaycheckRegularity,
  paymentTransactionType: PaymentTransactionType,
  plannedExpenseOccurrence: PlannedExpenseOccurrence,
  plannedExpenseRegularity: PlannedExpenseRegularity,
  transactionType: TransactionType,
  transferTransactionType: TransferTransactionType,
  weekday: Weekday,
};

export const jsonReplacer = (key: string, value: any) => {
  // Handle DateOnly
  if (value instanceof DateOnly) {
    return `${value.year.toString().padStart(4, "0")}-${value.month
      .toString()
      .padStart(2, "0")}-${value.day.toString().padStart(2, "0")}`;
  }

  // Handle Optional<T>
  if (value instanceof Optional) {
    if (!value.hasValue) {
      return undefined;
    }
    // Handle DateOnly inside Optional
    if (value.value instanceof DateOnly) {
      const d = value.value;
      return `${d.year.toString().padStart(4, "0")}-${d.month
        .toString()
        .padStart(2, "0")}-${d.day.toString().padStart(2, "0")}`;
    }
    return value.value;
  }

  // Handle Enums (numeric → string)
  const enumObj = enumFieldMap[key as keyof typeof enumFieldMap];
  if (enumObj && typeof value === "number") {
    return enumObj[value];
  }

  return value;
};

export const jsonReviver = (key: string, value: any) => {
  // Handle DateOnly strings → DateOnly
  if (value === null) {
    return null; // just return null
  }

  if (typeof value === "string" && /^\d{4}-\d{2}-\d{2}$/.test(value)) {
    const [year, month, day] = value.split("-").map(Number);
    const dateOnly = new DateOnly();
    dateOnly.year = year;
    dateOnly.month = month;
    dateOnly.day = day;
    return dateOnly;
  }

  // Handle Enums (string → numeric)
  const enumObj = enumFieldMap[key as keyof typeof enumFieldMap];
  if (enumObj && typeof value === "string") {
    return enumObj[value as keyof typeof enumObj];
  }

  return value;
};
