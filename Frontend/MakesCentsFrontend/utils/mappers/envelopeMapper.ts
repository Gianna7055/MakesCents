import { CreateEnvelopeRequest } from "@/types/create-envelope-request";
import { toOptional } from "../mappers/optionalMapper";
import { toDateOnly } from "../mappers/dateOnlyMapper";
import { GetEnvelopeDTOModel } from "@/types/get-envelope-dto-model";
import { UpdateEnvelopeRequest } from "@/types/update-envelope-request";

export type EnvelopeForm = {
  envelopeCategoryId: number | null;
  envelopeName: string | null;
  plannedAmount: number | null;
  remainingAmount: number | null;
  isSinkingFund: boolean | null;

  // Sinking fund props
  goalAmount?: number | null;
  goalEndDate?: Date | null;

  // Rollover props
  transferEnvelopeId?: number | null;
};

// Basically a default constructor
export const emptyEnvelopeForm: EnvelopeForm = {
  envelopeCategoryId: null,
  envelopeName: null,
  plannedAmount: null,
  remainingAmount: null,
  isSinkingFund: null,
  goalAmount: undefined,
  goalEndDate: undefined,
  transferEnvelopeId: undefined,
};

export const mapToCreateEnvelope = (
  form: EnvelopeForm,
): CreateEnvelopeRequest => {
  return {
    ...new CreateEnvelopeRequest(),

    envelopeCategoryId: form.envelopeCategoryId ?? 0,
    envelopeName: form.envelopeName ?? "",
    plannedAmount: form.plannedAmount ?? 0,
    remainingAmount: form.plannedAmount ?? 0,
    isSinkingFund: form.isSinkingFund ?? true,
    goalAmount: toOptional(form.goalAmount),
    goalEndDate: toOptional(
      form.goalEndDate ? toDateOnly(form.goalEndDate) : null,
    ),
    transferEnvelopeId: toOptional(form.transferEnvelopeId),
  };
};

export const mapToUpdateEnvelope = (
  form: EnvelopeForm,
  original: GetEnvelopeDTOModel,
): Partial<UpdateEnvelopeRequest> => {
  const update: Partial<UpdateEnvelopeRequest> = {};

  // Required id
  update.envelopeId = original.envelopeId;

  // Optional T? fields (set if changed)
  // Base envelope props
  if (
    form.envelopeCategoryId &&
    form.envelopeCategoryId !== original.envelopeCategoryId
  )
    update.envelopeCategoryId = form.envelopeCategoryId;

  if (form.envelopeName && form.envelopeName !== original.envelopeName)
    update.envelopeName = form.envelopeName;

  if (form.plannedAmount && form.plannedAmount !== original.plannedAmount)
    update.plannedAmount = form.plannedAmount;

  if (form.isSinkingFund && form.isSinkingFund !== original.isSinkingFund)
    update.isSinkingFund = form.isSinkingFund;

  // Sinking fund props
  if (form.goalAmount && form.goalAmount !== original.goalAmount)
    update.goalAmount = toOptional(form.goalAmount);

  if (form.goalEndDate && toDateOnly(form.goalEndDate) !== original.goalEndDate)
    update.goalEndDate = toOptional(
      form.goalEndDate ? toDateOnly(form.goalEndDate) : null,
    );

  // Rollover fund props
  if (
    form.transferEnvelopeId &&
    form.transferEnvelopeId !== original.transferEnvelopeId
  )
    update.transferEnvelopeId = toOptional(form.transferEnvelopeId);

  // Return the update list
  return update;
};
