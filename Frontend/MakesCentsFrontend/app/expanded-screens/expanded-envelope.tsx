import BottomNavBar from "@/components/bottom-nav-bar";
import { Button } from "@/components/buttons/button";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import makesCentsAxios from "@/data/datasource";
import { GetEnvelopeDTOModel } from "@/types/get-envelope-dto-model";
import { GetEnvelopeDTOResponse } from "@/types/get-envelope-dto-response";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import { AxiosResponse } from "axios";
import { router, useLocalSearchParams } from "expo-router";
import React, { useEffect, useState } from "react";
import { View, Image, Text, ScrollView } from "react-native";

type ExpandedEnvelopeProps = {
  envelopeId: string;
};

export default function ExpandedEnvelope() {
  //console.log("URL Params:", useLocalSearchParams());
  // Parameter mapping
  const { envelopeId: stringEnvelopeId } =
    useLocalSearchParams<ExpandedEnvelopeProps>();
  // Get the transaction id from the param
  const envelopeId = stringEnvelopeId ? parseInt(stringEnvelopeId) : null;
  const [envelope, setEnvelope] = useState<GetEnvelopeDTOModel>();

  useEffect(() => {
    const main = async () => {
      // Set up the try-catch
      try {
        // Get the envelope
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/envelopes/${envelopeId}`,
        );
        // Log the axios response
        console.log("Axios response:", axiosResponse);

        // Get the response
        const response: GetEnvelopeDTOResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );
        // Log the response
        console.log("Get Envelope Response:", response);

        // Set the envelope
        setEnvelope(response.envelopeDTO);
      } catch (error: any) {
        handleAxiosError(error);
      }
    };

    // Call to main
    main();
  }, []);
  const handleCancelClickEH = () => {
    router.back();
  };

  const handleDoneClickEH = async () => {};

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
            {envelope ? envelope.envelopeName : "Edit Envelope"}
          </Text>
        </View>
      </View>
      <ScrollView></ScrollView>
      {renderCancelDoneButtons()}
      <BottomNavBar />
    </ScreenWrapper>
  );
}
