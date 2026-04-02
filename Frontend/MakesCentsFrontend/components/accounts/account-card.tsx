import React from "react";
import { View, Text, StyleSheet } from "react-native";
import { useRouter } from "expo-router";
import { Button } from "../buttons/button";
import {
  screenHeight,
  screenWidth,
} from "@/css/globalStyles";
import { SummaryAccountDTOModel } from "@/types/summary-account-dto-model";
import { formatCurrency } from "@/utils/formatCurrency";

type AccountCardProps = {
  account: SummaryAccountDTOModel;
};

const ProductCard = ({ account }: AccountCardProps) => {
  // Get the router object
  const router = useRouter();

  // Functions to handle back and next button clicks
  const handleSeeMoreClick = () => {};
  return (
    <View style={styles.cardContainer}>
      <View style={styles.accountNameContainer}>
        <Text style={{ fontSize: 24 }}>○ </Text>
        <Text style={styles.cardTitle}>{account.accountName}</Text>
      </View>
      <Text style={styles.institutionText}>{account.institution}</Text>
      <View style={styles.cardFlex}>
        <Text style={styles.balanceText}>
          Balance: {formatCurrency(account.balance)}
        </Text>
        <Button
          name="See More ->"
          onPress={handleSeeMoreClick}
          containerStyle={{ paddingTop: 0 }}
        />
      </View>
    </View>
  );
};

export default ProductCard;

const styles = StyleSheet.create({
  cardContainer: {
    width: screenWidth * 0.9,
    marginHorizontal: "auto",
    backgroundColor: "#FFF",
    borderRadius: 10,
    marginBottom: 15,
    paddingHorizontal: 15,
  },
  cardTitle: {
    fontSize: 24,
    fontWeight: "semibold",
  },
  institutionText: {
    fontSize: 16,
    color: "#717D96",
  },
  balanceText: {
    fontSize: 18,
    fontWeight: "bold",
    maxWidth: screenWidth * 0.5,
  },
  cardFlex: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 15,
  },
  accountNameContainer: {
    flexDirection: "row",
    alignItems: "center",
    marginTop: 15,
  },
});
