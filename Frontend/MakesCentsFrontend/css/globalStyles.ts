import { Colors } from "@/constants/theme";
import { Dimensions, StyleSheet } from "react-native";

export const screenWidth = Dimensions.get("window").width;
export const screenHeight = Dimensions.get("window").height;

export const globalStyles = StyleSheet.create({
  screen: {
    backgroundColor: Colors.light.background,
    fontFamily: "Inter",
    flex: 1,
  },
  centeredTitle: {
    fontFamily: "Roboto",
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
    width: screenWidth * 0.6,
    height: screenWidth * 0.15,
    resizeMode: "contain",
    borderColor: "purple",
    marginLeft: screenWidth * 0.01,
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
    width: screenWidth * 0.15,
    height: screenWidth * 0.15,
    resizeMode: "contain",
    borderColor: "purple",
    marginLeft: screenWidth * 0.05,
    marginBottom: screenHeight * 0.01,
    flex: 0,
    //borderWidth: 1, // For Testing
  },
  logoTitleContainer: {
    position: "absolute",
    left: 0,
    right: 0,
    alignItems: "center",
  },
  logoTitle: {
    fontFamily: "Roboto",
    fontSize: 24,
    textAlign: "center",
    left: 0,
    right: 0,
    alignItems: "center",
    position: "relative",
    maxWidth: screenWidth * 0.58,
  },
  // Text styles
  redAmount: {
    color: "#B5362F",
    textAlign: "right",
  },
  blackAmount: {
    color: "#000000",
    textAlign: "right",
  },
  greenAmount: {
    color: "#32C54B",
    textAlign: "right",
  },
  textHeader: {
    fontSize: 14,
    fontFamily: "Inter",
    color: "#000",
    paddingLeft: screenWidth * 0.01,
    fontWeight: "semibold",
    paddingBottom: 5,
  },

  // Bottom split button style
  bottomButtons: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    position: "absolute",
    bottom: 67,
    left: 0,
    right: 0,
    padding: 20,
  },

  // Settings page styles
  settingsContainer: {
    backgroundColor: "#C9DAD0",
    borderWidth: 1,
    width: screenWidth * 0.9,
    alignSelf: "center",
    borderRadius: 20,
    paddingBottom: 15,
    marginTop: 10,
  },
  settingsTitle: {
    paddingTop: 10,
    fontSize: 20,
    textAlign: "center",
  },

  // Empty list styles
  emptyListContainer: {
    flex: 1,
    alignItems: "center",
    gap: 15,
  },
  emptyListText: {
    fontSize: 16,
    color: "#555",
  },
});
