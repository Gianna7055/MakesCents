import BottomNavBar from "@/components/bottom-nav-bar";
import { Button } from "@/components/buttons/button";
import Input from "@/components/text/text-input";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import makesCentsAxios from "@/data/datasource";
import { storage } from "@/data/storage";
import { BaseIdResponse } from "@/types/base-id-response";
import { CreateBudgetRequest } from "@/types/create-budget-request";
import { Month } from "@/types/month";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import { AxiosResponse } from "axios";
import { router, useLocalSearchParams } from "expo-router";
import React, { useState } from "react";
import { View, Image, Text, ScrollView } from "react-native";

type NewBudgetProps = {
  year: string;
  month: string;
};

export default function NewBudget() {
  //console.log("URL Params:", useLocalSearchParams());
  // Parameter mapping
  const { year: yearString, month: monthString } =
    useLocalSearchParams<NewBudgetProps>();
  const year: number = parseInt(yearString);
  const monthInt: number = parseInt(monthString);
  const month: Month = monthInt as Month;
  const [budgetName, setBudgetName] = useState<string>("");

  const handleCancelClickEH = () => {
    router.back();
  };

  const handleDoneClickEH = async () => {
    try {
      // Create the request
      const request: CreateBudgetRequest = {
        userId: 0, // This will be set by the backend based on the authenticated user
        budgetName,
        month,
        year,
      };

      // Call the API
      const axiosResponse: AxiosResponse = await makesCentsAxios.post(
        "/api/budgets",
        request,
      );

      // Get the response
      const response: BaseIdResponse = JSON.parse(
        JSON.stringify(axiosResponse.data),
        jsonReviver,
      );
      // Log the response
      console.log("Create Budget Response:", response);

      // Set the new budget id
      await storage.saveBudgetId(response.id);
      router.push("/budget");
    } catch (error: any) {
      handleAxiosError(error);
    }
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
          <Text style={globalStyles.logoTitle}>New Budget for</Text>
          <Text style={globalStyles.logoTitle}>
            {Month[month]} {year}
          </Text>
        </View>
      </View>
      {/* Content */}
      <ScrollView style={{ marginVertical: 0 }}>
        {/* Budget creation form goes here */}
        <Input
          name="Budget Name"
          placeholder="Budget Name"
          type="text"
          value={budgetName || ""}
          onChangeText={setBudgetName}
          autoCapitalize="words"
        />
      </ScrollView>
      {renderCancelDoneButtons()}
      <BottomNavBar />
    </ScreenWrapper>
  );
}
