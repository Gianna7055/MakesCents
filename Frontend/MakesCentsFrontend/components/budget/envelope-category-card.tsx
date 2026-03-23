import { globalStyles, screenWidth } from "@/css/globalStyles";
import React, { useState } from "react";
import { StyleSheet, View, Text, TouchableOpacity } from "react-native";
import EnvelopeCard from "@/components/budget/envelope-card";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";

type EnvelopeCategoryCardProps = {
  envelopeCategory: SummaryEnvelopeCategoryResponse;
};

export default function EnvelopeCategoryCard(props: EnvelopeCategoryCardProps) {
  const [isOpen, setIsOpen] = useState<boolean>(true);

  const getCategoryTotal = (): number => {
    return props.envelopeCategory.envelopes.reduce((total, envelope) => {
      return total + envelope.remainingAmount;
    }, 0);
  };

  const formatAmount = () => {
    const total = getCategoryTotal();
    const abs = Math.abs(total);
    const formatted = total < 0 ? `-$${abs.toFixed(2)}` : `$${abs.toFixed(2)}`;

    if (total == 0) {
      return <Text style={globalStyles.zeroAmount}>${abs.toFixed(2)}</Text>;
    } else if (total > 0) {
      return <Text style={globalStyles.zeroAmount}>{formatted}</Text>;
    } else {
      return <Text style={globalStyles.negativeAmount}>{formatted}</Text>;
    }
  };

  const onPress = () => {
    setIsOpen(!isOpen);
  };

  return (
    <View style={styles.cardContainer}>
      <TouchableOpacity onPress={onPress}>
        <View style={styles.envelopeCategoryNameContainer}>
          <View style={styles.envelopeCategoryNameGroup}>
            {isOpen ? (
              <Text style={{ fontSize: 20 }}>▼</Text>
            ) : (
              <Text style={{ fontSize: 20 }}>▶</Text>
            )}
            <Text
              style={[globalStyles.centeredTitle, styles.envelopeCategoryName]}
              numberOfLines={1}
              ellipsizeMode="tail"
            >
              {props.envelopeCategory.envelopeCategoryName}
            </Text>
          </View>
          <Text style={[styles.amountText]}>{formatAmount()}</Text>
        </View>
      </TouchableOpacity>
      {isOpen
        ? props.envelopeCategory.envelopes.map((envelope) => (
            <EnvelopeCard key={envelope.envelopeId} envelope={envelope} />
          ))
        : null}
    </View>
  );
}

const styles = StyleSheet.create({
  cardContainer: {
    width: screenWidth * 0.9,
    marginHorizontal: "auto",
    borderColor: "#727272",
    borderWidth: 1,
    padding: 3,
    borderRadius: 20,
    backgroundColor: "#C9DAD0",
  },
  envelopeCategoryNameContainer: {
    flexDirection: "row",
    paddingVertical: 7,
    justifyContent: "space-between",
    alignItems: "center",
    paddingHorizontal: 10,
    marginBottom: 5,
  },
  envelopeCategoryNameGroup: {
    flexDirection: "row",
    alignItems: "flex-start",
    gap: 5,
  },
  envelopeCategoryName: {
    width: screenWidth * 0.5,
    flexShrink: 1,
    alignSelf: "flex-start",
    fontFamily: "Roboto",
    fontSize: 20,
    textAlign: "left",
  },
  amountText: {
    textAlign: "right",
    fontSize: 20,
  },
});
