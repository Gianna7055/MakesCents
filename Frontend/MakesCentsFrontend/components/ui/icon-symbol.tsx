// IconSymbol.tsx
import MaterialIcons from "@expo/vector-icons/MaterialIcons";
import FontAwesome5 from "@expo/vector-icons/FontAwesome5";
import { OpaqueColorValue, type StyleProp, type TextStyle } from "react-native";
import React from "react";

/**
 * Mapping SF Symbols to Android/Web icon equivalents.npm install @expo/vector-icons
 */
const MATERIALS_MAPPING = {
  house: "home",
} as const;

const FONT_AWESOME_5_MAPPING = {
  banknote: "money-bill",
  "wallet.bifold": "wallet",
  "buildings.columns.fill": "building-columns", // solid
  "person.crop.circle.fill": "circle-user",     // solid
} as const;

// Define mapping types
type MaterialsIconMapping = typeof MATERIALS_MAPPING;
type FontAwesome5IconMapping = typeof FONT_AWESOME_5_MAPPING;

type IconSymbolName = keyof MaterialsIconMapping | keyof FontAwesome5IconMapping;

/**
 * IconSymbol component: uses SF Symbols on iOS, mapped icons on Android/Web
 */
export function IconSymbol({
  name,
  size = 24,
  color,
  style,
  iconSet,
}: {
  name: IconSymbolName;
  size?: number;
  color: string | OpaqueColorValue;
  style?: StyleProp<TextStyle>;
  iconSet?: "material" | "fontAwesome5";
}) {
  const resolvedIconSet =
    iconSet ?? (name in FONT_AWESOME_5_MAPPING ? "fontAwesome5" : "material");

  if (resolvedIconSet === "material" && name in MATERIALS_MAPPING) {
    return (
      <MaterialIcons
        color={color}
        size={size}
        name={MATERIALS_MAPPING[name as keyof MaterialsIconMapping]}
        style={style}
      />
    );
  } else if (resolvedIconSet === "fontAwesome5" && name in FONT_AWESOME_5_MAPPING) {
    return (
      <FontAwesome5
        color={color}
        size={size}
        name={FONT_AWESOME_5_MAPPING[name as keyof FontAwesome5IconMapping]}
        style={style}
      />
    );
  } else {
    // fallback
    return <MaterialIcons name="help-outline" size={size} color={color} />;
  }
}