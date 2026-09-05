import BottomNavBar from "@/components/bottom-nav-bar";
import { Button } from "@/components/buttons/button";
import CalendarInput from "@/components/text/calendar-input";
import MoneyInput from "@/components/text/money-input";
import RadioInput from "@/components/text/radio-input";
import SingleDropdownInput from "@/components/text/single-dropdown-input";
import Input from "@/components/text/text-input";
import TransactionList from "@/components/transactions/transaction-list";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import makesCentsAxios from "@/data/datasource";
import { storage } from "@/data/storage";
import { BaseIdResponse } from "@/types/base-id-response";
import { GetAllEnvelopeCategoriesResponse } from "@/types/get-all-envelope-categories-response";
import { GetEnvelopeDTOModel } from "@/types/get-envelope-dto-model";
import { GetEnvelopeDTOResponse } from "@/types/get-envelope-dto-response";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { formatCurrency } from "@/utils/formatCurrency";
import { fromDateOnly } from "@/utils/mappers/dateOnlyMapper";
import {
  emptyEnvelopeForm,
  EnvelopeForm,
} from "@/utils/mappers/envelopeMapper";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import { updateEnvelope } from "@/utils/new-edit-helpers/newEditEnvelopeHelper";
import { AxiosResponse } from "axios";
import { router, useLocalSearchParams } from "expo-router";
import React, { useEffect, useState } from "react";
import { View, Image, Text, ScrollView, StyleSheet } from "react-native";

type ExpandedEnvelopeProps = {
  envelopeId: string;
};

export default function ExpandedEnvelope() {
  //console.log("URL Params:", useLocalSearchParams());
  // Parameter mapping
  const { envelopeId: stringEnvelopeId } =
    useLocalSearchParams<ExpandedEnvelopeProps>();
  // Get the transaction id from the param
  const envelopeId = stringEnvelopeId ? parseInt(stringEnvelopeId) : null;
  const [originalEnvelope, setOriginalEnvelope] =
    useState<GetEnvelopeDTOModel>();
  const [envelope, setEnvelope] = useState<EnvelopeForm>(emptyEnvelopeForm);
  const [envelopeCategories, setEnvelopeCategories] = useState<
    SummaryEnvelopeCategoryResponse[]
  >([]);

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
      // Log the id
      //console.log("Envelope Id:", envelopeId);
      // Set up the try-catch
      try {
        // Get the envelope
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/envelopes/${envelopeId}`,
        );

        // Get the response
        const response: GetEnvelopeDTOResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );
        // Log the response
        //console.log("Get Envelope Response:", response);

        // Get the envelope
        let envelopeDTO = response.envelopeDTO;

        // Set the envelope
        setOriginalEnvelope(envelopeDTO);

        // Create a new envelope form with the envelope DTO props
        const envelopeForm: EnvelopeForm = {
          envelopeCategoryId: envelopeDTO.envelopeCategoryId,
          envelopeName: envelopeDTO.envelopeName,
          plannedAmount: envelopeDTO.plannedAmount,
          remainingAmount: envelopeDTO.remainingAmount,
          isSinkingFund: envelopeDTO.isSinkingFund,
          goalAmount: envelopeDTO.goalAmount,
          goalEndDate: envelopeDTO.goalEndDate
            ? fromDateOnly(envelopeDTO.goalEndDate)
            : null,
          transferEnvelopeId: envelopeDTO.transferEnvelopeId,
        };
        // Set the current envelope the the form
        setEnvelope(envelopeForm);
      } catch (error: any) {
        handleAxiosError(error);
      }
    };

    // Call to main
    main();
  }, []);
  const handleCancelClickEH = () => {
    router.back();
  };

  const handleDoneClickEH = async () => {
    // Set up the try catch
    try {
      // Call the helper method to create the envelope
      const response: BaseIdResponse = await updateEnvelope(
        envelope,
        originalEnvelope as GetEnvelopeDTOModel,
      );
      // Navigate back to the budget screen
      router.replace("/budget");
    } catch (error: any) {
      handleAxiosError(error);
    }
  };

  // Method to update single field K in envelope
  const updateEnvelopeForm = <K extends keyof EnvelopeForm>(
    key: K,
    value: EnvelopeForm[K],
  ) => {
    setEnvelope((prev) => ({ ...prev, [key]: value }));
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
      <RadioInput
        name="Envelope Type"
        options={options}
        value={
          envelope?.isSinkingFund !== null
            ? envelope?.isSinkingFund.toString()
            : ""
        }
        onChange={(value) =>
          updateEnvelopeForm("isSinkingFund", value === "true")
        }
      />
    );
  };

  const renderSinkingFundInputs = () => {
    return (
      <View>
        <MoneyInput
          name="Goal Amount"
          value={envelope.goalAmount || 0}
          onChangeValue={(value) => updateEnvelopeForm("goalAmount", value)}
        />
        <CalendarInput
          name="Goal End Date"
          value={envelope.goalEndDate || new Date()}
          onChange={(value) => updateEnvelopeForm("goalEndDate", value)}
        />
      </View>
    );
  };

  const renderRolloverFundInputs = () => {
    const allEnvelopes = envelopeCategories.flatMap((c) => c.envelopes);
    //console.log("All envelopes:", allEnvelopes);

    return (
      <View>
        <SingleDropdownInput
          name="Rollover Envelope"
          value={
            allEnvelopes.find(
              (e) => e.envelopeId === envelope.transferEnvelopeId,
            ) ?? null
          }
          items={allEnvelopes}
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
        />
      </View>
    );
  };

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

  return (
    <ScreenWrapper>
      {/* Logo and Title */}
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <View style={globalStyles.logoTitleContainer}>
          <Text
            style={globalStyles.logoTitle}
            numberOfLines={2}
            ellipsizeMode="tail"
          >
            {envelope ? envelope.envelopeName : "Edit Envelope"}
          </Text>
        </View>
      </View>
      <ScrollView
        style={{ marginVertical: 0 }}
        contentContainerStyle={{ paddingBottom: 95 }}
      >
        <View style={globalStyles.settingsContainer}>
          <Text style={globalStyles.settingsTitle}>Settings</Text>
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
          />
          <Input
            name={"Envelope Name"}
            placeholder="Name"
            type="text"
            value={envelope.envelopeName || ""}
            onChangeText={(text) => updateEnvelopeForm("envelopeName", text)}
          />
          {renderPlannedAmountInput()}
          {renderEnvelopeTypeRadioButtons()}
          {envelope.isSinkingFund !== null &&
            (envelope.isSinkingFund
              ? renderSinkingFundInputs()
              : renderRolloverFundInputs())}
        </View>

        <View style={styles.amountContainer}>
          <Text style={styles.amountLabel}>Planned Amount:</Text>
          <Text style={styles.amountLabel}>
            {formatCurrency(envelope.plannedAmount || 0)}
          </Text>
        </View>
        <View style={styles.amountContainer}>
          <Text style={styles.amountLabel}>Remaining Amount:</Text>
          <Text style={styles.amountLabel}>
            {formatCurrency(envelope.remainingAmount || 0)}
          </Text>
        </View>
        <Text style={[globalStyles.settingsTitle, { marginTop: 10 }]}>
          Transactions
        </Text>
        <TransactionList transactions={originalEnvelope?.transactions || []} />
      </ScrollView>
      {renderCancelDoneButtons()}
      <BottomNavBar />
    </ScreenWrapper>
  );
}

const styles = StyleSheet.create({
  amountContainer: {
    marginTop: 10,
    justifyContent: "space-between",
    flexDirection: "row",
    marginHorizontal: 20,
  },
  amountLabel: {
    fontSize: 16,
  },
});
