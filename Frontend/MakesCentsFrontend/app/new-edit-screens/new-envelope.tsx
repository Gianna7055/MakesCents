import BottomNavBar from "@/components/bottom-nav-bar";
import { Button } from "@/components/buttons/button";
import CalendarInput from "@/components/text/calendar-input";
import MoneyInput from "@/components/text/money-input";
import RadioInput from "@/components/text/radio-input";
import SingleDropdownInput from "@/components/text/single-dropdown-input";
import Input from "@/components/text/text-input";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import makesCentsAxios from "@/data/datasource";
import { storage } from "@/data/storage";
import { BaseIdResponse } from "@/types/base-id-response";
import { GetAllEnvelopeCategoriesResponse } from "@/types/get-all-envelope-categories-response";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import { SummaryEnvelopeResponse } from "@/types/summary-envelope-response";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import {
  emptyEnvelopeForm,
  EnvelopeForm,
} from "@/utils/mappers/envelopeMapper";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import { createEnvelope } from "@/utils/new-edit-helpers/newEditEnvelopeHelper";
import { handleBlur } from "@/utils/touched";
import { AxiosResponse } from "axios";
import { router } from "expo-router";
import React, { useEffect, useState } from "react";
import { View, Image, Text, ScrollView } from "react-native";

export default function NewEnvelope() {
  const [envelope, setEnvelope] = useState<EnvelopeForm>(emptyEnvelopeForm);
  const [envelopeCategories, setEnvelopeCategories] = useState<
    SummaryEnvelopeCategoryResponse[]
  >([]);

  const [touched, setTouched] = useState<
    Partial<Record<keyof EnvelopeForm, boolean>>
  >({});
  const [formError, setFormError] = useState<string | null>(null);
  const [submitted, setSubmitted] = useState<boolean>(false);

  useEffect(() => {
    const main = async () => {
      // Get the budget id from axios
      const budgetId = await storage.getBudgetId();

      try {
        // Call the API to get the envelope categories
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/envelope-categories/budget/${budgetId}`,
        );

        // Get the response
        const response: GetAllEnvelopeCategoriesResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );

        // Make sure a response was received
        if (response) {
          // Set the envelope categories
          setEnvelopeCategories(response.envelopeCategories);
        }
      } catch (error: any) {
        handleAxiosError(error);
      }
    };

    // Call main
    main();
  });

  // Method to update single field K in envelope
  const updateEnvelopeForm = <K extends keyof EnvelopeForm>(
    key: K,
    value: EnvelopeForm[K],
  ) => {
    setEnvelope((prev) => ({ ...prev, [key]: value }));
  };

  const handleCancelClickEH = () => {
    router.back();
  };

  const handleDoneClickEH = async () => {
    // Set the form to submitted
    setSubmitted(true);
    // Make sure the required info was provided
    if (
      !envelope.envelopeCategoryId ||
      !envelope.envelopeName ||
      !envelope.plannedAmount ||
      !envelope.isSinkingFund == null ||
      (envelope.isSinkingFund ? null : !envelope.transferEnvelopeId)
    )
      return;
    // Set up the try catch
    try {
      // Call the helper method to create the envelope
      const response: BaseIdResponse = await createEnvelope(envelope);
      // Navigate back to the budget screen
      router.replace("/budget");
    } catch (error: any) {
      setFormError("Issue with creation");
      handleAxiosError(error);
    }
  };

  // Render component consts
  const renderCancelDoneButtons = () => {
    return (
      <View style={globalStyles.bottomButtons}>
        <Button
          name="Cancel"
          onPress={handleCancelClickEH}
          variant="secondary"
          containerStyle={{ paddingTop: 0 }}
        />
        <Button
          name="Done"
          onPress={handleDoneClickEH}
          variant="primary"
          containerStyle={{ paddingTop: 0 }}
        />
      </View>
    );
  };

  const renderPlannedAmountInput = () => {
    return (
      <View>
        <MoneyInput
          name="Planned Amount"
          value={envelope.plannedAmount}
          onChangeValue={(amount) =>
            updateEnvelopeForm("plannedAmount", amount)
          }
          onBlur={() => handleBlur("plannedAmount", setTouched)}
        />
        {(touched.plannedAmount || submitted) && !envelope.plannedAmount && (
          <Text style={globalStyles.errorText}>
            Planned amount is required.
          </Text>
        )}
      </View>
    );
  };

  const renderEnvelopeTypeRadioButtons = () => {
    const options = [
      {
        label: "Sinking Fund",
        value: "true",
      },
      {
        label: "Rollover Fund",
        value: "false",
      },
    ];

    return (
      <View>
        <RadioInput
          name="Envelope Type"
          options={options}
          value={
            envelope.isSinkingFund !== null
              ? envelope.isSinkingFund.toString()
              : ""
          }
          onChange={(value) =>
            updateEnvelopeForm("isSinkingFund", value === "true")
          }
          onBlur={() => handleBlur("isSinkingFund", setTouched)}
        />
        {(touched.isSinkingFund || submitted) &&
          !envelope.isSinkingFund == null && (
            <Text style={globalStyles.errorText}>
              Envelope type is required.
            </Text>
          )}
      </View>
    );
  };

  const renderSinkingFundInputs = () => {
    return (
      <View>
        <MoneyInput
          name="Goal Amount"
          value={envelope.goalAmount || 0}
          onChangeValue={(value) => updateEnvelopeForm("goalAmount", value)}
          onBlur={() => handleBlur("goalAmount", setTouched)}
        />
        <CalendarInput
          name="Goal End Date"
          value={envelope.goalEndDate || new Date()}
          onChange={(value) => updateEnvelopeForm("goalEndDate", value)}
          onBlur={() => handleBlur("goalEndDate", setTouched)}
        />
      </View>
    );
  };

  const renderRolloverFundInputs = () => {
    return (
      <View>
        <SingleDropdownInput
          name="Rollover Envelope"
          value={envelopeCategories
            .flatMap((c) => c.envelopes)
            .find((e) => e.envelopeId === envelope.transferEnvelopeId)}
          items={envelopeCategories.flatMap((c) => c.envelopes)}
          getLabel={(env) => env!.envelopeName}
          getValue={(env) => env!.envelopeId.toString()}
          groupBy={(env) => {
            const category = envelopeCategories.find((cat) =>
              cat.envelopes.some((e) => e.envelopeId === env!.envelopeId),
            );
            return category?.envelopeCategoryName || null;
          }}
          onChange={(env) =>
            updateEnvelopeForm("transferEnvelopeId", env?.envelopeId!)
          }
          onBlur={() => handleBlur("transferEnvelopeId", setTouched)}
        />
        {(touched.transferEnvelopeId || submitted) &&
          !envelope.transferEnvelopeId && (
            <Text style={globalStyles.errorText}>
              Transfer envelope is required.
            </Text>
          )}
      </View>
    );
  };

  return (
    <ScreenWrapper>
      {/* Logo and Title */}
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <View style={globalStyles.logoTitleContainer}>
          <Text style={globalStyles.logoTitle}>New Envelope</Text>
        </View>
      </View>

      <ScrollView
        style={{ marginVertical: 0 }}
        contentContainerStyle={{ paddingBottom: 75 }}
      >
        <SingleDropdownInput
          name="Envelope Category"
          value={
            envelopeCategories.find(
              (c) => c.envelopeCategoryId === envelope.envelopeCategoryId,
            ) ?? null
          }
          items={envelopeCategories}
          getLabel={(item) => item.envelopeCategoryName} // enum label
          getValue={(item) => item.envelopeCategoryId.toString()}
          onChange={(item) =>
            updateEnvelopeForm("envelopeCategoryId", item.envelopeCategoryId)
          }
          placeholder="Select a category"
          onBlur={() => handleBlur("envelopeCategoryId", setTouched)}
        />
        {(touched.envelopeCategoryId || submitted) &&
          !envelope.envelopeCategoryId && (
            <Text style={globalStyles.errorText}>
              Envelope category is required.
            </Text>
          )}
        <Input
          name={"Envelope Name"}
          placeholder="Name"
          type="text"
          value={envelope.envelopeName || ""}
          onChangeText={(text) => updateEnvelopeForm("envelopeName", text)}
          onBlur={() => handleBlur("envelopeName", setTouched)}
        />
        {(touched.envelopeName || submitted) && !envelope.envelopeName && (
          <Text style={globalStyles.errorText}>Envelope name is required.</Text>
        )}
        {renderPlannedAmountInput()}
        {renderEnvelopeTypeRadioButtons()}
        {envelope.isSinkingFund !== null &&
          (envelope.isSinkingFund
            ? renderSinkingFundInputs()
            : renderRolloverFundInputs())}
        {formError && <Text style={globalStyles.errorText}>{formError}</Text>}
      </ScrollView>
      {renderCancelDoneButtons()}
      <BottomNavBar />
    </ScreenWrapper>
  );
}
