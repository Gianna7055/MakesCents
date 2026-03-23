import React from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text } from "react-native";
import { Button } from "@/components/buttons";
import { router } from "expo-router";
import { globalStyles } from "@/css/globalStyles";

export default function Profile() {
  // Create a router

  const logout = () => {
    // Go back to the login page
    router.replace("/login-register/login");
  };
  return (
    <SafeAreaView style={globalStyles.screen}>
      <ScrollView>
        <Text>Profile Screen</Text>
        <Button name="Log Out" onPress={logout} />
      </ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}
