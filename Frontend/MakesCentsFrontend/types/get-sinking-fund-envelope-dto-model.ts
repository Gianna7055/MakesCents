/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GetSinkingFundEnvelopeBaseModel } from "./get-sinking-fund-envelope-base-model";
import { SummaryTransactionDTOModel } from "./summary-transaction-dto-model";

/**
 * DTO model to get a specific sinking fund envelope
 */
export interface GetSinkingFundEnvelopeDTOModel extends GetSinkingFundEnvelopeBaseModel {
    transactions: SummaryTransactionDTOModel[];
}
