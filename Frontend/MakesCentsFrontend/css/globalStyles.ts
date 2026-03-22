import { Dimensions, StyleSheet } from "react-native";
import { EdgeInsets } from "react-native-safe-area-context";

export const screenWidth = Dimensions.get('window').width;
export const screenHeight = Dimensions.get('window').height;

export const globalStyles = StyleSheet.create({
    Screen: {
        backgroundColor: '#DDF0E5',
        fontFamily: 'Inter',
        flex: 1,
        position: "absolute",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
    },
    Title: {
        fontFamily: 'Roboto',
        fontSize: 24,
        textAlign: "center",
    },
    horizLogoContainer: {
        alignItems: "center",
        marginTop: screenHeight * 0.01,
        marginBottom: screenHeight * 0.02,
    },
    horizLogo: {
        width: screenWidth * 0.8, // 60% of screen width
        height: screenWidth * 0.8 * 0.5, // maintain aspect ratio ~2:1
        resizeMode: "contain",
    },
    inLineLogoContainer: {
        alignItems: "flex-start",
        marginTop: screenHeight * 0.01,
        marginBottom: screenHeight * 0.02,
    },
    Logo: {
        width: screenWidth * 0.8, // 60% of screen width
        height: screenWidth * 0.8 * 0.5, // maintain aspect ratio ~2:1
        resizeMode: "contain",
    },
});

// Helper to get dynamic padding using insets
export const safePadding = (insets: EdgeInsets) => ({
  paddingTop: insets.top,
  paddingBottom: insets.bottom,
  paddingHorizontal: (insets.top / 1.5)
});

export const formatDate = (date: any): string => {
    const dateString = date as string;
    const [year, month, day] = dateString.split('-').map(Number);
    return `${month}/${day}`;
  };