import { GetPaymentTransactionDTOModel } from "@/types/get-payment-transaction-dto-model";
import { GetTransferTransactionDTOModel } from "@/types/get-transfer-transaction-dto-model";
import {
  mapToCreatePayment,
  mapToCreateTransfer,
  mapToUpdatePayment,
  mapToUpdateTransfer,
  TransactionForm,
} from "../mappers/transactionMapper";
import makesCentsAxios from "@/data/datasource";
import { AxiosResponse } from "axios";
import { UpdatePaymentTransactionRequest } from "@/types/update-payment-transaction-request";
import { UpdateTransferTransactionRequest } from "@/types/update-transfer-transaction-request";
import { CreatePaymentTransactionRequest } from "@/types/create-payment-transaction-request";
import { jsonReplacer, jsonReviver } from "../mappers/jsonReplacer";
import { CreateTransferTransactionRequest } from "@/types/create-transfer-transaction-request";
import { CreatePaymentTransactionResponse } from "@/types/create-payment-transaction-response";
import { CreateTransferTransactionResponse } from "@/types/create-transfer-transaction-response";
import { UpdatePaymentTransactionResponse } from "@/types/update-payment-transaction-response";
import { BaseIdResponse } from "@/types/base-id-response";

export const createPaymentTransaction = async (
  transaction: TransactionForm,
  budgetId: number,
): Promise<CreatePaymentTransactionResponse> => {
  // Create the request
  const request: CreatePaymentTransactionRequest = mapToCreatePayment(
    transaction,
    budgetId,
  );

  const payload = JSON.stringify(request, jsonReplacer);
  // Log the request
  //console.log("Create Payment Transaction Request:", request);
  //console.log("Create Payment Transaction Payload:", payload);

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
): Promise<CreateTransferTransactionResponse> => {
  // Create the request
  const request: CreateTransferTransactionRequest = mapToCreateTransfer(
    transaction,
    budgetId,
  );

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
): Promise<UpdatePaymentTransactionResponse> => {
  const updatedFields: Partial<UpdatePaymentTransactionRequest> =
    mapToUpdatePayment(transaction, originalTransaction);

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
): Promise<BaseIdResponse> => {
  const updatedFields: Partial<UpdateTransferTransactionRequest> =
    mapToUpdateTransfer(transaction, originalTransaction);

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
  console.log("Delete Transaction Response:", axiosResponse.data);
};
