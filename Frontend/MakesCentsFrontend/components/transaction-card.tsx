import React from "react";
import { StyleSheet, View, Text } from "react-native";
import { screenWidth } from "../css/globalStyles";
import { SummaryTransactionDTOModel } from "../types/summary-transaction-dto-model";

type TransactionCardProps = {
  transaction: SummaryTransactionDTOModel;
};

export default function TransactionCard(props: TransactionCardProps) {
  return (
    <View style={styles.cardContainer}>
      <View>
        <Text>
          {props.transaction.date.month}/{props.transaction.date.day}
        </Text>
      </View>
      <View>
        <Text>{props.transaction.location}</Text>
        <Text>{props.transaction.envelopes}</Text>
      </View>
      <View>{props.transaction.amount}</View>
      <View>
        <Text>{">"}</Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  cardContainer: {
    backgroundColor: "#FFFFFF",
    height: 65,
    width: screenWidth * 0.8,
    flexDirection: "row",
    alignItems: "center",
  },
});
