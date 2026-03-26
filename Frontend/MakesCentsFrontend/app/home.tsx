import React, { useEffect } from "react";
import { globalStyles, screenHeight, screenWidth } from "@/css/globalStyles";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Image, StyleSheet, View, Text } from "react-native";
import axios, { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { GetBudgetResponse } from "@/types/get-budget-response";
import { storage } from "@/data/storage";
import { Button } from "@/components/buttons/button";
import { router } from "expo-router";
import ScreenWrapper from "@/components/ui/screen-wrapper";

export default function Home() {
  // Home constructor
  useEffect(() => {
    const main = async () => {
      // Get the current year and month
      const now: Date = new Date(2026, 3, 2, 2, 2, 2, 2);
      const year: number = now.getFullYear();
      const month: string = now.toLocaleString("default", { month: "long" });

      try {
        // Get the budget id from axios
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/budgets/year/${year}/month/${month}`,
        );

        // Get the response
        const response: GetBudgetResponse = axiosResponse.data;
        // Log the response
        //console.log("Response:", response);
        if (response) {
          // Store the budget id in storage
          storage.saveBudgetId(response.budget.budgetId);
        }
      } catch (error: any) {
        if (axios.isAxiosError(error)) {
          console.log(
            "Axios error:",
            error.response?.status,
            error.response?.data,
          );
          if (error.response?.status == 401)
            router.replace("/login-register/login");
        } else {
          console.log("Error:", error);
        }
      }
    };

    // Call to main
    main();
  }, []);

  const AddTransactionClickEH = () => {};

  return (
    <ScreenWrapper>
      <ScrollView>
        <View style={styles.vertLogoContainer}>
          <Image
            source={require("@/assets/images/MakesCentsVertLogo.png")}
            style={styles.vertLogo}
          />
        </View>
        <Button
          name="Add a Transaction"
          onPress={AddTransactionClickEH}
          variant="secondary"
          textStyle={[globalStyles.centeredTitle, { fontWeight: "regular" }]}
        />
      </ScrollView>
      <BottomNavBar />
    </ScreenWrapper>
  );
}

const styles = StyleSheet.create({
  vertLogoContainer: {
    alignItems: "center",
    marginTop: screenHeight * 0.08,
    marginBottom: screenHeight * 0.03,
  },
  vertLogo: {
    width: screenWidth * 0.9, // 60% of screen width
    height: screenWidth * 1.5 * 0.5, // maintain aspect ratio ~2:1
    resizeMode: "contain",
  },
});
