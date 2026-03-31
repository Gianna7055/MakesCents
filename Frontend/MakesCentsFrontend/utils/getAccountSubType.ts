import { SummaryAccountDTOModel } from "@/types/summary-account-dto-model";
import { formatEnum } from "./formatEnum";

export const getAccountSubType = (account: SummaryAccountDTOModel): string => {
  if (account.bankAccountType)
    return formatEnum(account.bankAccountType.toString());
  if (account.debtAccountType)
    return formatEnum(account.debtAccountType.toString());
  if (account.investmentAccountType) {
    if (account.investmentAccountType.toString() == "IRA") return "IRA";
    if (account.investmentAccountType.toString() == "Retirement401K403B")
      return "401K / 403B";
    return formatEnum(account.investmentAccountType.toString());
  }
  return "";
};
