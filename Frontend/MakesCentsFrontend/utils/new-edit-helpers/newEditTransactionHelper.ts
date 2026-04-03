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
import { jsonReplacer, jsonReviver } from "../mappers/jsonReplacer";
import { CreateTransferTransactionRequest } from "@/types/create-transfer-transaction-request";
import { CreatePaymentTransactionResponse } from "@/types/create-payment-transaction-response";
import { CreateTransferTransactionResponse } from "@/types/create-transfer-transaction-response";
import { UpdatePaymentTransactionResponse } from "@/types/update-payment-transaction-response";
import { BaseIdResponse } from "@/types/base-id-response";

export function getUpdatedPaymentFields(
  originalTransaction: GetPaymentTransactionDTOModel,
  currentTransaction: TransactionForm,
): Partial<UpdatePaymentTransactionRequest> {
  const updatedFields: Partial<UpdatePaymentTransactionRequest> = {};

  // Add the transaction id
  updatedFields.transactionId = originalTransaction.transactionId;

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

  // Add the transaction id
  updatedFields.transactionId = originalTransaction.transactionId;

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
  optionalNotes.hasValue =
    transaction.notes !== undefined && transaction.notes !== null;
  optionalNotes.value = transaction.notes || null;
  request.notes = optionalNotes;
  request.accountId = transaction.accountId!;
  request.paymentTransactionType = transaction.paymentTransactionType!;
  request.merchantSourceName = transaction.merchantName!;
  const optionalCheckNumber = new Optional<number>();
  optionalCheckNumber.hasValue =
    transaction.checkNumber !== undefined && transaction.checkNumber !== null;
  optionalCheckNumber.value = transaction.checkNumber || null;
  request.checkNumber = optionalCheckNumber;
  request.transactionSplits = transaction.splits!.map((split) => ({
    transactionId: 0, // Placeholder, will be ignored by the API
    envelopeId: split.envelopeId,
    amount: split.amount,
  }));
  const payload = JSON.stringify(request, jsonReplacer);
  // Log the request
  //console.log("Create Payment Transaction Request:", request);
  console.log("Create Payment Transaction Payload:", payload);

  // Call the API to create a payment transaction
  const axiosResponse: AxiosResponse = await makesCentsAxios.post(
    "/api/payment-transactions",
    payload,
  );

  // Get the response
  const response: CreatePaymentTransactionResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  //console.log("Create Payment Transaction Response:", response);
  return response;
};

export const createTransferTransaction = async (
  transaction: TransactionForm,
  budgetId: number,
) => {
  // Create the request
  const request = new CreateTransferTransactionRequest();
  request.budgetId = budgetId;
  request.userId = 0; // Placeholder, will be set by the API
  request.transactionDate = toDateOnly(transaction.date!);
  request.totalAmount = transaction.amount!;
  const optionalNotes = new Optional<string>();
  optionalNotes.hasValue =
    transaction.notes !== undefined && transaction.notes !== null;
  optionalNotes.value = transaction.notes || null;
  request.notes = optionalNotes;
  request.transferFromId = transaction.fromId!;
  request.transferToId = transaction.toId!;
  request.transferTransactionType = transaction.transferTransactionType!;

  // Get the payload
  const payload = JSON.stringify(request, jsonReplacer);

  // Log the payload
  console.log("Create Transfer Transaction Payload:", payload);

  // Call the API to create a transfer transaction
  const axiosResponse: AxiosResponse = await makesCentsAxios.post(
    "/api/transfer-transactions",
    payload,
  );

  // Get the response
  const response: CreateTransferTransactionResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Create Transfer Transaction Response:", response);
  return response;
};

export const updatePaymentTransaction = async (
  originalTransaction: GetPaymentTransactionDTOModel,
  transaction: TransactionForm,
) => {
  const updatedFields: Partial<UpdatePaymentTransactionRequest> =
    getUpdatedPaymentFields(originalTransaction, transaction);

  // Get the payload
  const payload = JSON.stringify(updatedFields, jsonReplacer);
  // Log the payload
  console.log("Update Payment Transaction Payload:", payload);
  console.log(
    "Payment transaction Id:",
    originalTransaction.paymentTransactionId,
  );

  // Call the API to update a payment transaction
  const axiosResponse: AxiosResponse = await makesCentsAxios.put(
    `/api/payment-transactions/${originalTransaction.paymentTransactionId}`,
    payload,
  );

  // Get the response
  const response: UpdatePaymentTransactionResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Update Payment Transaction Response:", response);
  return response;
};

export const updateTransferTransaction = async (
  originalTransaction: GetTransferTransactionDTOModel,
  transaction: TransactionForm,
) => {
  const updatedFields: Partial<UpdateTransferTransactionRequest> =
    getUpdatedTransferFields(originalTransaction, transaction);

  // Get the payload
  const payload = JSON.stringify(updatedFields, jsonReplacer);
  // Log the payload
  console.log("Update Transfer Transaction Payload:", payload);
  console.log(
    "Transfer transaction Id:",
    originalTransaction.transferTransactionId,
  );

  // Call the API to update a transfer transaction
  const axiosResponse: AxiosResponse = await makesCentsAxios.put(
    `/api/transfer-transactions/${originalTransaction.transferTransactionId}`,
    payload,
  );

  // Get the response
  const response: BaseIdResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Update Transfer Transaction Response:", response);
  return response;
};

export const deleteTransaction = async (transactionId: number) => {
  const axiosResponse: AxiosResponse = await makesCentsAxios.delete(
    `/api/transactions/${transactionId}`,
  );
  console.log("Deleted  Transaction Response:", axiosResponse.data);
};
