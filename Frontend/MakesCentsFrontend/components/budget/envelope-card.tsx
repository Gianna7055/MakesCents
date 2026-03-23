import { globalStyles, screenWidth } from "@/css/globalStyles";
import { SummaryEnvelopeResponse } from "@/types/summary-envelope-response";
import React from "react";
import { View, Text, StyleSheet } from "react-native";

type EnvelopeCardProps = {
  envelope: SummaryEnvelopeResponse;
};

export default function EnvelopeCard(props: EnvelopeCardProps) {
  const formatAmount = () => {
    const abs = Math.abs(props.envelope.remainingAmount);
    const formatted =
      props.envelope.remainingAmount < 0
        ? `-$${abs.toFixed(2)}`
        : `$${abs.toFixed(2)}`;

    if (props.envelope.remainingAmount == 0) {
      return <Text style={globalStyles.zeroAmount}>${abs.toFixed(2)}</Text>;
    } else if (props.envelope.remainingAmount > 0) {
      return <Text style={globalStyles.zeroAmount}>{formatted}</Text>;
    } else {
      return <Text style={globalStyles.negativeAmount}>{formatted}</Text>;
    }
  };
  return (
    <View style={styles.cardContainer}>
      <View style={styles.envelopeCategoryNameContainer}>
        <View style={styles.envelopeCategoryNameGroup}>
          <Text style={{ fontSize: 16 }}>○</Text>
          <Text
            style={[globalStyles.centeredTitle, styles.envelopeCategoryName]}
            numberOfLines={1}
            ellipsizeMode="tail"
          >
            {props.envelope.envelopeName}
          </Text>
        </View>
        <Text style={[styles.amountText]}>{formatAmount()}</Text>
        <Text style={{ fontSize: 16 }}>▶</Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  cardContainer: {
    width: screenWidth * 0.83,
    marginHorizontal: "auto",
    padding: 3,
    borderRadius: 10,
    backgroundColor: "#FFFFFF",
    marginBottom: 10,
  },
  envelopeCategoryNameContainer: {
    flexDirection: "row",
    paddingVertical: 7,
    justifyContent: "space-between",
    alignItems: "center",
    paddingHorizontal: 10,
  },
  envelopeCategoryNameGroup: {
    flexDirection: "row",
    alignItems: "flex-start",
    gap: 5,
  },
  envelopeCategoryName: {
    width: screenWidth * 0.4,
    flexShrink: 1,
    alignSelf: "flex-start",
    fontFamily: "Roboto",
    fontSize: 16,
    textAlign: "left",
  },
  amountText: {
    textAlign: "right",
    fontSize: 16,
  },
});
