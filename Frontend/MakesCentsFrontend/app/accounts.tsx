import React, { useEffect, useState } from "react";
import { globalStyles, screenHeight, screenWidth } from "@/css/globalStyles";
import BottomNavBar from "@/components/bottom-nav-bar";
import { ScrollView, Text, View, Image } from "react-native";
import AccountList from "@/components/accounts/account-list";
import { Button } from "@/components/buttons/button";
import { storage } from "@/data/storage";
import axios, { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { GetAllAccountsResponse } from "@/types/get-all-accounts-response";
import { router } from "expo-router";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { SummaryAccountDTOModel } from "@/types/summary-account-dto-model";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";

export default function Accounts() {
  const [budgetId, setBudgetId] = useState<number>(0);
  const [accounts, setAccounts] = useState<SummaryAccountDTOModel[]>([]);

  // Run on create (constructor)
  useEffect(() => {
    const main = async () => {
      // Load token and budget id from storage
      const storedBudgetId = await storage.getBudgetId();

      setBudgetId(storedBudgetId || 0);

      try {
        //console.log("BudgetId:", storedBudgetId);
        // Load transactions from the backend
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/accounts/budget/${storedBudgetId}`,
        );
        // Get the response
        const response: GetAllAccountsResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );

        // Sort the accounts by the account name
        setAccounts(
          response.accounts.sort((a, b) =>
            a.accountName.localeCompare(b.accountName),
          ),
        );
      } catch (error: any) {
        handleAxiosError(error);
      }
    };

    // Main method call
    main();
  }, []);

  const handlePlusClick = () => {};

  return (
    <ScreenWrapper>
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <Text style={globalStyles.logoTitle}>Accounts</Text>
      </View>
      <ScrollView style={{ marginVertical: 0 }}>
        <AccountList accounts={accounts} />
      </ScrollView>
      <Button
        name="+"
        onPress={handlePlusClick}
        containerStyle={{
          position: "absolute",
          bottom: screenHeight * 0.115,
          right: screenWidth * 0.03,
        }}
        style={{
          width: 60,
          height: 60,
          borderRadius: 30,
          justifyContent: "center",
          alignItems: "center",
        }}
        textStyle={{
          fontSize: 30,
          lineHeight: 34,
        }}
        variant="tertiary"
      />
      <BottomNavBar />
    </ScreenWrapper>
  );
}
