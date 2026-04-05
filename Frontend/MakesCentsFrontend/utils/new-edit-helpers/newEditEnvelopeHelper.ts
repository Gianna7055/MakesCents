import { CreateEnvelopeRequest } from "@/types/create-envelope-request";
import {
  EnvelopeForm,
  mapToCreateEnvelope,
  mapToUpdateEnvelope,
} from "../mappers/envelopeMapper";
import { jsonReplacer, jsonReviver } from "../mappers/jsonReplacer";
import { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { BaseIdResponse } from "@/types/base-id-response";
import { GetEnvelopeDTOModel } from "@/types/get-envelope-dto-model";
import { UpdateEnvelopeRequest } from "@/types/update-envelope-request";

export const createEnvelope = async (
  envelope: EnvelopeForm,
): Promise<BaseIdResponse> => {
  // Create the request
  const request: CreateEnvelopeRequest = mapToCreateEnvelope(envelope);

  // Stringify to get the payload
  const payload = JSON.stringify(request, jsonReplacer);
  // Log the payload
  console.log("Create envelope request:", request);
  console.log("Create envelope payload:", payload);

  // Call the API to create the envelope
  const axiosResponse: AxiosResponse = await makesCentsAxios.post(
    "/api/envelopes",
    payload,
  );

  // Get the response
  const response: BaseIdResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Create envelope response:", response);

  // Return the response
  return response;
};

export const updateEnvelope = async (
  envelope: EnvelopeForm,
  originalEnvelope: GetEnvelopeDTOModel,
): Promise<BaseIdResponse> => {
  // Get the fields to update
  const updates: Partial<UpdateEnvelopeRequest> = mapToUpdateEnvelope(
    envelope,
    originalEnvelope,
  );

  // Get the payload
  const payload = JSON.stringify(updates, jsonReplacer);

  // Call the API to update the envelope
  const axiosResponse: AxiosResponse = await makesCentsAxios.put(
    `/api/envelopes/${originalEnvelope.envelopeId}`,
    payload,
  );

  // Get the response
  const response: BaseIdResponse = JSON.parse(
    JSON.stringify(axiosResponse.data),
    jsonReviver,
  );
  // Log the response
  console.log("Update envelope response:", response);

  // Return the response
  return response;
};

export const deleteEnvelope = async (envelopeId: number) => {
  // Call the API to delete the envelope
  const axiosResponse: AxiosResponse = await makesCentsAxios.delete(
    `/api/envelopes/${envelopeId}`,
  );
  // Log the response
  console.log("Delete envelope response:", axiosResponse.data);
};
