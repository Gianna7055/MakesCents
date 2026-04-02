import { PaymentTransactionType } from "@/types/payment-transaction-type";
import { TransferTransactionType } from "@/types/transfer-transaction-type";
import { CreatePaymentTransactionRequest } from "@/types/create-payment-transaction-request";
import { toDateOnly } from "@/utils/mappers/dateOnlyMapper";
import { toOptional } from "@/utils/mappers/optionalMapper";
import { CreateTransferTransactionRequest } from "@/types/create-transfer-transaction-request";
import { TransactionType } from "@/types/transaction-type";
import { UpdatePaymentTransactionRequest } from "@/types/update-payment-transaction-request";
import { UpdateTransferTransactionRequest } from "@/types/update-transfer-transaction-request";
import { CreateTransactionSplitRequest } from "@/types/create-transaction-split-request";
import { UpdateTransactionSplitRequest } from "@/types/update-transaction-split-request";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import { DateOnly } from "@/types/date-only";

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

export const emptyForm: TransactionForm = {
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
const mapToCreatePayment = (
  form: TransactionForm,
  base: { budgetId: number; userId: number },
): CreatePaymentTransactionRequest => {
  return {
    ...new CreatePaymentTransactionRequest(),

    budgetId: base.budgetId,
    userId: base.userId,
    transactionId: 0,

    transactionDate: toDateOnly(form.date!),
    totalAmount: form.amount!,
    notes: toOptional(form.notes),

    accountId: form.accountId!,
    paymentTransactionType: form.paymentTransactionType!,
    merchantSourceName: form.merchantName!,
    checkNumber: toOptional(form.checkNumber),

    transactionSplits: form.splits!.map((s) => ({
      transactionId: 0,
      envelopeId: s.envelopeId,
      amount: s.amount,
    })),
  };
};

// Mapper to create a new transfer transaction
const mapToCreateTransfer = (
  form: TransactionForm,
  base: { budgetId: number; userId: number },
): CreateTransferTransactionRequest => {
  return {
    ...new CreateTransferTransactionRequest(),

    budgetId: base.budgetId,
    userId: base.userId,
    transactionId: 0,

    transactionDate: toDateOnly(form.date!),
    totalAmount: form.amount!,
    notes: toOptional(form.notes),

    transferTransactionType: form.transferTransactionType!,
    transferFromId: form.fromId!,
    transferToId: form.toId!,
  };
};

/**
 * Maps a TransactionForm + original UpdatePaymentTransactionRequest
 * into a new UpdatePaymentTransactionRequest, only including updated fields.
 */
export const mapToUpdatePayment = (
  form: TransactionForm,
  original: UpdatePaymentTransactionRequest,
): UpdatePaymentTransactionRequest => {
  const update = new UpdatePaymentTransactionRequest();

  // Required IDs
  update.transactionId = original.transactionId;
  update.userId = original.userId;

  // Optional T? fields (set if changed)
  if (form.amount !== original.totalAmount) update.totalAmount = form.amount!;
  if (form.date) update.transactionDate = toDateOnly(form.date);
  if (form.accountId !== original.accountId) update.accountId = form.accountId!;
  if (form.paymentTransactionType !== original.paymentTransactionType)
    update.paymentTransactionType = form.paymentTransactionType!;
  if (form.merchantName !== original.merchantSourceName)
    update.merchantSourceName = form.merchantName!;

  // Optional<T> fields for nullable DB columns
  update.notes = toOptional(form.notes);
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
  original: UpdateTransferTransactionRequest,
): UpdateTransferTransactionRequest => {
  const update = new UpdateTransferTransactionRequest();

  // Required IDs
  update.transactionId = original.transactionId;
  update.userId = original.userId;

  // Optional T? fields (set if changed)
  if (form.amount !== original.totalAmount) update.totalAmount = form.amount!;
  if (toDateOnly(form.date!) !== original.transactionDate)
    update.transactionDate = toDateOnly(form.date!);
  if (form.fromId !== original.transferFromId)
    update.transferFromId = form.fromId!;
  if (form.toId !== original.transferToId) update.transferToId = form.toId!;
  if (form.transferTransactionType !== original.transferTransactionType)
    update.transferTransactionType = form.transferTransactionType!;

  // Optional<T> fields for nullable DB columns
  update.notes = toOptional(form.notes);

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
