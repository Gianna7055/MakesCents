/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { Optional } from "./optional";
import { PaycheckRegularity } from "./paycheck-regularity";
import { UpdatePaycheckSplitRequest } from "./update-paycheck-split-request";

export class UpdatePaycheckRequest {
    paycheckId: number;
    userId: number;
    startingDate: DateOnly;
    secondaryDate: Optional<DateOnly>;
    totalAmount: number;
    paycheckRegularity: PaycheckRegularity;
    paycheckSplits: UpdatePaycheckSplitRequest[] = [];
}
