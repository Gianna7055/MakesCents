import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import { useRouter, usePathname } from "expo-router";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { IconSymbol } from "./ui/icon-symbol";
import { Colors } from "../constants/theme";

const NAV_ITEMS = [
  { label: "Home", path: "/home", icon: "house" as const, iconSet: "material" as const },
  { label: "Budget", path: "/budget", icon: "banknote" as const, iconSet: "fontAwesome5" as const },
  { label: "Transactions", path: "/transactions", icon: "wallet.bifold" as const, iconSet: "fontAwesome5" as const },
  { label: "Accounts", path: "/accounts", icon: "buildings.columns.fill" as const, iconSet: "fontAwesome5" as const },
  { label: "Profile", path: "/profile", icon: "person.crop.circle.fill" as const, iconSet: "fontAwesome5" as const },
];

export default function BottomNavBar() {
  const router = useRouter();
  const pathname = usePathname();
  const { bottom } = useSafeAreaInsets();

  const isActive = (path: string) => {
    const segment = path.split("/").pop() ?? "";
    return pathname.includes(segment);
  };

  return (
    <View style={[styles.container, { paddingBottom: bottom || 10 }]}>
      {NAV_ITEMS.map((item) => {
        const active = isActive(item.path);
        const color = active ? Colors.light.tabIconSelected : Colors.light.tabIconDefault;
        return (
          <TouchableOpacity
            key={item.path}
            style={styles.tab}
            onPress={() => router.replace(item.path as any)}
            activeOpacity={0.7}
          >
            <IconSymbol name={item.icon} size={28} color={color} iconSet={item.iconSet} />
            <Text style={[styles.label, { color }]}>{item.label}</Text>
          </TouchableOpacity>
        );
      })}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    borderTopWidth: 1,
    borderTopColor: "#e0e0e0",
    backgroundColor: "#fff",
    paddingBottom: 10,
    paddingTop: 10,
  },
  tab: {
    flex: 1,
    alignItems: "center",
    gap: 4,
  },
  label: {
    fontSize: 11,
  },
});