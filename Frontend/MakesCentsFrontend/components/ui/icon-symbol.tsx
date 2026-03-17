// IconSymbol.tsx
import React from "react";
import { Platform, View, type StyleProp, type TextStyle } from "react-native";
import MaterialIcons from "@expo/vector-icons/MaterialIcons";
import FontAwesome5 from "@expo/vector-icons/FontAwesome5";
import { SymbolView } from "expo-symbols";
import { OpaqueColorValue } from "react-native";

/**
 * Only include the exact SF Symbols names you’re using.
 * This roots the type of `name` so it satisfies SymbolView’s prop.
 */
type SFSymbolNames =
  | "house"
  | "banknote"
  | "wallet.bifold"
  | "building.columns.fill"
  | "person.crop.circle.fill";

const MATERIALS_MAPPING = {
  house: "home",
} as const;

const FONT_AWESOME_5_MAPPING = {
  banknote: "money-bill",
  "wallet.bifold": "wallet",
  "building.columns.fill": "university",
  "person.crop.circle.fill": "user-circle",
} as const;

type MaterialsIconMapping = typeof MATERIALS_MAPPING;
type FontAwesome5IconMapping = typeof FONT_AWESOME_5_MAPPING;

export type IconSymbolName =
  | keyof MaterialsIconMapping
  | keyof FontAwesome5IconMapping;

type Props = {
  name: IconSymbolName;
  size?: number;
  color: string | OpaqueColorValue;
  style?: StyleProp<TextStyle>;
  iconSet?: "material" | "fontAwesome5";
};

export function IconSymbol({ name, size = 24, color, style, iconSet }: Props) {
  if (Platform.OS === "ios") {
    // Here `name` is typed as one of the SF Symbol strings
    return (
      <SymbolView
        name={name as SFSymbolNames} // cast to the literal union
        size={size}
        tintColor={color as string}
      />
    );
  }

  const resolvedIconSet =
    iconSet ?? (name in FONT_AWESOME_5_MAPPING ? "fontAwesome5" : "material");

  if (resolvedIconSet === "material" && name in MATERIALS_MAPPING) {
    return (
      <MaterialIcons
        name={MATERIALS_MAPPING[name as keyof MaterialsIconMapping]}
        size={size}
        color={color}
        style={style}
      />
    );
  } else if (
    resolvedIconSet === "fontAwesome5" &&
    name in FONT_AWESOME_5_MAPPING
  ) {
    return (
      <FontAwesome5
        name={FONT_AWESOME_5_MAPPING[name as keyof FontAwesome5IconMapping]}
        size={size}
        color={color}
        style={style}
        solid
      />
    );
  }

  return <MaterialIcons name="help-outline" size={size} color={color} />;
}
