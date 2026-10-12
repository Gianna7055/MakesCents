/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { UpdateAccountRequest } from "./update-account-request";
import { DebtAccountType } from "./debt-account-type";
import { Optional } from "./optional";
import { DateOnly } from "./date-only";
import { DebtPaymentRegularity } from "./debt-payment-regularity";

export class UpdateDebtAccountRequest extends UpdateAccountRequest {
    debtAccountId: number;
    debtAccountType: DebtAccountType;
    accountNumber: Optional<number> = {"hasValue":true,"value":null};
    dateOfNextBill: Optional<DateOnly>;
    amountOfNextBill: Optional<number>;
    debtPaymentRegularity: Optional<DebtPaymentRegularity>;
}
