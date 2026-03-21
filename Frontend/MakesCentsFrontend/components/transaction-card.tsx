import React from "react";
import { StyleSheet, View, Text } from "react-native";
import { SummaryTransactionDTOModel } from "../types/summary-transaction-dto-model";
import { formatDate, screenWidth } from "../css/globalStyles";

type TransactionCardProps = {
  transaction: SummaryTransactionDTOModel;
};

export default function TransactionCard(props: TransactionCardProps) {
  const formatAmount = () => {
    const formatted = `$${Math.abs(props.transaction.amount).toFixed(2)}`;
    if (
      props.transaction.envelopes.includes("->") ||
      props.transaction.amount == 0
    ) {
      return <Text style={styles.zeroAmount}>{formatted}</Text>;
    } else if (props.transaction.amount > 0) {
      return <Text style={styles.positiveAmount}>{formatted}</Text>;
    } else if (props.transaction.amount < 0) {
      return <Text style={styles.negativeAmount}>{formatted}</Text>;
    } else {
      return <Text style={styles.zeroAmount}>{formatted}</Text>;
    }
  };

  return (
    <View style={styles.cardContainer}>
      <View>
        <Text>{formatDate(props.transaction.date)}</Text>
      </View>
      <View style={styles.locationContainer}>
        <Text
          numberOfLines={1}
          ellipsizeMode="tail"
          style={styles.locationText}
        >
          {props.transaction.location}
        </Text>
        <Text
          numberOfLines={1}
          ellipsizeMode="tail"
          style={styles.envelopeText}
        >
          {props.transaction.envelopes}
        </Text>
      </View>
      <View>
        <Text>{formatAmount()}</Text>
      </View>
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
    width: screenWidth * 0.9,
    flexDirection: "row",
    alignItems: "center",
    marginHorizontal: "auto",
    justifyContent: "space-around",
    borderRadius: 10,
    marginBottom: 10,
  },
  negativeAmount: {
    color: "#B5362F",
    textAlign: "right",
  },
  zeroAmount: {
    color: "#000000",
    textAlign: "right",
  },
  positiveAmount: {
    color: "#32C54B",
    textAlign: "right",
  },
  locationContainer: {
    width: screenWidth * 0.4,
  },
  locationText: {
    fontSize: 14,
  },
  envelopeText: {
    fontSize: 12,
    color: "#808080",
  },
});
