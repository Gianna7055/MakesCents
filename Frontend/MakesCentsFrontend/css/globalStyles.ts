import { Colors } from "@/constants/theme";
import { Dimensions, StyleSheet } from "react-native";
import { EdgeInsets } from "react-native-safe-area-context";

export const screenWidth = Dimensions.get('window').width;
export const screenHeight = Dimensions.get('window').height;


export const globalStyles = StyleSheet.create({
    Screen: {
        backgroundColor: Colors.light.background,
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
        borderColor: "purple",
        //borderWidth: 1, For Testing
        flexDirection: "row",
        /* For Logo
        flexDirection: "row",
        alignItems: "center",
        //marginTop: screenHeight * 0.01,*/
    },
    inLineLogo: {
        width: screenWidth * 0.6, // For InLine
        //width: screenWidth * .15, // For Logo
        height: screenWidth * 0.15,
        resizeMode: "contain",
        borderColor: "purple",
        marginLeft: screenWidth * 0.03, // For InLine
        //marginLeft: screenWidth * 0.05, // For Logo
        marginBottom: screenHeight * 0.02,
        marginTop: screenHeight * 0.01,
        //borderWidth:, 1 For Testing
    },
});

export const formatDate = (date: any): string => {
    const dateString = date as string;
    const [year, month, day] = dateString.split('-').map(Number);
    return `${month}/${day}`;
  };