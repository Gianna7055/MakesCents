import React, { useState } from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import { globalStyles } from "../../css/styles";
import { Button } from "../../components/buttons";
import { useRouter } from "expo-router";
import {
  Dimensions,
  Image,
  StyleSheet,
  View,
  Text,
  KeyboardAvoidingView,
} from "react-native";
import Input from "../../components/inputs";
import { LoginRequest } from "../../types/login-request";
import { LoginResponse } from "../../types/login-response";

export default function Register() {
  // Get the router object
  const router = useRouter();
  const [usernameOrEmail, setUsernameOrEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  // Functions to handle button clicks
  const handleLoginClick = () => {
    // Check if the username/email or password is blank
    if (!usernameOrEmail || !password) {
      console.log("Missing Username/Email or Password");
      /* 
      --------------------------------------------------------------------------------------------
        DEAL WITH MISSING USERNAME/EMAIL OR PASSWORD
      --------------------------------------------------------------------------------------------
      */
    }
    // Create the login request
    const request = new LoginRequest();
    request.usernameOrEmail = usernameOrEmail;
    request.password = password;

    /*const response: LoginResponse = await fetch("/api/user/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });*/
  };
  const handleRegisterClick = () => {
    // Add nav here
    router.replace("/login-register/register");
  };

  return (
    <SafeAreaView style={globalStyles.Screen}>
      <View style={styles.logoContainer}>
        <Image
          source={require("../../assets/images/MakesCentsHorizLogo.png")}
          style={styles.logo}
        />
      </View>
      <Text style={globalStyles.Title}>Login</Text>
      <Input
        name="Username/Email"
        placeholder="Value"
        type="text"
        value={usernameOrEmail}
        onChangeText={setUsernameOrEmail}
      ></Input>
      <Input
        name="Password"
        placeholder="Value"
        type="password"
        value={password}
        onChangeText={setPassword}
      ></Input>
      <Button name="Login" onPress={handleLoginClick} />
      <Text style={styles.subtext}>Don't have an account?</Text>
      <Button
        name="Register"
        variant="secondary"
        onPress={handleRegisterClick}
      />
    </SafeAreaView>
  );
}

// Get screen width once
const screenWidth = Dimensions.get("window").width;
const screenHeight = Dimensions.get("window").height;

const styles = StyleSheet.create({
  logoContainer: {
    alignItems: "center",
    marginTop: screenHeight * 0.02,
  },
  logo: {
    width: screenWidth * 0.8, // 60% of screen width
    height: screenWidth * 0.8 * 0.5, // maintain aspect ratio ~2:1
    resizeMode: "contain",
  },
  subtext: {
    textAlign: "center",
    paddingTop: screenHeight * 0.02,
  },
});
