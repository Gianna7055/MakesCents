/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { DateOnly } from "./date-only";
import { Optional } from "./optional";

export interface UpdateTransactionRequest {
    userId: number;
    transactionId: number;
    transactionDate: DateOnly;
    totalAmount: number;
    notes: Optional<string>;
}
