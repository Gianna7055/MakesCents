import { CreateBankAccountRequest } from "@/types/create-bank-account-request";
import { CreateBankAccountResponse } from "@/types/create-bank-account-response";
import {
  AccountForm,
  mapToCreateBankAccount,
  mapToCreateDebtAccount,
  mapToCreateInvestmentAccount,
  mapToUpdateBankAccount,
  mapToUpdateDebtAccount,
  mapToUpdateInvestmentAccount,
} from "../mappers/accountMapper";
import { jsonReplacer, jsonReviver } from "../mappers/jsonReplacer";
import { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { CreateDebtAccountResponse } from "@/types/create-debt-account-response";
import { CreateDebtAccountRequest } from "@/types/create-debt-account-request";
import { CreateInvestmentAccountResponse } from "@/types/create-investment-account-response";
import { CreateInvestmentAccountRequest } from "@/types/create-investment-account-request";
import { GetBankAccountDTOModel } from "@/types/get-bank-account-dto-model";
import { BaseIdResponse } from "@/types/base-id-response";
import { UpdateBankAccountRequest } from "@/types/update-bank-account-request";
import { GetDebtAccountDTOModel } from "@/types/get-debt-account-dto-model";
import { UpdateDebtAccountRequest } from "@/types/update-debt-account-request";
import { GetInvestmentAccountDTOModel } from "@/types/get-investment-account-dto-model";
import { UpdateInvestmentAccountRequest } from "@/types/update-investment-account-request";

export const createBankAccount = async (
  account: AccountForm,
  budgetId: number,
): Promise<CreateBankAccountResponse> => {
  // Create the request
  const request: CreateBankAccountRequest = mapToCreateBankAccount(
    account,
    budgetId,
  );

  // Map to the payload
  const payload = JSON.stringify(request, jsonReplacer);
  // Log the payload
  console.log("Create Bank Account Payload:", payload);

  // Call the API to create a bank account
  const axiosResponse: AxiosResponse = await makesCentsAxios.post(
    "/api/bank-accounts",
    payload,
  );

  // Get the response
  const response: CreateBankAccountResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Create Bank Account Response:", response);

  // Return the response
  return response;
};

export const createDebtAccount = async (
  account: AccountForm,
  budgetId: number,
): Promise<CreateDebtAccountResponse> => {
  // Create the request
  const request: CreateDebtAccountRequest = mapToCreateDebtAccount(
    account,
    budgetId,
  );

  // Map to the payload
  const payload = JSON.stringify(request, jsonReplacer);
  // Log the payload
  console.log("Create Debt Account Payload:", payload);

  // Call the API to create a debt account
  const axiosResponse: AxiosResponse = await makesCentsAxios.post(
    "/api/debt-accounts",
    payload,
  );

  // Get the response
  const response: CreateDebtAccountResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Create Debt Account Response:", response);

  // Return the response
  return response;
};

export const createInvestmentAccount = async (
  account: AccountForm,
  budgetId: number,
): Promise<CreateInvestmentAccountResponse> => {
  // Create the request
  const request: CreateInvestmentAccountRequest = mapToCreateInvestmentAccount(
    account,
    budgetId,
  );

  // Map to the payload
  const payload = JSON.stringify(request, jsonReplacer);
  // Log the payload
  console.log("Create Investment Account Payload:", payload);

  // Call the API to create an investment account
  const axiosResponse: AxiosResponse = await makesCentsAxios.post(
    "/api/investment-accounts",
    payload,
  );

  // Get the response
  const response: CreateInvestmentAccountResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Create Investment Account Response:", response);

  // Return the response
  return response;
};

export const updateBankAccount = async (
  account: AccountForm,
  originalAccount: GetBankAccountDTOModel,
): Promise<BaseIdResponse> => {
  // Get the updated fields
  const updatedFields: Partial<UpdateBankAccountRequest> =
    mapToUpdateBankAccount(account, originalAccount);

  // Get the payload
  const payload = JSON.stringify(updatedFields, jsonReplacer);

  // Call the API to update the bank account
  const axiosResponse: AxiosResponse = await makesCentsAxios.put(
    `/api/bank-accounts/${originalAccount.accountId}`,
    payload,
  );

  // Get the response
  const response: BaseIdResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Update Bank Account Response:", response);

  // Return the response
  return response;
};

export const updateDebtAccount = async (
  account: AccountForm,
  originalAccount: GetDebtAccountDTOModel,
): Promise<BaseIdResponse> => {
  // Get the updated fields
  const updatedFields: Partial<UpdateDebtAccountRequest> =
    mapToUpdateDebtAccount(account, originalAccount);

  // Get the payload
  const payload = JSON.stringify(updatedFields, jsonReplacer);

  // Call the API to update the debt account
  const axiosResponse: AxiosResponse = await makesCentsAxios.put(
    `/api/debt-accounts/${originalAccount.accountId}`,
    payload,
  );

  // Get the response
  const response: BaseIdResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Update Debt Account Response:", response);

  // Return the response
  return response;
};

export const updateInvestmentAccount = async (
  account: AccountForm,
  originalAccount: GetInvestmentAccountDTOModel,
): Promise<BaseIdResponse> => {
  // Get the updated fields
  const updatedFields: Partial<UpdateInvestmentAccountRequest> =
    mapToUpdateInvestmentAccount(account, originalAccount);

  // Get the payload
  const payload = JSON.stringify(updatedFields, jsonReplacer);

  // Call the API to update the investment account
  const axiosResponse: AxiosResponse = await makesCentsAxios.put(
    `/api/investment-accounts/${originalAccount.accountId}`,
    payload,
  );

  // Get the response
  const response: BaseIdResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Update Investment Account Response:", response);

  // Return the response
  return response;
};

export const deleteAccount = async (accountId: number) => {
  const axiosResponse: AxiosResponse = await makesCentsAxios.delete(
    `/api/accounts/${accountId}`,
  );
  console.log("Delete Account Response:", axiosResponse.data);
};
