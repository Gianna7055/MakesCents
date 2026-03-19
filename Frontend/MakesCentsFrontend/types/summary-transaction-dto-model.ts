/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";

export interface SummaryTransactionDTOModel {
    transactionId: number;
    date: DateOnly;
    location: string;
    envelopes: string;
    amount: number;
}
