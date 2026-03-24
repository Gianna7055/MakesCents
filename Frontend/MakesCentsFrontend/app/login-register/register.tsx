import React, { useRef, useState } from "react";
import { globalStyles, screenHeight } from "@/css/globalStyles";
import { Button } from "@/components/buttons/button";
import { router } from "expo-router";
import {
  Image,
  StyleSheet,
  View,
  Text,
  ScrollView,
  TextInput,
} from "react-native";
import Input from "@/components/text/inputs";
import { RegisterRequest } from "@/types/register-request";
import { RegisterResponse } from "@/types/register-response";
import { makesCentsPublicAxios } from "@/data/datasource";

import { AxiosResponse } from "axios";
import { storage } from "@/data/storage";
import ScreenWrapper from "@/components/ui/screen-wrapper";

export default function Register() {
  // UseState variables
  const [username, setUsername] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  // references for text inputs
  const emailRef = useRef<TextInput>(null);
  const passwordRef = useRef<TextInput>(null);

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
          // Log the token
          //console.log("ID token from response:", response.token);

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
        <Text style={globalStyles.centeredTitle}>Register</Text>
        <Input
          name="Username"
          placeholder="Value"
          type="text"
          value={username}
          onChangeText={setUsername}
          autoCapitalize="none"
          returnKeyType="next"
          onSubmitEditing={() => emailRef.current?.focus()}
        ></Input>
        <Input
          name="Email"
          placeholder="Value"
          type="text"
          value={email}
          onChangeText={setEmail}
          autoCapitalize="none"
          returnKeyType="next"
          onSubmitEditing={() => passwordRef.current?.focus()}
          ref={emailRef}
        ></Input>
        <Input
          name="Password"
          placeholder="Value"
          type="password"
          value={password}
          onChangeText={setPassword}
          autoCapitalize="none"
          returnKeyType="done"
          onSubmitEditing={handleRegisterClick}
          ref={passwordRef}
        ></Input>
        <Button name="Register" onPress={handleRegisterClick} />
        <Text style={styles.subtext}>Already have an account?</Text>
        <Button name="Login" variant="secondary" onPress={handleLoginClick} />
      </ScrollView>
    </ScreenWrapper>
  );
}

const styles = StyleSheet.create({
  subtext: {
    textAlign: "center",
    paddingTop: screenHeight * 0.02,
  },
});
