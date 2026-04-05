/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GetEnvelopeBaseModel } from "./get-envelope-base-model";
import { SummaryTransactionDTOModel } from "./summary-transaction-dto-model";

export interface GetEnvelopeDTOModel extends GetEnvelopeBaseModel {
    transactions: SummaryTransactionDTOModel[];
}
