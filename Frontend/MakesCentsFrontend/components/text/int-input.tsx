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
  placeHolder?: string;
};

const IntInput = forwardRef<TextInput, InputProps>((props, ref) => {
  const [textValue, setTextValue] = useState("");

  // Sync external value → internal string
  useEffect(() => {
    if (props.value !== null && props.value !== undefined) {
      setTextValue(props.value.toString());
    } else {
      setTextValue("");
    }
  }, [props.value]);

  const handleChange = (text: string) => {
    // Remove non-digits
    const digits = text.replace(/\D/g, "");

    setTextValue(digits);

    if (digits === "") {
      props.onChangeValue(null);
    } else {
      props.onChangeValue(parseInt(digits, 10));
    }
  };

  return (
    <View style={styles.inputContainer}>
      <Text style={globalStyles.textHeader}>{props.name}</Text>

      <TextInput
        ref={ref}
        style={styles.input}
        value={textValue}
        onChangeText={handleChange}
        keyboardType="number-pad"
        returnKeyType={props.returnKeyType}
        onSubmitEditing={props.onSubmitEditing}
        placeholder={props.placeHolder}
      />
    </View>
  );
});

export default IntInput;

const styles = StyleSheet.create({
  inputContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingTop: screenWidth * 0.05,
  },
  input: {
    paddingVertical: 15,
    paddingHorizontal: 12,
    backgroundColor: "#FFF",
    borderRadius: 10,
    borderColor: "#C0C0C0",
    borderWidth: 1,
  },
});
