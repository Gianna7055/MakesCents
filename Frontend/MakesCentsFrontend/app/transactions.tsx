import React, { useEffect, useState } from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles, safePadding } from "../css/globalStyles";
import BottomNavBar from "../components/bottom-nav-bar";
import { ScrollView, Text } from "react-native";
import TransactionList from "../components/transaction-list";
import { storage } from "../data/storage";
import { AxiosResponse } from "axios";
import makesCentsAxios from "../data/datasource";
import { store } from "expo-router/build/global-state/router-store";
import { GetAllTransactionsDTOResponse } from "../types/get-all-transactions-dto-response";
import { SummaryTransactionDTOModel } from "../types/summary-transaction-dto-model";

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

      // Load transactions from the backend
      const axiosResponse: AxiosResponse = await makesCentsAxios.get(
        `/api/transaction/${storedBudgetId}`,
        {
          headers: {
            Authorization: `Bearer ${storedToken}`,
          },
        },
      );

      // Get the response
      const response: GetAllTransactionsDTOResponse = axiosResponse.data;
      setTransactions(response.transactions);
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
