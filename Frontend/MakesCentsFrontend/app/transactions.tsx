import React, { useEffect, useState } from "react";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text, View, Image } from "react-native";
import axios, { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { GetAllTransactionsDTOResponse } from "@/types/get-all-transactions-dto-response";
import { SummaryTransactionDTOModel } from "@/types/summary-transaction-dto-model";
import { globalStyles, screenHeight, screenWidth } from "@/css/globalStyles";
import { storage } from "@/data/storage";
import TransactionList from "@/components/transactions/transaction-list";
import { Button } from "@/components/buttons";
import { router } from "expo-router";
import ScreenWrapper from "@/components/ui/screen-wrapper";

export default function Transactions() {
  const [budgetId, setBudgetId] = useState<number>(0);
  const [transactions, setTransactions] = useState<
    SummaryTransactionDTOModel[]
  >([]);

  // Run on create (constructor)
  useEffect(() => {
    const main = async () => {
      // Load token and budget id from storage
      const storedBudgetId = await storage.getBudgetId();

      setBudgetId(storedBudgetId || 0);

      try {
        //console.log("BudgetId:", storedBudgetId);
        // Load transactions from the backend
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/transactions/${storedBudgetId}`,
        );

        // Get the response
        const response: GetAllTransactionsDTOResponse = axiosResponse.data;
        //const transaction2 = response.transactions.concat(response.transactions,);
        //console.log("Get All Transactions Response:", response);
        setTransactions(response.transactions);
        //setTransactions(transaction2);
      } catch (error) {
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

    // Main method call
    main();
  }, []);

  const handlePlusClick = () => {};

  return (
    <ScreenWrapper>
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <Text style={globalStyles.logoTitle}>Transactions</Text>
      </View>
      <ScrollView style={{ marginVertical: 0 }}>
        <TransactionList transactions={transactions} />
      </ScrollView>
      <Button
        name="+"
        onPress={handlePlusClick}
        containerStyle={{
          position: "absolute",
          bottom: screenHeight * 0.115,
          right: screenWidth * 0.03,
        }}
        style={{
          width: 60,
          height: 60,
          borderRadius: 30,
          justifyContent: "center",
          alignItems: "center",
        }}
        textStyle={{
          fontSize: 30,
          lineHeight: 34,
        }}
        variant="tertiary"
      />
      <BottomNavBar />
    </ScreenWrapper>
  );
}
