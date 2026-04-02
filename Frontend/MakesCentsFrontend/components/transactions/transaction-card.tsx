import React from "react";
import { StyleSheet, View, Text, TouchableOpacity } from "react-native";
import { SummaryTransactionDTOModel } from "@/types/summary-transaction-dto-model";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import { formatCurrency } from "@/utils/formatCurrency";
import { formatDate } from "@/utils/formatDate";
import { router } from "expo-router";

type TransactionCardProps = {
  transaction: SummaryTransactionDTOModel;
};

export default function TransactionCard(props: TransactionCardProps) {
  const renderAmount = () => {
    const formatted = formatCurrency(props.transaction.amount);
    if (
      props.transaction.envelopes.includes("->") ||
      props.transaction.amount == 0
    ) {
      return <Text style={globalStyles.blackAmount}>{formatted}</Text>;
    } else if (props.transaction.amount > 0) {
      return <Text style={globalStyles.greenAmount}>{formatted}</Text>;
    } else {
      return <Text style={globalStyles.redAmount}>{formatted}</Text>;
    }
  };

  const transactionClickEH = () => {
    // Navigate to create a new transaction
    router.push(
      `/new-edit-screens/new-edit-transaction?paramTransactionId=${props.transaction.transactionId}&paramTransactionType=${props.transaction.envelopes.includes("->") ? 3 : 2}`,
      //`/new-edit-screens/new-edit-transaction`,
    );
  };

  return (
    <TouchableOpacity onPress={transactionClickEH}>
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
          <Text>{renderAmount()}</Text>
        </View>
        <View style={styles.arrowContainer}>
          <Text>▶</Text>
        </View>
      </View>
    </TouchableOpacity>
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
