import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";

type ButtonProps = {
  name: string;
  onPress: () => void;
  variant?: "primary" | "secondary";
  style?: any;
  textStyle?: any;
  containerStyle?: any;
};

const Button = ({
  name,
  onPress,
  variant = "primary",
  style,
  textStyle,
  containerStyle,
}: ButtonProps) => {
  const buttonStyle = [
    styles.ButtonWrapper,
    variant === "primary"
      ? styles.PrimaryButtonWrapper
      : styles.SecondaryButtonWrapper,
    style,
  ];

  const buttonTextStyle = [
    styles.ButtonText,
    variant === "primary"
      ? styles.PrimaryButtonText
      : styles.SecondaryButtonText,
    textStyle,
  ];
  return (
    <View style={[styles.ButtonContainer, containerStyle]}>
      <TouchableOpacity onPress={onPress}>
        <View style={[buttonStyle, style]}>
          <Text style={[buttonTextStyle, textStyle]}>{name}</Text>
        </View>
      </TouchableOpacity>
    </View>
  );
};

export { Button };

const styles = StyleSheet.create({
  // Base button styles
  ButtonContainer: {
    paddingTop: 15,
    alignItems: "center",
  },
  ButtonWrapper: {
    borderRadius: 10,
    justifyContent: "center",
    alignItems: "center",
    height: 50,
    paddingHorizontal: 15,
  },
  ButtonText: {
    fontSize: 15,
    fontWeight: "bold",
  },
  // Primary button styles
  PrimaryButtonWrapper: {
    backgroundColor: "#088940",
  },
  PrimaryButtonText: {
    color: "#FFF",
  },
  // Secondary button styles
  SecondaryButtonWrapper: {
    backgroundColor: "#B0E3BF",
  },
  SecondaryButtonText: {
    color: "#000",
  },
});
