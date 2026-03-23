import React, { useEffect, useState } from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text, Image, View, StyleSheet } from "react-native";
import { storage } from "@/data/storage";
import axios, { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { GetBudgetResponse } from "@/types/get-budget-response";
import { GetBudgetDTOModel } from "@/types/get-budget-dto-model";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import EnvelopeCategoryCard from "@/components/budget/envelope-category-card";

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
        //console.log("Response:", response);
        //console.log("Budget:", response.budget);
        //console.log("Envelope Categories:", response.budget.envelopeCategories);
        //console.log("Envelope 1:", response.budget.envelopeCategories[1]);
        if (response) {
          // Store the budget id in storage
          setBudget(response.budget);
          //console.log("Category 1", budget!.envelopeCategories[0]);
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

  const getTotalBudgetRemaining = (): number => {
    if (!budget) return 0;
    return budget.envelopeCategories.reduce((total, category) => {
      return total + getCategoryTotal(category);
    }, 0);
  };

  const getCategoryTotal = (
    category: SummaryEnvelopeCategoryResponse,
  ): number => {
    return category.envelopes.reduce((total, envelope) => {
      return total + envelope.remainingAmount;
    }, 0);
  };

  return (
    <SafeAreaView style={globalStyles.screen}>
      <View style={globalStyles.inLineLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsInLineLogo.png")}
          style={globalStyles.inLineLogo}
        />
      </View>
      <View style={styles.budgetStatusContainer}>
        <Text>Your Budget Makes Cents</Text>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={styles.logo}
        />
      </View>
      {budget ? ( // Block if budget exists
        <View style={styles.budgetNameContainer}>
          <View style={styles.budgetNameGroup}>
            <Text
              style={[globalStyles.centeredTitle, styles.budgetName]}
              numberOfLines={1}
              ellipsizeMode="tail"
            >
              {budget.budgetName}
            </Text>
            <Text style={globalStyles.centeredTitle}>▼</Text>
          </View>
          <Text style={globalStyles.centeredTitle}>
            ${getTotalBudgetRemaining()}
          </Text>
        </View>
      ) : (
        // Block if budget does not exist
        <Text style={globalStyles.centeredTitle}>Budget</Text>
      )}
      <ScrollView>
        {budget?.envelopeCategories.map((category) => (
          <EnvelopeCategoryCard
            key={category.envelopeCategoryId}
            envelopeCategory={category}
          />
        ))}
      </ScrollView>
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
  budgetNameContainer: {
    flexDirection: "row",
    paddingVertical: 7,
    justifyContent: "space-around",
    borderBottomWidth: 1,
    borderColor: "#B2B2B2",
    marginBottom: 15,
  },
  budgetNameGroup: {
    flexDirection: "row",
    justifyContent: "space-between",
  },
  budgetName: {
    width: screenWidth * 0.5,
  },
});
