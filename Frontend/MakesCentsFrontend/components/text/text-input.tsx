import { Colors } from "@/constants/theme";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import React, { forwardRef } from "react";
import { View, Text, TextInput, StyleSheet } from "react-native";

type InputProps = {
  name: string;
  placeholder: string;
  type: "text" | "password";
  value: string;
  onChangeText: (text: string) => void;
  autoCapitalize?: "none" | "sentences" | "words" | "characters";
  returnKeyType?: "next" | "done" | "go" | "search" | "send" | "default";
  onSubmitEditing?: () => void;
  boxStyle?: any;
  line?: "single" | "multi";
  onBlur?: () => void;
};

const Input = forwardRef<TextInput, InputProps>((props, ref) => {
  /* Logic */
  // Determine if this is a password field
  const isPassword = props.type === "password";
  const isMultiLine = props.line === "multi";

  return (
    <View style={styles.inputContainer}>
      <Text style={globalStyles.textHeader}>{props.name}</Text>
      <TextInput
        style={[styles.input, props.boxStyle]}
        placeholder={props.placeholder}
        secureTextEntry={isPassword}
        value={props.value}
        onChangeText={props.onChangeText}
        onBlur={props.onBlur}
        autoCapitalize={props.autoCapitalize ?? "sentences"}
        returnKeyType={props.returnKeyType ?? "done"}
        onSubmitEditing={props.onSubmitEditing}
        ref={ref}
        textAlignVertical="top"
        multiline={isMultiLine}
      />
    </View>
  );
});

export default Input;

const styles = StyleSheet.create({
  inputContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingTop: screenWidth * 0.05,
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
