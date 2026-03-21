import React from "react";
import {
  View,
  Text,
  KeyboardAvoidingView,
  Platform,
  TextInput,
  StyleSheet,
  Dimensions,
} from "react-native";

type InputProps = {
  name: string;
  placeholder: string;
  type: "text" | "password";
  value: string;
  onChangeText: (text: string) => void;
  autoCapitalize?: "none" | "sentences" | "words" | "characters";
};

const Input = (props: InputProps) => {
  /* Logic */
  // Determine if this is a password field
  const isPassword = props.type === "password";

  return (
    <View style={styles.inputContainer}>
      <Text style={styles.inputHeader}>{props.name}</Text>
      <KeyboardAvoidingView
        behavior={Platform.OS === "ios" ? "padding" : "height"}
      >
        <TextInput
          style={styles.input}
          placeholder={props.placeholder}
          secureTextEntry={isPassword}
          value={props.value}
          onChangeText={props.onChangeText}
          autoCapitalize={props.autoCapitalize ?? "sentences"}
        />
      </KeyboardAvoidingView>
    </View>
  );
};

export default Input;

const screenWidth = Dimensions.get("window").width;

const styles = StyleSheet.create({
  inputContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingTop: screenWidth * 0.05,
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
