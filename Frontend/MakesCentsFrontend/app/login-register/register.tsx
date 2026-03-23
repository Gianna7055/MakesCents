import React, { useState } from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import { globalStyles, screenHeight } from "@/css/globalStyles";
import { Button } from "@/components/buttons";
import { useRouter } from "expo-router";
import { Image, StyleSheet, View, Text } from "react-native";
import Input from "@/components/inputs";
import { RegisterRequest } from "@/types/register-request";
import { RegisterResponse } from "@/types/register-response";
import { makesCentsPublicAxios } from "@/data/datasource";

import { AxiosResponse } from "axios";
import { storage } from "@/data/storage";

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
        DEAL WITH MISSING USERNAME, EMAIL, OR PASSWORD
      --------------------------------------------------------------------------------------------
      */
    } else {
      // Create the register request
      const request = new RegisterRequest();
      request.username = username;
      request.email = email;
      request.passwordHash = password;

      // Set up a try-catch to ensure safe-failure
      try {
        // Call the API
        const axiosResponse: AxiosResponse = await makesCentsPublicAxios.post(
          "/api/user/register",
          request,
        );

        // Get the response
        const response: RegisterResponse = axiosResponse.data;

        // Check the response code
        if (response.httpStatus == 201) {
          storage.saveToken(response.token);
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
      } catch (error: any) {
        console.log("Status:", error.response?.status);
        console.log("Response:", error.response?.data);
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
    <SafeAreaView style={globalStyles.screen}>
      <View style={globalStyles.horizLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsHorizLogo.png")}
          style={globalStyles.horizLogo}
        />
      </View>
      <Text style={globalStyles.centeredTitle}>Login</Text>
      <Input
        name="Username"
        placeholder="Value"
        type="text"
        value={username}
        onChangeText={setUsername}
        autoCapitalize="none"
      ></Input>
      <Input
        name="Email"
        placeholder="Value"
        type="text"
        value={email}
        onChangeText={setEmail}
        autoCapitalize="none"
      ></Input>
      <Input
        name="Password"
        placeholder="Value"
        type="password"
        value={password}
        onChangeText={setPassword}
        autoCapitalize="none"
      ></Input>
      <Button name="Register" onPress={handleRegisterClick} />
      <Text style={styles.subtext}>Already have an account?</Text>
      <Button name="Login" variant="secondary" onPress={handleLoginClick} />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  subtext: {
    textAlign: "center",
    paddingTop: screenHeight * 0.02,
  },
});
