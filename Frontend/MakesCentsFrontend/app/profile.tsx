import React from "react";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text } from "react-native";
import { Button } from "@/components/buttons";
import { router } from "expo-router";
import { globalStyles } from "@/css/globalStyles";
import ScreenWrapper from "@/components/ui/screen-wrapper";

export default function Profile() {
  // Create a router

  const logout = () => {
    // Go back to the login page
    router.replace("/login-register/login");
  };
  return (
    <ScreenWrapper>
      <ScrollView>
        <Text style={globalStyles.centeredTitle}>Profile Screen</Text>
        <Button name="Log Out" onPress={logout} />
      </ScrollView>
      <BottomNavBar />
    </ScreenWrapper>
  );
}
