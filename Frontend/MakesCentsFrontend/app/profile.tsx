import React from "react";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text, Image, View } from "react-native";
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
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <Text style={globalStyles.logoTitle}>Profile</Text>
      </View>
      <ScrollView style={{ marginVertical: 0 }}></ScrollView>
      <BottomNavBar />
    </ScreenWrapper>
  );
}
