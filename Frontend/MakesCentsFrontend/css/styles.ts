import { StyleSheet } from "react-native";
import { EdgeInsets } from "react-native-safe-area-context";


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
});


// Helper to get dynamic padding using insets
export const safePadding = (insets: EdgeInsets) => ({
  paddingTop: insets.top,
  paddingBottom: insets.bottom,
  paddingHorizontal: (insets.top / 1.5)
});