import { Colors } from "@/constants/theme";
import { SummaryAccountDTOModel } from "@/types/summary-account-dto-model";
import { Dimensions, StyleSheet } from "react-native";

export const screenWidth = Dimensions.get('window').width;
export const screenHeight = Dimensions.get('window').height;

export const globalStyles = StyleSheet.create({
    screen: {
        backgroundColor: Colors.light.background,
        fontFamily: 'Inter',
        flex: 1,
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
        width: screenWidth * .15,
        height: screenWidth * 0.15,
        resizeMode: "contain",
        borderColor: "purple",
        marginLeft: screenWidth * 0.05,
        marginBottom: screenHeight * 0.01,
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

    
    textHeader: {
      fontSize: 14,
      fontFamily: "Inter",
      color: "#000",
      paddingLeft: screenWidth * 0.01,
      fontWeight: "semibold",
      paddingBottom: 5,
    },
});

export const formatDate = (date: any): string => {
    const dateString = date as string;
    const [year, month, day] = dateString.split('-').map(Number);
    return `${month}/${day}`;
  };

export const formatEnum = (value: string): string => {
return value.replace(/([A-Z])/g, ' $1').trim();
};

export const getAccountSubType = (account: SummaryAccountDTOModel): string => {
  if (account.bankAccountType)
    return formatEnum(account.bankAccountType.toString());
  if (account.debtAccountType)
    return formatEnum(account.debtAccountType.toString());
  if (account.investmentAccountType) {
    if (account.investmentAccountType.toString() == "IRA") return "IRA";
    if (account.investmentAccountType.toString() == "Retirement401K403B")
      return "401K / 403B";
    return formatEnum(account.investmentAccountType.toString());
  }
  return "";
};
