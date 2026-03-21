import React, { useEffect } from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles, safePadding } from "../css/globalStyles";
import BottomNavBar from "../components/bottom-nav-bar";
import { ScrollView, Image, StyleSheet, View } from "react-native";
import { useRouter } from "expo-router";
import { AxiosResponse } from "axios";
import makesCentsAxios from "../data/datasource";
import { GetBudgetResponse } from "../types/get-budget-response";
import { storage } from "../data/storage";
import { store } from "expo-router/build/global-state/router-store";

export default function Home() {
  // Create a router
  const router = useRouter();
  const insets = useSafeAreaInsets();

  // Home constructor
  useEffect(() => {
    const main = async () => {
      // Get the current year and month
      const now: Date = new Date();
      const year: number = now.getFullYear();
      const month: string = now.toLocaleString("default", { month: "long" });

      // Get the budget id from axios
      const axiosResponse: AxiosResponse = await makesCentsAxios.get(
        `/api/budgets/year/${year}/month/${month}`,
      );

      // Get the response
      const response: GetBudgetResponse = axiosResponse.data;
      console.log("Response:", response);
      if (false) {
        console.log("Local budget id:", response.getBudgetDTO.budgetId);

        // Store the budget id in storage
        storage.saveBudgetId(response.getBudgetDTO.budgetId);
        console.log("Stored budget id:", storage.getBudgetId());
      }
    };

    // Call to main
    main();
  }, []);

  const logout = () => {
    // Go back to the login page
    router.replace("/login-register/login");
  };
  return (
    <SafeAreaView style={globalStyles.Screen}>
      <ScrollView contentContainerStyle={safePadding(insets)}>
        <View style={globalStyles.vertLogoContainer}>
          <Image
            source={require("../assets/images/MakesCentsVertLogo.png")}
            style={globalStyles.vertLogo}
          />
        </View>
      </ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({});
