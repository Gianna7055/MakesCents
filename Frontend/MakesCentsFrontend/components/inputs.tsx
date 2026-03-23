import { Colors } from "@/constants/theme";
import { screenWidth } from "@/css/globalStyles";
import React, { forwardRef } from "react";
import { View, Text, TextInput, StyleSheet } from "react-native";

type InputProps = {
  name: string;
  placeholder: string;
  type: "text" | "password";
  value: string;
  onChangeText: (text: string) => void;
  autoCapitalize?: "none" | "sentences" | "words" | "characters";
  returnKeyType?: "next" | "done" | "go" | "search" | "send";
  onSubmitEditing?: () => void;
};

const Input = forwardRef<TextInput, InputProps>((props, ref) => {
  /* Logic */
  // Determine if this is a password field
  const isPassword = props.type === "password";

  return (
    <View style={styles.inputContainer}>
      <Text style={styles.inputHeader}>{props.name}</Text>
      <TextInput
        style={styles.input}
        placeholder={props.placeholder}
        secureTextEntry={isPassword}
        value={props.value}
        onChangeText={props.onChangeText}
        autoCapitalize={props.autoCapitalize ?? "sentences"}
        returnKeyType={props.returnKeyType ?? "done"}
        onSubmitEditing={props.onSubmitEditing}
        ref={ref}
      />
    </View>
  );
});

export default Input;

const styles = StyleSheet.create({
  inputContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingTop: screenWidth * 0.05,
    backgroundColor: Colors.light.background,
  },
  inputHeader: {
    fontSize: 14,
    fontFamily: "Inter",
    color: "#000",
    paddingLeft: screenWidth * 0.01,
    fontWeight: "semibold",
    paddingBottom: 5,
  },
  input: {
    paddingVertical: 15,
    paddingLeft: screenWidth * 0.02,
    backgroundColor: "#FFF",
    borderRadius: 10,
    borderColor: "#C0C0C0",
    borderWidth: 1,
  },
});
