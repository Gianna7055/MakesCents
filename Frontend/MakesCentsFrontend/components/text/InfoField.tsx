import { Colors } from "@/constants/theme";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import React, { forwardRef } from "react";
import { View, Text, TextInput, StyleSheet } from "react-native";

type InfoFieldProps = {
  name: string;
  value: string;
};

const InfoField = (props: InfoFieldProps) => {
  /* Logic */

  return (
    <View style={styles.infoContainer}>
      <Text style={styles.infoHeader}>{props.name}</Text>
      <Text style={styles.info}>{props.value}</Text>
    </View>
  );
};

export default InfoField;

const styles = StyleSheet.create({
  infoContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingBottom: 10,
  },
  infoHeader: {
    fontSize: 14,
    fontFamily: "Inter",
    color: "#999",
    paddingLeft: screenWidth * 0.01,
    fontWeight: "semibold",
    //paddingBottom: 5,
  },
  info: {
    paddingBottom: 15,
    paddingTop: 5,
    paddingLeft: 15,
    //backgroundColor: "#FFF",
    borderRadius: 10,
    borderColor: "#C0C0C0",
    //borderWidth: 1,
  },
});
