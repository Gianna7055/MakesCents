/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { CreateAccountRequest } from "./create-account-request";
import { DebtAccountType } from "./debt-account-type";
import { Optional } from "./optional";
import { DateOnly } from "./date-only";
import { DebtPaymentRegularity } from "./debt-payment-regularity";

export class CreateDebtAccountRequest extends CreateAccountRequest {
    debtAccountType: DebtAccountType = 1;
    accountNumber: Optional<number>;
    dateOfNextBill: Optional<DateOnly>;
    amountOfNextBill: Optional<number>;
    debtPaymentRegularity: Optional<DebtPaymentRegularity>;
}
