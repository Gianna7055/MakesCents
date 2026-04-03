import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import { Month } from "@/types/month";
import { TransactionType } from "@/types/transaction-type";
import { useLocalSearchParams } from "expo-router";
import React from "react";
import { View, Image, Text } from "react-native";

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

  return (
    <ScreenWrapper>
      {/* Logo and Title */}
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <Text style={globalStyles.logoTitle}>
          New Budget
        </Text>
      </View>
    </ScreenWrapper>
  );
}
