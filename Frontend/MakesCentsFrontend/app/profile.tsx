import React, { useEffect, useState } from "react";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text, Image, View, StyleSheet } from "react-native";
import { Button } from "@/components/buttons/button";
import { router } from "expo-router";
import { globalStyles, screenHeight, screenWidth } from "@/css/globalStyles";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import InfoField from "@/components/text/InfoField";
import Input from "@/components/text/inputs";
import axios, { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { GetUserResponse } from "@/types/get-user-response";
import { GetUserDTOModel } from "@/types/get-user-dto-model";
import IconButton from "@/components/buttons/icon-button";
import { EditUserRequest } from "@/types/edit-user-request";
import { BaseIdResponse } from "@/types/base-id-response";

export default function Profile() {
  // Variables
  const [user, setUser] = useState<GetUserDTOModel>({
    userId: 0,
    username: "",
    email: "",
    isDarkMode: false,
  });
  const [savedUser, setSavedUser] = useState<GetUserDTOModel>({
    userId: 0,
    username: "",
    email: "",
    isDarkMode: false,
  });
  const [isEditMode, setEditMode] = useState<boolean>(false);
  const [newPassword, setNewPassword] = useState<string>("");
  const [reEnterPassword, setReEnterPassword] = useState<string>("");

  // Profile constructor
  useEffect(() => {
    const main = async () => {
      try {
        // Get the budget id from axios
        const axiosResponse: AxiosResponse =
          await makesCentsAxios.get(`/api/user`);

        // Get the response
        const response: GetUserResponse = axiosResponse.data;
        // Log the response
        console.log("Response:", response);
        if (response) {
          // Store the budget id in storage
          setUser(response.getUserDTO);
        }
      } catch (error: any) {
        if (axios.isAxiosError(error)) {
          console.log(
            "Axios error:",
            error.response?.status,
            error.response?.data,
          );
          if (error.response?.status == 401)
            router.replace("/login-register/login");
        } else {
          console.log("Error:", error);
        }
      }
    };

    // Call main
    main();
  }, []);

  const handleLogoutClickEH = () => {
    // Go back to the login page
    router.replace("/login-register/login");
  };

  const handleEditClickEH = () => {
    if (user) {
      setSavedUser(user);
    }
    setEditMode(true);
  };

  const handleCancelClickEH = () => {
    if (savedUser) {
      setUser(savedUser);
    }
    setEditMode(false);
  };

  const handleDoneClickEH = async () => {
    // Create a new update user request
    const request = new EditUserRequest();
    if (user!.username != savedUser!.username) {
      request.username = user!.username;
    }
    if (user!.email != savedUser!.email) {
      request.email = user!.email;
    }
    if (newPassword && newPassword == reEnterPassword) {
      request.passwordHash = newPassword;
    }

    // Make sure the request has information (don't call for 0 updates)
    if (request.username || request.email || request.passwordHash) {
      // Call axios to update the user
      // Set up the try catch
      try {
        // Call the API
        const axiosResponse: AxiosResponse = await makesCentsAxios.put(
          "/api/user",
          request,
        );

        // Get the response
        const response: BaseIdResponse = axiosResponse.data;

        // Log the response
        //console.log("Response:", response);

        // Check the response code
        if (response.httpStatus == 200) {
          // Set the saved user as the user
          setSavedUser(user);
        } else {
          console.log("Login failed");
          /* 
          --------------------------------------------------------------------------------------------
            DEAL WITH UPDATE FAIL
          --------------------------------------------------------------------------------------------
        */
        }
      } catch (error: any) {
        console.log("Error:", error);
        console.log("Status:", error.response?.status);
        console.log("Response:", error.response?.data);
        /* 
      --------------------------------------------------------------------------------------------
        DEAL WITH SAVE USER FAIL
      --------------------------------------------------------------------------------------------
      */
      }
    }
    setEditMode(false);
  };

  return (
    <ScreenWrapper>
      <ScrollView style={{ marginVertical: 0 }}>
        <View style={[globalStyles.noWordsLogoContainer, { marginBottom: 20 }]}>
          <Image
            source={require("@/assets/images/MakesCentsLogo.png")}
            style={globalStyles.noWordsLogo}
          />
          <Text style={globalStyles.logoTitle}>Profile</Text>
        </View>
        {isEditMode ? ( // Main content for edit user screen
          <View>
            {user ? ( // Block if user exists
              <View>
                <Input
                  name="Username"
                  value={user?.username ?? ""}
                  placeholder="New Username"
                  type="text"
                  onChangeText={(text) => {
                    setUser({ ...user!, username: text });
                  }}
                />
                <Input
                  name="Email"
                  value={user?.email ?? ""}
                  placeholder="New Email"
                  type="text"
                  onChangeText={(text) => {
                    setUser({ ...user!, email: text });
                  }}
                />
                <Input
                  name="Password"
                  value={newPassword}
                  placeholder="New Password"
                  type="text"
                  onChangeText={setNewPassword}
                />
                <Input
                  name="Re-Enter Password"
                  value={reEnterPassword}
                  placeholder="Re-Enter New Password"
                  type="text"
                  onChangeText={setReEnterPassword}
                />
              </View>
            ) : // Block if user does not exist
            null}
          </View>
        ) : (
          // Main content for view user screen
          <View>
            {user ? ( // Block if user exists
              <View>
                <InfoField name="Username" value={user!.username} />
                <InfoField name="Email" value={user!.email} />
              </View>
            ) : // Block if user does not exist
            null}
          </View>
        )}
      </ScrollView>
      {isEditMode ? ( // Buttons for edit user screen
        <View style={styles.viewButtons}>
          <Button
            name="Cancel"
            onPress={handleCancelClickEH}
            variant="secondary"
            containerStyle={{ paddingTop: 0 }}
          />
          <Button
            name="Done"
            onPress={handleDoneClickEH}
            variant="primary"
            containerStyle={{ paddingTop: 0 }}
          />
        </View>
      ) : (
        // Buttons for view user screen
        <View style={styles.viewButtons}>
          <Button
            name="Sign Out"
            onPress={handleLogoutClickEH}
            variant="secondary"
            containerStyle={{ paddingTop: 0 }}
          />
          <IconButton
            icon="pencil"
            iconSet="material"
            onPress={handleEditClickEH}
          />
        </View>
      )}
      <BottomNavBar />
    </ScreenWrapper>
  );
}

const styles = StyleSheet.create({
  viewButtons: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    margin: 20,
  },
});
