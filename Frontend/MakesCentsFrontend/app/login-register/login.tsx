import React, { useRef, useState } from "react";
import { globalStyles, screenHeight } from "@/css/globalStyles";
import { Button } from "@/components/buttons";
import { router } from "expo-router";
import {
  Image,
  StyleSheet,
  View,
  Text,
  KeyboardAvoidingView,
  TextInput,
  ScrollView,
} from "react-native";
import Input from "@/components/inputs";
import { LoginRequest } from "@/types/login-request";
import { LoginResponse } from "@/types/login-response";
import { makesCentsPublicAxios } from "@/data/datasource";
import { AxiosResponse } from "axios";
import { storage } from "@/data/storage";
import ScreenWrapper from "@/components/ui/screen-wrapper";

export default function Login() {
  // UseState variables
  const [usernameOrEmail, setUsernameOrEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  // references for text inputs
  const passwordRef = useRef<TextInput>(null);

  // Functions to handle button clicks
  async function handleLoginClick() {
    // Log the username/email and password
    //console.log("Username/Email:", usernameOrEmail);
    //console.log("Password:", password);

    // Check if the username/email or password is blank
    if (!usernameOrEmail || !password) {
      console.log("Missing Username/Email or Password");
      /* 
      --------------------------------------------------------------------------------------------
        DEAL WITH MISSING USERNAME/EMAIL OR PASSWORD
      --------------------------------------------------------------------------------------------
      */
    } else {
      // Create the login request
      const request = new LoginRequest();
      request.usernameOrEmail = usernameOrEmail;
      request.password = password;

      // Log the request
      //console.log("Request:", request);

      // Set up a try-catch to ensure safe-failure
      try {
        // Call the API
        const axiosResponse: AxiosResponse = await makesCentsPublicAxios.post(
          "/api/user/login",
          request,
        );

        // Get the response
        const response: LoginResponse = axiosResponse.data;

        // Log the response
        //console.log("Response:", response);

        // Check the response code
        if (response.httpStatus == 200) {
          // Save the token from the response
          storage.saveToken(response.token);
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
      } catch (error: any) {
        console.log("Error:", error);
        console.log("Status:", error.response?.status);
        console.log("Response:", error.response?.data);
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
    <ScreenWrapper>
      <ScrollView
        contentContainerStyle={{ flexGrow: 1, paddingBottom: 20 }}
        keyboardShouldPersistTaps="handled"
      >
        <View style={globalStyles.horizLogoContainer}>
          <Image
            source={require("@/assets/images/MakesCentsHorizLogo.png")}
            style={globalStyles.horizLogo}
          />
        </View>
        <Text style={globalStyles.centeredTitle}>Login</Text>
        <KeyboardAvoidingView>
          <Input
            name="Username/Email"
            placeholder="Value"
            type="text"
            value={usernameOrEmail}
            onChangeText={setUsernameOrEmail}
            autoCapitalize="none"
            returnKeyType="next"
            onSubmitEditing={() => passwordRef.current?.focus()}
          ></Input>
          <Input
            name="Password"
            placeholder="Value"
            type="password"
            value={password}
            onChangeText={setPassword}
            autoCapitalize="none"
            returnKeyType="done"
            onSubmitEditing={handleLoginClick}
            ref={passwordRef}
          ></Input>
        </KeyboardAvoidingView>
        <Button name="Login" onPress={handleLoginClick} />
        <Text style={styles.subtext}>Don't have an account?</Text>
        <Button
          name="Register"
          variant="secondary"
          onPress={handleRegisterClick}
        />
      </ScrollView>
    </ScreenWrapper>
  );
}

// Get screen width once

const styles = StyleSheet.create({
  subtext: {
    textAlign: "center",
    paddingTop: screenHeight * 0.02,
  },
});
