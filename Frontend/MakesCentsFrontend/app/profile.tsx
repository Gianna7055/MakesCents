import React from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles, safePadding } from "../css/styles";
import BottomNavBar from "../components/bottom-nav-bar";
import { ScrollView, Text } from "react-native";
import { Button } from "../components/buttons";
import { useRouter } from "expo-router";

export default function Profile() {
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
        <Text>Profile Screen</Text>
        <Button name="Log Out" onPress={logout} />
      </ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}