/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { UpdateAccountRequest } from "./update-account-request";
import { InvestmentAccountType } from "./investment-account-type";
import { Optional } from "./optional";

export class UpdateInvestmentAccountRequest extends UpdateAccountRequest {
    investmentAccountId: number;
    investmentAccountType: InvestmentAccountType;
    accountNumber: Optional<number>;
    isTaxDeferred: boolean;
    isTextExempt: boolean;
}
