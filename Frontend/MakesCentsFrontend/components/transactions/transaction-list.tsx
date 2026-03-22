import React, { useEffect, useState } from "react";
import { View, Text } from "react-native";
import { SummaryTransactionDTOModel } from "@/types/summary-transaction-dto-model";
import TransactionCard from "@/components/transactions/transaction-card";
import { SafeAreaView } from "react-native-safe-area-context";
import { storage } from "@/data/storage";
import { screenHeight } from "@/css/globalStyles";

type TransactionListProps = {
  transactions: SummaryTransactionDTOModel[];
};

export default function TransactionList(props: TransactionListProps) {
  const [budgetId, setBudgetId] = useState<number>(0);

  useEffect(() => {
    const loadBudgetId = async () => {
      const storedBudgetId = await storage.getBudgetId();
      setBudgetId(storedBudgetId || 0);
    };
    loadBudgetId();
  }, []);

  const transactionList = (props.transactions || []).map(
    (transaction: SummaryTransactionDTOModel) => {
      return (
        // Returns a table row
        <TransactionCard
          key={transaction.transactionId}
          transaction={transaction}
        />
      );
    },
  );
  return (
    <SafeAreaView>
      {budgetId === 0 ? (
        <Text>Select a budget</Text>
      ) : transactionList.length === 0 ? (
        <Text>No Transactions</Text>
      ) : (
        <>{transactionList}</>
      )}

      {/* For Logo
      <View style={{ marginVertical: -(screenHeight * 0.02) }}>
        {budgetId === 0 ? (
          <Text>Select a budget</Text>
        ) : transactionList.length === 0 ? (
          <Text>No Transactions</Text>
        ) : (
          <>{transactionList}</>
        )}
      </View>*/}
    </SafeAreaView>
  );
}
