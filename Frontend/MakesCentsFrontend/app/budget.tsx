import React, { useEffect } from "react";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles } from "@/css/globalStyles";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text, Image, View, StyleSheet } from "react-native";

export default function Budget() {
  const insets = useSafeAreaInsets();

  useEffect(() => {
    const main = async () => {};

    // Main method
    main();
  }, []);

  return (
    <SafeAreaView style={globalStyles.Screen}>
      <View style={styles.budgetStatusContainer}>
        <Text>Your Budget Makes Cents</Text>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={styles.logo}
        />
      </View>
      <Text style={globalStyles.Title}>Budget</Text>
      <ScrollView></ScrollView>
      <BottomNavBar />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  budgetStatusContainer: {
    backgroundColor: "#FFF",
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 5,
  },
  logo: {
    width: 30,
    height: 30,
    marginVertical: 4,
  },
});
