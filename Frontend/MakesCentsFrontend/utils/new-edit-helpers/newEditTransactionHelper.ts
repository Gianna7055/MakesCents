import { GetPaymentTransactionDTOModel } from "@/types/get-payment-transaction-dto-model";
import { GetTransferTransactionDTOModel } from "@/types/get-transfer-transaction-dto-model";
import { TransactionForm } from "../mappers/transactionMapper";
import makesCentsAxios from "@/data/datasource";
import { AxiosResponse } from "axios";
import { UpdatePaymentTransactionRequest } from "@/types/update-payment-transaction-request";
import { UpdateTransferTransactionRequest } from "@/types/update-transfer-transaction-request";
import { Optional } from "@/types/optional";
import { toDateOnly } from "../mappers/dateOnlyMapper";
import { CreatePaymentTransactionRequest } from "@/types/create-payment-transaction-request";
import { jsonReplacer } from "../mappers/jsonReplacer";

export function getUpdatedPaymentFields(
  originalTransaction: GetPaymentTransactionDTOModel,
  currentTransaction: TransactionForm,
): Partial<UpdatePaymentTransactionRequest> {
  const updatedFields: Partial<UpdatePaymentTransactionRequest> = {};

  // Transaction fields
  if (currentTransaction.amount !== originalTransaction.totalAmount) {
    updatedFields.totalAmount = currentTransaction.amount!;
  }
  if (
    currentTransaction.date &&
    toDateOnly(currentTransaction.date) !== originalTransaction.transactionDate
  ) {
    updatedFields.transactionDate = toDateOnly(currentTransaction.date!);
  }
  if (currentTransaction.notes !== originalTransaction.notes) {
    const optionalNotes = new Optional<string>();
    optionalNotes.hasValue = true;
    optionalNotes.value = currentTransaction.notes!;
    updatedFields.notes = optionalNotes;
  }

  // Payment-specific fields
  if (currentTransaction.accountId !== originalTransaction.accountId) {
    updatedFields.accountId = currentTransaction.accountId!;
  }
  if (
    currentTransaction.merchantName !== originalTransaction.merchantSourceName
  ) {
    updatedFields.merchantSourceName = currentTransaction.merchantName!;
  }
  if (currentTransaction.checkNumber !== originalTransaction.checkNumber) {
    const optionalCheckNumber = new Optional<number>();
    optionalCheckNumber.hasValue = true;
    optionalCheckNumber.value = currentTransaction.checkNumber!;
    updatedFields.checkNumber = optionalCheckNumber;
  }
  if (
    currentTransaction.paymentTransactionType !==
    originalTransaction.paymentTransactionType
  ) {
    updatedFields.paymentTransactionType =
      currentTransaction.paymentTransactionType!;
  }
  updatedFields.transactionSplits = currentTransaction.splits!.map((split) => ({
    transactionId: originalTransaction.transactionId,
    transactionSplitId: split.splitId!,
    envelopeId: split.envelopeId,
    amount: split.amount,
  }));
  return updatedFields;
}

export function getUpdatedTransferFields(
  originalTransaction: GetTransferTransactionDTOModel,
  currentTransaction: TransactionForm,
): Partial<UpdateTransferTransactionRequest> {
  const updatedFields: Partial<UpdateTransferTransactionRequest> = {};

  if (currentTransaction.amount !== originalTransaction.totalAmount) {
    updatedFields.totalAmount = currentTransaction.amount!;
  }
  if (
    currentTransaction.date &&
    toDateOnly(currentTransaction.date) !== originalTransaction.transactionDate
  ) {
    updatedFields.transactionDate = toDateOnly(currentTransaction.date!);
  }
  if (currentTransaction.notes !== originalTransaction.notes) {
    const optionalNotes = new Optional<string>();
    optionalNotes.hasValue = true;
    optionalNotes.value = currentTransaction.notes!;
    updatedFields.notes = optionalNotes;
  }
  if (
    currentTransaction.transferTransactionType !==
    originalTransaction.transferTransactionType
  ) {
    updatedFields.transferTransactionType =
      currentTransaction.transferTransactionType!;
  }
  if (currentTransaction.fromId !== originalTransaction.transferFromId) {
    updatedFields.transferFromId = currentTransaction.fromId!;
  }
  if (currentTransaction.toId !== originalTransaction.transferToId) {
    updatedFields.transferToId = currentTransaction.toId!;
  }

  return updatedFields;
}

export const createPaymentTransaction = async (
  transaction: TransactionForm,
  budgetId: number,
) => {
  // Create the request
  const request = new CreatePaymentTransactionRequest();
  request.budgetId = budgetId;
  request.userId = 0; // Placeholder, will be set by the API
  request.transactionDate = toDateOnly(transaction.date!);
  request.totalAmount = transaction.amount!;
  const optionalNotes = new Optional<string>();
  optionalNotes.hasValue = true;
  optionalNotes.value = transaction.notes || "";
  request.notes = optionalNotes;
  request.accountId = transaction.accountId!;
  request.paymentTransactionType = transaction.paymentTransactionType!;
  request.merchantSourceName = transaction.merchantName!;
  const optionalCheckNumber = new Optional<number>();
  optionalCheckNumber.hasValue = true;
  optionalCheckNumber.value = transaction.checkNumber! || null;
  request.checkNumber = optionalCheckNumber;
  request.transactionSplits = transaction.splits!.map((split) => ({
    transactionId: 0, // Placeholder, will be ignored by the API
    envelopeId: split.envelopeId,
    amount: split.amount,
  }));
  const payload = JSON.stringify(request, jsonReplacer);
  // Log the request
  console.log("Create Payment Transaction Request:", request);
  console.log("Create Payment Transaction Payload:", payload);

  // Call the API to create a payment transaction
  const axiosResponse: AxiosResponse = await makesCentsAxios.post(
    "/api/payment-transactions",
    request,
  );

  // Get the response
  const response = axiosResponse.data;
  // Log the response
  console.log("Create Payment Transaction Response:", response);
  return response;
};

export const createTransferTransaction = async (
  transaction: TransactionForm,
  budgetId: number,
) => {
  // Call the API to create a transfer transaction
  const axiosResponse: AxiosResponse = await makesCentsAxios.post(
    "/api/transfer-transactions",
    {
      budgetId: budgetId,
      transactionDate: transaction.date,
      totalAmount: transaction.amount!,
      notes: transaction.notes,
      transferFromId: transaction.fromId!,
      transferToId: transaction.toId!,
      transferTransactionType: transaction.transferTransactionType!,
    },
  );

  // Get the response
  const response = axiosResponse.data;
  // Log the response
  console.log("Create Transfer Transaction Response:", response);
  return response;
};

export const updatePaymentTransaction = async (
  originalTransaction: GetPaymentTransactionDTOModel,
  transaction: TransactionForm,
) => {
  if (originalTransaction.transactionType !== transaction.type) {
    const axiosResponse: AxiosResponse = await makesCentsAxios.delete(
      `/api/payment-transactions/${originalTransaction.transactionId}`,
    );
    console.log(
      "Deleted Original Payment Transaction Response:",
      axiosResponse.data,
    );
    return createTransferTransaction(transaction, originalTransaction.budgetId);
  }

  const updatedFields: Partial<UpdatePaymentTransactionRequest> =
    getUpdatedPaymentFields(originalTransaction, transaction);

  // Call the API to update a payment transaction
  const axiosResponse: AxiosResponse = await makesCentsAxios.put(
    `/api/payment-transactions/${originalTransaction.transactionId}`,
    updatedFields,
  );

  // Get the response
  const response = axiosResponse.data;
  // Log the response
  console.log("Update Payment Transaction Response:", response);
  return response;
};

export const updateTransferTransaction = async (
  originalTransaction: GetTransferTransactionDTOModel,
  transaction: TransactionForm,
) => {
  if (originalTransaction.transactionType !== transaction.type) {
    const axiosResponse: AxiosResponse = await makesCentsAxios.delete(
      `/api/transfer-transactions/${originalTransaction.transactionId}`,
    );
    console.log(
      "Deleted Original Transfer Transaction Response:",
      axiosResponse.data,
    );
    return createPaymentTransaction(transaction, originalTransaction.budgetId);
  } else {
    const updatedFields: Partial<UpdateTransferTransactionRequest> =
      getUpdatedTransferFields(originalTransaction, transaction);
    // Call the API to update a transfer transaction
    const axiosResponse: AxiosResponse = await makesCentsAxios.put(
      `/api/transfer-transactions/${originalTransaction.transactionId}`,
      updatedFields,
    );

    // Get the response
    const response = axiosResponse.data;
    // Log the response
    console.log("Update Transfer Transaction Response:", response);
    return response;
  }
};
