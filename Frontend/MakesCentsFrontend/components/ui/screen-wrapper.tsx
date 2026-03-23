// components/screen-wrapper.tsx
import { KeyboardAvoidingView, Platform, StyleSheet } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { globalStyles } from "@/css/globalStyles";
import React from "react";

type ScreenWrapperProps = {
  children: React.ReactNode;
};

export default function ScreenWrapper({ children }: ScreenWrapperProps) {
  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === "ios" ? "padding" : "height"}
      style={{ flex: 1 }}
    >
      <SafeAreaView style={[globalStyles.screen, { flex: 1 }]}>
        {children}
      </SafeAreaView>
    </KeyboardAvoidingView>
  );
}
