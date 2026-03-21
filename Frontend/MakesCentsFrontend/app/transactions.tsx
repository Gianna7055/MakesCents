import React, { useEffect, useState } from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import BottomNavBar from "../components/bottom-nav-bar";
import { ScrollView, Text } from "react-native";
import axios, { AxiosError, AxiosResponse } from "axios";
import makesCentsAxios from "../data/datasource";
import { GetAllTransactionsDTOResponse } from "../types/get-all-transactions-dto-response";
import { SummaryTransactionDTOModel } from "../types/summary-transaction-dto-model";
import { globalStyles, safePadding } from "../css/globalStyles";
import { storage } from "../data/storage";
import TransactionList from "../components/transaction-list";

export default function Transactions() {
  const insets = useSafeAreaInsets();
  const [token, setToken] = useState<string>("");
  const [budgetId, setBudgetId] = useState<number>(0);
  const [transactions, setTransactions] = useState<
    SummaryTransactionDTOModel[]
  >([]);

  // Run on create (constructor)
  useEffect(() => {
    const main = async () => {
      // Load token and budget id from storage
      const storedToken = await storage.getToken();
      const storedBudgetId = await storage.getBudgetId();

      setToken(storedToken || "");
      setBudgetId(storedBudgetId || 0);

      try {
        console.log("BudgetId:", storedBudgetId);
        // Load transactions from the backend
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/transactions/${storedBudgetId}`,
          {
            headers: {
              Authorization: `Bearer ${storedToken}`,
            },
          },
        );

        // Get the response
        const response: GetAllTransactionsDTOResponse = axiosResponse.data;
        console.log("Get All Transactions Response:", response);
        setTransactions(response.transactions);
      } catch (error) {
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

    // Main method call
    main();
  }, []);

  return (
    <SafeAreaView style={globalStyles.Screen}>
      <ScrollView contentContainerStyle={safePadding(insets)}>
        <Text style={globalStyles.Title}>Transactions</Text>
        <TransactionList transactions={transactions} />
      </ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}
