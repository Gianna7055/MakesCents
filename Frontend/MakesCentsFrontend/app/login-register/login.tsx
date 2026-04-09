import React, { useEffect, useRef, useState } from "react";
import { globalStyles, screenHeight } from "@/css/globalStyles";
import { Button } from "@/components/buttons/button";
import { router } from "expo-router";
import {
  Image,
  StyleSheet,
  View,
  Text,
  TextInput,
  ScrollView,
} from "react-native";
import Input from "@/components/text/text-input";
import { LoginRequest } from "@/types/login-request";
import { LoginResponse } from "@/types/login-response";
import { makesCentsPublicAxios } from "@/data/datasource";
import axios, { AxiosResponse } from "axios";
import { storage } from "@/data/storage";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import { handleBlur, touchAll } from "@/utils/touched";

export default function Login() {
  // UseState variables
  const [usernameOrEmail, setUsernameOrEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [touched, setTouched] = useState<{
    usernameOrEmail: boolean;
    password: boolean;
  }>({
    usernameOrEmail: false,
    password: false,
  });
  const [formError, setFormError] = useState<string | null>(null);
  const [submitted, setSubmitted] = useState(false);

  // For testing: Remove
  /*useEffect(() => {
    setUsernameOrEmail("username");
    setPassword("password");
    handleLoginClick();
  });*/

  // references for text inputs
  const passwordRef = useRef<TextInput>(null);

  // Functions to handle button clicks
  const handleLoginClick = async () => {
    // Set that the form has been submitted
    setSubmitted(true);
    // Log the username/email and password
    //console.log("Username/Email:", usernameOrEmail);
    //console.log("Password:", password);

    // Check if the username/email or password is blank
    if (!usernameOrEmail || !password) {
      console.log("Missing Username/Email or Password");
      return;
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
        const response: LoginResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );

        // Log the response
        //console.log("Response:", response);

        // Check the response code
        if (response.httpStatus == 200) {
          // Save the token from the response
          storage.saveToken(response.token);
          // Log the token
          //console.log("ID token from response:", response.token);

          // Redirect to the home page
          router.replace("/home");
        } else {
          console.log("Login failed");
          setFormError("Incorrect username or password.");
        }
      } catch (error: any) {
        setFormError("Incorrect username or password.");
        console.log("Error:", error);
        if (axios.isAxiosError(error)) {
          console.log(
            "Axios error:",
            error.response?.status,
            error.response?.data,
          );
        } else {
          console.log("Error:", error);
        }
      }
    }
  };

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
        <Input
          name="Username/Email"
          placeholder="Value"
          type="text"
          value={usernameOrEmail}
          onChangeText={setUsernameOrEmail}
          onBlur={() => handleBlur("usernameOrEmail", setTouched)}
          autoCapitalize="none"
          returnKeyType="next"
          onSubmitEditing={() => passwordRef.current?.focus()}
        ></Input>
        {(touched.usernameOrEmail || submitted) && !usernameOrEmail && (
          <Text style={globalStyles.errorText}>
            Username or email is required.
          </Text>
        )}
        <Input
          name="Password"
          placeholder="Value"
          type="password"
          value={password}
          onChangeText={setPassword}
          onBlur={() => handleBlur("password", setTouched)}
          autoCapitalize="none"
          returnKeyType="done"
          onSubmitEditing={handleLoginClick}
          ref={passwordRef}
        ></Input>
        {(touched.password || submitted) && !password && (
          <Text style={globalStyles.errorText}>Password is required.</Text>
        )}
        {formError && <Text style={globalStyles.errorText}>{formError}</Text>}
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
