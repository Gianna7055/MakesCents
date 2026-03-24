import React from "react";
import { TouchableOpacity, StyleSheet, View } from "react-native";
import MaterialIcons from "@expo/vector-icons/MaterialIcons";
import { IconSymbol, IconSymbolName } from "../ui/icon-symbol";
import { Colors } from "@/constants/theme";

type IconButtonProps = {
  icon: IconSymbolName;
  iconSet: "material" | "fontAwesome5";
  onPress: () => void;
  size?: number;
  color?: string;
  style?: any;
};

export default function IconButton({
  icon,
  iconSet,
  onPress,
  size = 30,
  color = "white",
  style,
}: IconButtonProps) {
  // Variables
  return (
    <TouchableOpacity onPress={onPress} style={[styles.container, style]}>
      <IconSymbol name={icon} size={size} color={color} iconSet={iconSet} />
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    padding: 8,
    backgroundColor: Colors.light.primary,
    borderRadius: 50,
    height: 50,
    width: 50,
    alignItems: "center",
    justifyContent: "center",
  },
});
