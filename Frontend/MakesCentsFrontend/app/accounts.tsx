import React from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import { globalStyles } from "../css/styles";
import BottomNavBar from "../components/bottom-nav-bar";


export default function Accounts() {
    return (
        <SafeAreaView style={globalStyles.Screen}>
            <BottomNavBar />
        </SafeAreaView>
    );
}