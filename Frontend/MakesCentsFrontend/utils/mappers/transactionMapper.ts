import { PaymentTransactionType } from "@/types/payment-transaction-type";
import { TransferTransactionType } from "@/types/transfer-transaction-type";
import { CreatePaymentTransactionRequest } from "@/types/create-payment-transaction-request";
import { toDateOnly } from "@/utils/mappers/dateOnlyMapper";
import { toOptional } from "@/utils/mappers/optionalMapper";
import { CreateTransferTransactionRequest } from "@/types/create-transfer-transaction-request";
import { TransactionType } from "@/types/transaction-type";
import { UpdatePaymentTransactionRequest } from "@/types/update-payment-transaction-request";
import { UpdateTransferTransactionRequest } from "@/types/update-transfer-transaction-request";
import { UpdateTransactionSplitRequest } from "@/types/update-transaction-split-request";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import { GetPaymentTransactionDTOModel } from "@/types/get-payment-transaction-dto-model";
import { GetTransferTransactionDTOModel } from "@/types/get-transfer-transaction-dto-model";

export type TransactionForm = {
  // Determines UI + mapping
  type: TransactionType;
  typeLabel?: string; // For UI selection, e.g. "Expense", "Income", "Transfer"

  // Shared fields
  amount: number | null;
  date: Date | null;
  notes?: string | null;

  // Payment-specific
  accountId: number | null;
  merchantName: string | null;
  checkNumber?: number | null;
  paymentTransactionType: PaymentTransactionType | null;
  // Splits
  splits: EnvelopeSplit[] | null;

  // Transfer-specific
  transferTransactionType: TransferTransactionType | null;
  fromId: number | null;
  toId: number | null;
};

export type EnvelopeSplit = {
  splitId?: number;
  envelopeId: number;
  envelopeName: string;
  amount: number;
};

export const emptyTransactionForm: TransactionForm = {
  type: TransactionType.Payment,
  typeLabel: "Expense",
  amount: null,
  date: new Date(),
  notes: undefined,
  accountId: null,
  merchantName: "",
  checkNumber: undefined,
  paymentTransactionType: 1,
  splits: [],
  transferTransactionType: 2,
  fromId: null,
  toId: null,
};

// Mapper to create a payment transaction
export const mapToCreatePayment = (
  form: TransactionForm,
  budgetId: number,
): CreatePaymentTransactionRequest => {
  return {
    ...new CreatePaymentTransactionRequest(),

    budgetId: budgetId,
    userId: 0,
    transactionId: 0,

    transactionDate: toDateOnly(form.date ?? new Date()),
    totalAmount: form.amount ?? 0,
    notes: toOptional(form.notes),

    accountId: form.accountId ?? 0,
    paymentTransactionType: form.paymentTransactionType ?? 1,
    merchantSourceName: form.merchantName ?? "",
    checkNumber: toOptional(form.checkNumber),

    transactionSplits: form.splits!.map((s) => ({
      transactionId: 0,
      envelopeId: s.envelopeId,
      amount: s.amount,
    })),
  };
};

// Mapper to create a new transfer transaction
export const mapToCreateTransfer = (
  form: TransactionForm,
  budgetId: number,
): CreateTransferTransactionRequest => {
  return {
    ...new CreateTransferTransactionRequest(),

    budgetId: budgetId,
    userId: 0,
    transactionId: 0,

    transactionDate: toDateOnly(form.date ?? new Date()),
    totalAmount: form.amount ?? 0,
    notes: toOptional(form.notes),

    transferTransactionType: form.transferTransactionType ?? 1,
    transferFromId: form.fromId ?? 0,
    transferToId: form.toId ?? 0,
  };
};

/**
 * Maps a TransactionForm + original UpdatePaymentTransactionRequest
 * into a new UpdatePaymentTransactionRequest, only including updated fields.
 */
export const mapToUpdatePayment = (
  form: TransactionForm,
  original: GetPaymentTransactionDTOModel,
): Partial<UpdatePaymentTransactionRequest> => {
  const update: Partial<UpdatePaymentTransactionRequest> = {};

  // Required ID
  update.transactionId = original.transactionId;

  // Optional T? fields (set if changed)
  // Transaction props
  if (form.amount && form.amount !== original.totalAmount)
    update.totalAmount = form.amount;

  if (form.date && toDateOnly(form.date) !== original.transactionDate)
    update.transactionDate = toDateOnly(form.date);

  if (form.notes && form.notes !== original.notes)
    update.notes = toOptional(form.notes);

  // Payment transaction props
  if (form.accountId && form.accountId !== original.accountId)
    update.accountId = form.accountId;

  if (
    form.paymentTransactionType &&
    form.paymentTransactionType !== original.paymentTransactionType
  )
    update.paymentTransactionType = form.paymentTransactionType;

  if (form.merchantName && form.merchantName !== original.merchantSourceName)
    update.merchantSourceName = form.merchantName;

  if (form.checkNumber && form.checkNumber !== original.checkNumber)
    update.checkNumber = toOptional(form.checkNumber);

  // Transaction splits: always include amount + envelopeId, transactionId required
  if (form.splits) {
    update.transactionSplits = form.splits.map((s) => {
      const split = new UpdateTransactionSplitRequest();

      // If editing an existing split, use its ID from originalTransaction
      const originalSplit = original.transactionSplits.find(
        (o) => o.envelopeId === s.envelopeId,
      );

      split.transactionSplitId = originalSplit?.transactionSplitId ?? 0; // 0 for new split
      split.transactionId = original.transactionId;
      split.envelopeId = s.envelopeId;
      split.amount = s.amount;

      return split;
    });
  }

  return update;
};

/**
 * Maps a TransactionForm + original UpdateTransferTransactionRequest
 * into a new UpdateTransferTransactionRequest, only including updated fields.
 */
export const mapToUpdateTransfer = (
  form: TransactionForm,
  original: GetTransferTransactionDTOModel,
): Partial<UpdateTransferTransactionRequest> => {
  const update: Partial<UpdateTransferTransactionRequest> = {};

  // Required IDs
  update.transactionId = original.transactionId;

  // Optional T? fields (set if changed)
  // Transaction props
  if (form.amount && form.amount !== original.totalAmount)
    update.totalAmount = form.amount;

  if (form.date && toDateOnly(form.date) !== original.transactionDate)
    update.transactionDate = toDateOnly(form.date);

  if (form.notes && form.notes !== original.notes)
    update.notes = toOptional(form.notes);

  // Transfer Transaction Props
  if (
    form.transferTransactionType &&
    form.transferTransactionType !== original.transferTransactionType
  )
    update.transferTransactionType = form.transferTransactionType;

  if (form.fromId && form.fromId !== original.transferFromId)
    update.transferFromId = form.fromId;

  if (form.toId && form.toId !== original.transferToId)
    update.transferToId = form.toId;

  return update;
};

export const mapSplitsWithNames = (
  splits: { envelopeId: number; amount: number }[],
  categories: SummaryEnvelopeCategoryResponse[],
): EnvelopeSplit[] => {
  return splits.map((split) => {
    // Search all categories for the matching envelope
    const envelope = categories
      .flatMap((cat) => cat.envelopes)
      .find((e) => e.envelopeId === split.envelopeId);

    if (!envelope) {
      console.warn(`Envelope id ${split.envelopeId} not found in categories`);
    }

    return {
      envelopeId: split.envelopeId,
      envelopeName: envelope?.envelopeName ?? "Unknown", // fallback
      amount: split.amount,
    };
  });
};
