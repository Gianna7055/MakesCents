/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { CreateAccountRequest } from "./create-account-request";
import { InvestmentAccountType } from "./investment-account-type";
import { Optional } from "./optional";

export class CreateInvestmentAccountRequest extends CreateAccountRequest {
    investmentAccountType: InvestmentAccountType = 1;
    investmentAccountNumber: Optional<number>;
    isTaxDeferred: boolean;
    isTaxExempt: boolean;
}
