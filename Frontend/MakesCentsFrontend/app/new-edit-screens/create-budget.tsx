import BottomNavBar from "@/components/bottom-nav-bar";
import { Button } from "@/components/buttons/button";
import Input from "@/components/text/text-input";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import { CreateBudgetRequest } from "@/types/create-budget-request";
import { Month } from "@/types/month";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { router, useLocalSearchParams } from "expo-router";
import React, { useState } from "react";
import { View, Image, Text, ScrollView } from "react-native";

type NewEditBudgetProps = {
  yearString: string;
  monthString: string;
};

export default function NewEditBudget() {
  //console.log("URL Params:", useLocalSearchParams());
  // Parameter mapping
  const { yearString, monthString } =
    useLocalSearchParams<NewEditBudgetProps>();
  const year: number = parseInt(yearString);
  const monthInt: number = parseInt(monthString);
  const month: Month = monthInt as Month;
  const [budgetName, setBudgetName] = useState<string>("");

  const handleCancelClickEH = () => {
    router.back();
  };

  const handleDoneClickEH = () => {
    try {
        // Create the request
        const request: CreateBudgetRequest = {
            
            budgetName,
            month,
            year
        };
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
        <Text style={globalStyles.logoTitle}>New Budget</Text>
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
