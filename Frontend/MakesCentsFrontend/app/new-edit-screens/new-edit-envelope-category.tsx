import BottomNavBar from "@/components/bottom-nav-bar";
import { Button } from "@/components/buttons/button";
import Input from "@/components/text/text-input";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import makesCentsAxios from "@/data/datasource";
import { BaseIdResponse } from "@/types/base-id-response";
import { CreateEnvelopeCategoryRequest } from "@/types/create-envelope-category-request";
import { EditEnvelopeCategoryRequest } from "@/types/edit-envelope-category-request";
import { GetEnvelopeCategoryResponse } from "@/types/get-envelope-category-response";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import { handleBlur } from "@/utils/touched";
import { AxiosResponse } from "axios";
import { router, useLocalSearchParams } from "expo-router";
import React, { useEffect, useState } from "react";
import { View, Image, Text, ScrollView } from "react-native";

type NewEditEnvelopeCategoryProps = {
  budgetId: string;
  envelopeCategoryId?: string; // Optional for edit, not needed for create
};

export default function NewEditEnvelopeCategory() {
  //console.log("URL Params:", useLocalSearchParams());
  // Parameter mapping
  const { budgetId: budgetIdString, envelopeCategoryId } =
    useLocalSearchParams<NewEditEnvelopeCategoryProps>();
  const budgetId: number = parseInt(budgetIdString);
  const [envelopeCategoryName, setEnvelopeCategoryName] = useState<string>("");
  const isNew = !envelopeCategoryId;

  const [touched, setTouched] = useState<{ name: boolean }>({ name: false });
  const [formError, setFormError] = useState<string | null>(null);
  const [submitted, setSubmitted] = useState(false);

  useEffect(() => {
    const main = async () => {
      if (!isNew) {
        try {
          // Fetch existing envelope category details
          const axiosResponse: AxiosResponse = await makesCentsAxios.get(
            `/api/envelope-categories/${envelopeCategoryId}`,
          );

          // Get the response
          const response: GetEnvelopeCategoryResponse = JSON.parse(
            JSON.stringify(axiosResponse.data),
            jsonReviver,
          );
          // Log the response
          //console.log("Fetch Envelope Category Response:", response);

          // Set the form fields with existing data
          setEnvelopeCategoryName(
            response.envelopeCategory.envelopeCategoryName || "",
          );
        } catch (error: any) {
          handleAxiosError(error);
        }
      }
    };

    // Call main
    main();
  }, []);

  const handleCancelClickEH = () => {
    router.back();
  };

  const handleDoneClickEH = async () => {
    if (isNew) {
      // Set the form as submitted
      setSubmitted(true);
      if (!envelopeCategoryName) return;
      try {
        // Create the request
        const request: CreateEnvelopeCategoryRequest = {
          userId: 0, // This will be set by the backend based on the authenticated user
          budgetId,
          envelopeCategoryName,
        };

        // Call the API
        const axiosResponse: AxiosResponse = await makesCentsAxios.post(
          "/api/envelope-categories",
          request,
        );

        // Get the response
        const response: BaseIdResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );
        // Log the response
        console.log("Create Envelope Category Response:", response);

        // Set the new budget id
        router.push("/budget");
      } catch (error: any) {
        setFormError("Error with update");
        handleAxiosError(error);
      }
    } else {
      try {
        // Create the request
        const request: EditEnvelopeCategoryRequest = {
          userId: 0, // This will be set by the backend based on the authenticated user
          envelopeCategoryId: 0,
          envelopeCategoryName,
        };

        // Call the API
        const axiosResponse: AxiosResponse = await makesCentsAxios.put(
          `/api/envelope-categories/${parseInt(envelopeCategoryId!)}`,
          request,
        );

        // Get the response
        const response: BaseIdResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );
        // Log the response
        console.log("Create Envelope Category Response:", response);

        // Set the new budget id
        router.push("/budget");
      } catch (error: any) {
        setFormError("Error with creation");
        handleAxiosError(error);
      }
    }
  };

  const renderCancelDoneButtons = () => {
    return (
      <View style={globalStyles.bottomButtons}>
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
    );
  };

  return (
    <ScreenWrapper>
      {/* Logo and Title */}
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />

        <View style={globalStyles.logoTitleContainer}>
          <Text style={globalStyles.logoTitle}>
            {" "}
            {isNew ? "New" : "Edit"} Envelope Category
          </Text>
        </View>
      </View>
      {/* Content */}
      <ScrollView
        style={{ marginVertical: 0 }}
        contentContainerStyle={{ paddingBottom: 75 }}
      >
        {/* Budget creation form goes here */}
        <Input
          name="Envelope Category Name"
          placeholder="Envelope Category Name"
          type="text"
          value={envelopeCategoryName || ""}
          onChangeText={setEnvelopeCategoryName}
          autoCapitalize="words"
          onBlur={() => handleBlur("name", setTouched)}
        />
        {(touched.name || submitted) && !envelopeCategoryName && isNew && (
          <Text style={globalStyles.errorText}>
            Envelope category name is required.
          </Text>
        )}
        {formError && <Text style={globalStyles.errorText}>{formError}</Text>}
      </ScrollView>
      {renderCancelDoneButtons()}
      <BottomNavBar />
    </ScreenWrapper>
  );
}
