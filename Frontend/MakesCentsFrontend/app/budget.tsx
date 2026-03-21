import React from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles, safePadding } from "../css/globalStyles";
import BottomNavBar from "../components/bottom-nav-bar";
import { ScrollView, Text } from "react-native";

export default function Budget() {
  const insets = useSafeAreaInsets();
  return (
    <SafeAreaView style={globalStyles.Screen}>
      <ScrollView contentContainerStyle={safePadding(insets)}>
        <Text style={globalStyles.Title}>Budget</Text>
      </ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}
