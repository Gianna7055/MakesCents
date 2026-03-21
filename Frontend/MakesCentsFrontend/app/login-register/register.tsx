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
import { RegisterRequest } from "../../types/register-request";
import { RegisterResponse } from "../../types/register-response";
import { makesCentsUrl } from "../../data/datasource";
import { tokenStorage } from "../../data/tokenStorage";

export default function Register() {
  // Get the router object
  const router = useRouter();
  const [username, setUsername] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  // Functions to handle button clicks
  async function handleRegisterClick() {
    // Check if the username, email, or password is blank
    if (!username || !email || !password) {
      console.log("Missing username, email, or password");
      /* 
      --------------------------------------------------------------------------------------------
        DEAL WITH MISSING USERNAME/EMAIL OR PASSWORD
      --------------------------------------------------------------------------------------------
      */
    } else {
      // Create the register request
      const request = new RegisterRequest();
      request.username = username;
      request.email = email;
      request.passwordHash = password;

      // Call the API
      const rawResponse: Response = await fetch(
        makesCentsUrl + "/api/user/register",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(request),
        },
      );

      const text = await rawResponse.text();
      console.log("Status:", rawResponse.status);
      console.log("Raw response:", text);

      // Get the response from the raw response
      const response: RegisterResponse = JSON.parse(text);

      // Check the response code
      if (response.httpStatus == 201) {
        // Save the token from the response
        tokenStorage.saveToken(response.token);
        console.log("ID token from response:", response.token);

        // Redirect to the home page
        router.replace("/home");
      } else {
        console.log("Login failed");
        /* 
          --------------------------------------------------------------------------------------------
            DEAL WITH REGISTER FAIL
          --------------------------------------------------------------------------------------------
          */
      }
    }
  }
  const handleLoginClick = () => {
    // Add nav here
    router.replace("/login-register/login");
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
        name="Username"
        placeholder="Value"
        type="text"
        value={username}
        onChangeText={setUsername}
      ></Input>
      <Input
        name="Email"
        placeholder="Value"
        type="text"
        value={email}
        onChangeText={setEmail}
      ></Input>
      <Input
        name="Password"
        placeholder="Value"
        type="password"
        value={password}
        onChangeText={setPassword}
      ></Input>
      <Button name="Register" onPress={handleRegisterClick} />
      <Text style={styles.subtext}>Already have an account?</Text>
      <Button name="Login" variant="secondary" onPress={handleLoginClick} />
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
