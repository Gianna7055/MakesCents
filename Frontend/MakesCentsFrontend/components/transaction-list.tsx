import React from "react";
import { View, Text } from "react-native";
import { SummaryTransactionDTOModel } from "../types/summary-transaction-dto-model";
import TransactionCard from "./transaction-card";
import { SafeAreaView } from "react-native-safe-area-context";

type TransactionListProps = {
  transactions: SummaryTransactionDTOModel[];
};

export default function TransactionList(props: TransactionListProps) {
  const transactionList = (props.transactions || []).map(
    (transaction: SummaryTransactionDTOModel) => {
      return (
        // Returns a table row
        <TransactionCard transaction={transaction} />
      );
    },
  );
  return (
    <SafeAreaView>
      {transactionList.length === 0 ? (
        <Text>No Transactions</Text>
      ) : (
        <>{transactionList}</>
      )}
    </SafeAreaView>
  );
}
