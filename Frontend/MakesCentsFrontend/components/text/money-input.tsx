import { Colors } from "@/constants/theme";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import React, { forwardRef, useState, useEffect } from "react";
import { View, Text, TextInput, StyleSheet } from "react-native";

type InputProps = {
  name: string;
  value: number | null;
  onChangeValue: (value: number | null) => void;
  returnKeyType?: "next" | "done" | "go" | "search" | "send";
  onSubmitEditing?: () => void;
};

const formatCentsToCurrency = (cents: number) => {
  return (cents / 100).toFixed(2);
};

const MoneyInput = forwardRef<TextInput, InputProps>((props, ref) => {
  const [cents, setCents] = useState(0);

  // Sync external value → internal cents
  useEffect(() => {
    if (props.value !== null && props.value !== undefined) {
      setCents(Math.round(props.value * 100));
    } else {
      setCents(0);
    }
  }, [props.value]);

  const handleChange = (text: string) => {
    // Remove all non-digits
    const digits = text.replace(/\D/g, "");

    const newCents = digits ? parseInt(digits, 10) : 0;

    setCents(newCents);

    const dollarValue = newCents / 100;
    props.onChangeValue(dollarValue === 0 ? null : dollarValue);
  };

  const displayValue = formatCentsToCurrency(cents);

  return (
    <View style={styles.inputContainer}>
      <Text style={globalStyles.textHeader}>{props.name}</Text>

      <View style={styles.inputWrapper}>
        {/* 💲 Currency symbol inside input */}
        <Text style={styles.dollarSign}>$</Text>

        <TextInput
          ref={ref}
          style={styles.input}
          value={displayValue}
          onChangeText={handleChange}
          keyboardType="number-pad"
        />
      </View>
    </View>
  );
});

export default MoneyInput;

const styles = StyleSheet.create({
  inputContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingTop: screenWidth * 0.05,
    backgroundColor: Colors.light.background,
  },
  inputWrapper: {
    position: "relative",
    justifyContent: "center",
  },
  dollarSign: {
    position: "absolute",
    left: 12,
    fontSize: 16,
    color: "#555",
    zIndex: 1,
  },
  input: {
    paddingVertical: 15,
    paddingLeft: 25, // 👈 space for $
    backgroundColor: "#FFF",
    borderRadius: 10,
    borderColor: "#C0C0C0",
    borderWidth: 1,
  },
});
