import { globalStyles, screenWidth } from "@/css/globalStyles";
import React, { useState } from "react";
import { StyleSheet, View, Text, TouchableOpacity } from "react-native";
import EnvelopeCard from "@/components/budget/envelope-card";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import { formatCurrency } from "@/utils/formatCurrency";

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

  const renderAmount = () => {
    const total = getCategoryTotal();
    const formatted = formatCurrency(total);
    const style = total < 0 ? globalStyles.redAmount : globalStyles.blackAmount;

    return <Text style={[style, styles.amountText]}>{formatted}</Text>;
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
          <Text style={[styles.amountText]}>{renderAmount()}</Text>
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
    marginBottom: 10,
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
    maxWidth: screenWidth * 0.5,
    flexShrink: 1,
    alignSelf: "flex-start",
    fontFamily: "Roboto",
    fontSize: 20,
    textAlign: "left",
  },
  amountText: {
    textAlign: "right",
    fontSize: 20,
    marginRight: 5,
  },
});
