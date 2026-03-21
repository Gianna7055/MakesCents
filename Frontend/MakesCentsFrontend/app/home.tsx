import React from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles, safePadding } from "../css/styles";
import BottomNavBar from "../components/bottom-nav-bar";
import { ScrollView, Image, Dimensions, StyleSheet } from "react-native";
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
        <Image
          source={require("../assets/images/MakesCentsVertLogo.png")}
          style={styles.logo}
        />
      </ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}

// Get screen width once
const screenWidth = Dimensions.get("window").width;
const screenHeight = Dimensions.get("window").height;

const styles = StyleSheet.create({
  logoContainer: {
    alignItems: "center",
    marginTop: screenHeight * 0.02,
  },
  logo: {
    width: screenWidth * 0.8, // 60% of screen width
    height: screenWidth * 0.8 * 0.5, // maintain aspect ratio ~2:1
    resizeMode: "contain",
  },
  subtext: {
    textAlign: "center",
    paddingTop: screenHeight * 0.02,
  },
});
