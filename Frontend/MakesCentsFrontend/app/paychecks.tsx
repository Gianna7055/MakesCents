import BottomNavBar from "@/components/bottom-nav-bar";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import React from "react";
import { ScrollView, View, Image, Text } from "react-native";

export default function Paychecks() {
  return (
    <ScreenWrapper>
      {/* Logo and Title */}
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <View style={globalStyles.logoTitleContainer}>
          <Text
            style={globalStyles.logoTitle}
            numberOfLines={2}
            ellipsizeMode="tail"
          >
            Paychecks
          </Text>
        </View>
      </View>

      <ScrollView
        style={{ marginVertical: 0 }}
        contentContainerStyle={{ paddingBottom: 75 }}
      ></ScrollView>
      <BottomNavBar />
    </ScreenWrapper>
  );
}
