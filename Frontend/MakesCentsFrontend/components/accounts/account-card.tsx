import React from "react";
import { View, Text, StyleSheet } from "react-native";
import { useRouter } from "expo-router";
import { AccountSummaryDTOModel } from "@/types/account-summary-dto-model";
import { Button } from "../buttons";

type AccountCardProps = {
  account: AccountSummaryDTOModel;
};

const ProductCard = ({ account }: AccountCardProps) => {
  // Get the router object
  const router = useRouter();

  // Functions to handle back and next button clicks
  const handleSeeMoreClick = () => {};
  return (
    <View style={styles.cardContainer}>
      <Text style={styles.cardTitle}>{account.accountName}</Text>
      <Text style={styles.accountInstitution}>{account.accountType}</Text>
      <View style={styles.cardFlex}>
        <Text style={styles.accountBalance}>Balance: ${account.balance}</Text>
        <Button name="See More ->" onPress={handleSeeMoreClick} />
      </View>
    </View>
  );
};

export default ProductCard;

const styles = StyleSheet.create({
  cardContainer: {
    padding: 20,
    paddingHorizontal: 15,
    margin: 10,
    borderWidth: 1,
    borderColor: "#CCCCCC",
    backgroundColor: "#FFF",
    borderRadius: 10,
    //filter: 'drop-shadow(30px 1px 2px #888888)',
  },
  cardTitle: {
    fontSize: 24,
    color: "#2D3648",
    fontWeight: "bold",
    lineHeight: 34, // 24 * 1.4
  },
  accountInstitution: {
    fontSize: 12,
    color: "#717D96",
    fontWeight: "medium",
    lineHeight: 16, // 12 * 1.33
  },
  accountBalance: {
    fontSize: 16,
    color: "#2D3648",
    fontWeight: "bold",
  },
  cardFlex: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginHorizontal: 10,
  },
});
