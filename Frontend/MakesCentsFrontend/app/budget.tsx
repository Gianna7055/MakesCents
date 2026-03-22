import React, { useEffect, useState } from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import { globalStyles } from "@/css/globalStyles";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text, Image, View, StyleSheet } from "react-native";
import { storage } from "@/data/storage";
import axios, { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { GetBudgetResponse } from "@/types/get-budget-response";
import { GetBudgetDTOModel } from "@/types/get-budget-dto-model";

export default function Budget() {
  const [budgetId, setBudgetId] = useState<number>(0);
  const [budget, setBudget] = useState<GetBudgetDTOModel>();

  useEffect(() => {
    const main = async () => {
      // Load budget id from storage
      const storedBudgetId = await storage.getBudgetId();
      setBudgetId(storedBudgetId || 0);

      try {
        // Get the budget id from axios
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/budgets/${storedBudgetId}`,
        );

        // Get the response
        const response: GetBudgetResponse = axiosResponse.data;
        // Log the response
        console.log("Response:", response);
        if (response) {
          // Store the budget id in storage
          setBudget(response.budget);
        }
      } catch (error: any) {
        if (axios.isAxiosError(error)) {
          console.log(
            "Axios error:",
            error.response?.status,
            error.response?.data,
          );
        } else {
          console.log("Error:", error);
        }
      }
    };

    // Main method
    main();
  }, []);

  return (
    <SafeAreaView style={globalStyles.Screen}>
      <View style={styles.budgetStatusContainer}>
        <Text>Your Budget Makes Cents</Text>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={styles.logo}
        />
      </View>
      <Text style={globalStyles.Title}>Budget</Text>
      <ScrollView></ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  budgetStatusContainer: {
    backgroundColor: "#FFF",
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 5,
  },
  logo: {
    width: 30,
    height: 30,
    marginVertical: 4,
  },
});
