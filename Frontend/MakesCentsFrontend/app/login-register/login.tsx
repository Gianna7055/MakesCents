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
import { makesCentsUrl } from "../../data/datasource";
import { tokenStorage } from "../../data/tokenStorage";

export default function Login() {
  // Get the router object
  const router = useRouter();
  const [usernameOrEmail, setUsernameOrEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  // Functions to handle button clicks
  async function handleLoginClick() {
    console.log("In login click EH");
    // Check if the username/email or password is blank
    if (!usernameOrEmail || !password) {
      console.log("Missing Username/Email or Password");
      /* 
      --------------------------------------------------------------------------------------------
        DEAL WITH MISSING USERNAME/EMAIL OR PASSWORD
      --------------------------------------------------------------------------------------------
      */
    } else {
      console.log("Username/email:", usernameOrEmail);
      console.log("Password:", password);
      // Create the login request
      const request = new LoginRequest();
      request.usernameOrEmail = usernameOrEmail;
      request.password = password;

      console.log("Request:", request);

      // Call the API
      const rawResponse: Response = await fetch(
        makesCentsUrl + "/api/user/login",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(request),
        },
      );
      console.log("Fetch complete");

      console.log("Status:", rawResponse.status);
      console.log("Response:", rawResponse.body);

      console.log("Fetch complete");

      const text = await rawResponse.text();
      console.log("Status:", rawResponse.status);
      console.log("Raw response:", text);

      // Get the response from the raw response
      const response: LoginResponse = JSON.parse(text);
      console.log("Response:", response);

      // Check the response code
      if (response.httpStatus == 200) {
        // Save the token from the response
        tokenStorage.saveToken(response.token);
        console.log("ID token from response:", response.token);

        // Redirect to the home page
        router.replace("/home");
      } else {
        console.log("Login failed");
        /* 
      --------------------------------------------------------------------------------------------
        DEAL WITH LOGIN FAIL
      --------------------------------------------------------------------------------------------
      */
      }
    }
  }
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
