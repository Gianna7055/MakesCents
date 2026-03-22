import React from "react";
import { StyleSheet, View, Text } from "react-native";
import { SummaryTransactionDTOModel } from "@/types/summary-transaction-dto-model";
import { formatDate, screenWidth } from "@/css/globalStyles";
import { Button } from "@/components/buttons";

type TransactionCardProps = {
  transaction: SummaryTransactionDTOModel;
};

export default function TransactionCard(props: TransactionCardProps) {
  const formatAmount = () => {
    const abs = Math.abs(props.transaction.amount);
    const formatted =
      props.transaction.amount < 0
        ? `-$${abs.toFixed(2)}`
        : `$${abs.toFixed(2)}`;

    if (
      props.transaction.envelopes.includes("->") ||
      props.transaction.amount == 0
    ) {
      return <Text style={styles.zeroAmount}>${abs.toFixed(2)}</Text>;
    } else if (props.transaction.amount > 0) {
      return <Text style={styles.positiveAmount}>{formatted}</Text>;
    } else {
      return <Text style={styles.negativeAmount}>{formatted}</Text>;
    }
  };

  return (
    <View style={styles.cardContainer}>
      <View style={styles.dateContainer}>
        <Text style={styles.dateText}>
          {formatDate(props.transaction.date)}
        </Text>
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
      <View style={styles.amountContainer}>
        <Text>{formatAmount()}</Text>
      </View>
      <View style={styles.arrowContainer}>
        <Text>▶</Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  // Container styles
  cardContainer: {
    backgroundColor: "#FFFFFF",
    height: 65,
    width: screenWidth * 0.9,
    flexDirection: "row",
    alignItems: "center",
    marginHorizontal: "auto",
    borderRadius: 10,
    marginBottom: 10,
  },
  dateContainer: {
    marginLeft: screenWidth * 0.03,
    width: screenWidth * 0.16,
  },
  locationContainer: {
    width: screenWidth * 0.42,
  },
  amountContainer: {
    width: screenWidth * 0.2,
  },
  arrowContainer: {
    marginLeft: screenWidth * 0.02,
    width: screenWidth * 0.05,
  },

  // Text styles
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
  locationText: {
    fontSize: 14,
  },
  envelopeText: {
    fontSize: 12,
    color: "#808080",
  },
  dateText: {
    fontSize: 20,
  },
});
