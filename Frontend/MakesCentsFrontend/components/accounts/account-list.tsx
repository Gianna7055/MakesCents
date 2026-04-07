import React from "react";
import { View, Text } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { globalStyles, screenHeight } from "@/css/globalStyles";
import AccountCard from "./account-card";
import { SummaryAccountDTOModel } from "@/types/summary-account-dto-model";

type AccountListProps = {
  accounts: SummaryAccountDTOModel[];
};

export default function AccountList(props: AccountListProps) {
  const accountList = (props.accounts || []).map(
    (account: SummaryAccountDTOModel) => {
      return (
        // Returns a table row
        <AccountCard key={account.accountId} account={account} />
      );
    },
  );
  return (
    <SafeAreaView>
      <View style={{ marginVertical: -(screenHeight * 0.02) }}>
        {accountList.length === 0 ? (
          <View style={globalStyles.emptyListContainer}>
            <Text style={globalStyles.emptyListText}>No Accounts</Text>
          </View>
        ) : (
          <>{accountList}</>
        )}
      </View>
    </SafeAreaView>
  );
}
