import React, { useEffect } from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles, safePadding } from "../css/globalStyles";
import BottomNavBar from "../components/bottom-nav-bar";
import { ScrollView, Image, StyleSheet, View } from "react-native";
import { useRouter } from "expo-router";

export default function Home() {
  // Create a router
  const router = useRouter();
  const insets = useSafeAreaInsets();

  // Home constructor
  useEffect(() => {});

  const logout = () => {
    // Go back to the login page
    router.replace("/login-register/login");
  };
  return (
    <SafeAreaView style={globalStyles.Screen}>
      <ScrollView contentContainerStyle={safePadding(insets)}>
        <View style={globalStyles.vertLogoContainer}>
          <Image
            source={require("../assets/images/MakesCentsVertLogo.png")}
            style={globalStyles.vertLogo}
          />
        </View>
      </ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({});
