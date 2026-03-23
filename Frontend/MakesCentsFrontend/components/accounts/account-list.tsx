import React, { useEffect, useState } from "react";
import { View, Text } from "react-native";
import { SummaryTransactionDTOModel } from "@/types/summary-transaction-dto-model";
import TransactionCard from "@/components/transactions/transaction-card";
import { SafeAreaView } from "react-native-safe-area-context";
import { storage } from "@/data/storage";
import { screenHeight } from "@/css/globalStyles";
import { AccountSummaryDTOModel } from "@/types/account-summary-dto-model";
import AccountCard from "./account-card";

type AccountListProps = {
  accounts: AccountSummaryDTOModel[];
};

export default function AccountList(props: AccountListProps) {
  const [budgetId, setBudgetId] = useState<number>(0);

  useEffect(() => {
    const loadBudgetId = async () => {
      const storedBudgetId = await storage.getBudgetId();
      setBudgetId(storedBudgetId || 0);
    };
    loadBudgetId();
  }, []);

  const accountList = (props.accounts || []).map(
    (account: AccountSummaryDTOModel) => {
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
          <Text>No Accounts</Text>
        ) : (
          <>{accountList}</>
        )}
      </View>
    </SafeAreaView>
  );
}
