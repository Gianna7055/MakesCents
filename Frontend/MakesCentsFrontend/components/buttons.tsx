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
    styles.buttonWrapper,
    variant === "primary"
      ? styles.primaryButtonWrapper
      : styles.secondaryButtonWrapper,
    style,
  ];

  const buttonTextStyle = [
    styles.buttonText,
    variant === "primary"
      ? styles.primaryButtonText
      : styles.secondaryButtonText,
    textStyle,
  ];
  return (
    <View style={[styles.buttonContainer, containerStyle]}>
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
  buttonContainer: {
    paddingTop: 15,
    alignItems: "center",
  },
  buttonWrapper: {
    borderRadius: 10,
    justifyContent: "center",
    alignItems: "center",
    height: 50,
    paddingHorizontal: 15,
  },
  buttonText: {
    fontSize: 15,
    fontWeight: "bold",
  },
  // Primary button styles
  primaryButtonWrapper: {
    backgroundColor: "#088940",
  },
  primaryButtonText: {
    color: "#FFF",
  },
  // Secondary button styles
  secondaryButtonWrapper: {
    backgroundColor: "#B0E3BF",
  },
  secondaryButtonText: {
    color: "#000",
  },
});
