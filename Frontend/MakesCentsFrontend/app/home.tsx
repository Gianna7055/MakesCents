import React from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles, safePadding } from "../css/styles";
import BottomNavBar from "../components/bottom-nav-bar";
import { ScrollView, Text } from "react-native";
import { useRouter } from "expo-router";
import { Button } from "../components/buttons";

export default function Home() {
  // Create a router
  const router = useRouter();
  const insets = useSafeAreaInsets();

  const logout = () => {
    // Go back to the login page
    router.replace("/login-register/login");
  };
  return (
    <SafeAreaView style={globalStyles.Screen}>
      <ScrollView contentContainerStyle={safePadding(insets)}>
        <Text>Home Screen</Text>
        <Button name="Temp" onPress={logout} />
      </ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}
