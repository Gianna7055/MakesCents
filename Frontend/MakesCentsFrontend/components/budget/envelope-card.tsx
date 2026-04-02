import { globalStyles, screenWidth } from "@/css/globalStyles";
import { SummaryEnvelopeResponse } from "@/types/summary-envelope-response";
import { formatCurrency } from "@/utils/formatCurrency";
import React from "react";
import { View, Text, StyleSheet } from "react-native";

type EnvelopeCardProps = {
  envelope: SummaryEnvelopeResponse;
};

export default function EnvelopeCard(props: EnvelopeCardProps) {
  const renderAmount = () => {
    const formatted = formatCurrency(props.envelope.remainingAmount);
    const style =
      props.envelope.remainingAmount < 0
        ? globalStyles.redAmount
        : globalStyles.blackAmount;

    return <Text style={[style, styles.amountText]}>{formatted}</Text>;
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
        <View style={styles.amountContainer}>
          {renderAmount()}
          <Text style={{ fontSize: 16 }}>▶</Text>
        </View>
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
  amountContainer: {
    flexDirection: "row",
    gap: 5,
  },
});
