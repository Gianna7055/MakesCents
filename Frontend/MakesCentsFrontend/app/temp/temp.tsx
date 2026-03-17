// MyScreen.tsx
import React from "react";
import { View, ScrollView, Text, StyleSheet } from "react-native";
import {
  SafeAreaView,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { globalStyles } from "../../css/styles"; // your global styles file

export default function Temp() {
  const insets = useSafeAreaInsets();

  return (
    <View style={styles.container}>
      {/* Full-screen background */}
      <View style={styles.background} />

      {/* Safe area scrollable content */}
      <SafeAreaView style={{ flex: 1 }}>
        <ScrollView
          contentContainerStyle={{
            paddingTop: insets.top,
            paddingBottom: insets.bottom,
            paddingHorizontal: 16, // optional side padding
          }}
        >
          <Text>This is your safe content</Text>
          <Text>More content below</Text>
        </ScrollView>
      </SafeAreaView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  background: {
    position: "absolute",
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: "lightgreen", // your screen color
  },
});
