import { Colors } from "@/constants/theme";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";

type Option = {
  label: string;
  value: string;
};

type RadioInputProps = {
  name: string;
  options: Option[];
  value: string;
  onChange: (value: string) => void;
  containerStyle?: any;
};

const RadioInput = (props: RadioInputProps) => {
  return (
    <View style={styles.inputContainer}>
      <Text style={globalStyles.textHeader}>{props.name}</Text>

      <View style={[styles.optionsRow, props.containerStyle]}>
        {props.options.map((option) => {
          const isSelected = props.value === option.value;

          return (
            <TouchableOpacity
              key={option.value}
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
    </View>
  );
};

export default RadioInput;

const styles = StyleSheet.create({
  inputContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingTop: screenWidth * 0.05,
  },

  optionsRow: {
    flexDirection: "row",
    justifyContent: "center", // centers the buttons horizontally
    marginTop: 10,
    gap: 30,
    alignItems: "center",
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
    borderColor: "#888888",
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
    fontSize: 16,
  },
});
