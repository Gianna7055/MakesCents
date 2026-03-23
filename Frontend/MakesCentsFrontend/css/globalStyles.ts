import { Colors } from "@/constants/theme";
import { Dimensions, StyleSheet } from "react-native";

export const screenWidth = Dimensions.get('window').width;
export const screenHeight = Dimensions.get('window').height;

export const globalStyles = StyleSheet.create({
    screen: {
        backgroundColor: Colors.light.background,
        fontFamily: 'Inter',
        flex: 1,
        position: "absolute",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
    },
    centeredTitle: {
        fontFamily: 'Roboto',
        fontSize: 24,
        textAlign: "center",
    },
    // Logo styles
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
        borderColor: "purple",
        flexDirection: "row",
        //borderWidth: 1, // For Testing
    },
    inLineLogo: {
        width: screenWidth * 0.6, // For InLine
        height: screenWidth * 0.15,
        resizeMode: "contain",
        borderColor: "purple",
        marginLeft: screenWidth * 0.01, // For InLine
        marginBottom: screenHeight * 0.01,
        marginTop: screenHeight * 0.0,
        //borderWidth: 1, // For Testing
    },
    noWordsLogoContainer: {
        flexDirection: "row",
        alignItems: "center",
        marginTop: screenHeight * 0.01,
        borderColor: "purple",
        //borderWidth: 1, // For Testing
    },
    noWordsLogo: {
        width: screenWidth * .15, // For Logo
        height: screenWidth * 0.15,
        resizeMode: "contain",
        borderColor: "purple",
        marginLeft: screenWidth * 0.05, // For Logo
        marginBottom: screenHeight * 0.02,
        marginTop: screenHeight * 0.01,
        flex: 0
        //borderWidth: 1, // For Testing
    },
    logoTitle: {
        fontFamily: 'Roboto',
        fontSize: 24,
        textAlign: "center",
              position: "absolute",
              left: 0,
              right: 0,
              alignItems: "center",
    },
    // Text styles
    negativeAmount: {
      color: "#B5362F",
      textAlign: "right",
    },
    zeroAmount: {
      color: "#000000",
      textAlign: "right",
    },
    positiveAmount: {
      color: "#32C54B",
      textAlign: "right",
    },
});

export const formatDate = (date: any): string => {
    const dateString = date as string;
    const [year, month, day] = dateString.split('-').map(Number);
    return `${month}/${day}`;
  };