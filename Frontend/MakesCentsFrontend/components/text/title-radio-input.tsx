import { Colors } from "@/constants/theme";
import { screenWidth } from "@/css/globalStyles";
import { AccountType } from "@/types/account-type";
import { TransactionType } from "@/types/transaction-type";
import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";

export type TitleRadioOption = {
  label: string;
  value: string;
  type: TransactionType | AccountType;
};

type TitleRadioInputProps = {
  options: TitleRadioOption[];
  value: string;
  onChange: (value: string) => void;
};

const TitleRadioInput = (props: TitleRadioInputProps) => {
  return (
    <View style={styles.optionsRow}>
      {props.options.map((option) => {
        const isSelected = props.value === option.value;

        return (
          <TouchableOpacity
            key={option.label}
            style={styles.optionContainer}
            onPress={() => props.onChange(option.value)}
          >
            <View
              style={[
                styles.radioOuter,
                isSelected && styles.radioOuterSelected,
              ]}
            >
              {isSelected && <View style={styles.radioInner} />}
            </View>

            <Text style={styles.optionText}>{option.label}</Text>
          </TouchableOpacity>
        );
      })}
    </View>
  );
};

export default TitleRadioInput;

const styles = StyleSheet.create({
  optionsRow: {
    flexDirection: "row",
    justifyContent: "center",
    gap: 15,
    alignItems: "center",
    marginTop: 15,
  },

  optionContainer: {
    flexDirection: "row",
    alignItems: "center",
  },

  radioOuter: {
    height: 20,
    width: 20,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: "#888888", // slightly darker for unchecked
    alignItems: "center",
    justifyContent: "center",
    marginRight: 8,
  },

  radioOuterSelected: {
    borderColor: Colors.light.primary,
  },

  radioInner: {
    height: 10,
    width: 10,
    borderRadius: 5,
    backgroundColor: Colors.light.primary,
  },

  optionText: {
    fontSize: 20, // larger text for compact version
  },
});
