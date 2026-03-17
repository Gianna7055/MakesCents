import React from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import { globalStyles } from "../../css/styles";
import { Button } from "../../components/buttons";
import { useRouter } from "expo-router";


export default function Login() {
// Get the router object
  const router = useRouter();

  // Functions to handle back and next button clicks
  const handleLoginClick = () => {
    // Add nav here
    router.replace("/home");
  };

    return (
        <SafeAreaView style={globalStyles.Screen}>
            <Button name="Login" onPress={handleLoginClick} />
        </SafeAreaView>
    );
}