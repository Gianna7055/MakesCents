import BottomNavBar from "@/components/bottom-nav-bar";
import CalendarInput from "@/components/text/calendar-input";
import SingleDropdownInput from "@/components/text/single-dropdown-input";
import MoneyInput from "@/components/text/money-input";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles } from "@/css/globalStyles";
import makesCentsAxios from "@/data/datasource";
import { GetPaymentTransactionResponse } from "@/types/get-payment-transaction-response";
import { GetTransferTransactionDTOResponse } from "@/types/get-transfer-transaction-dto-response";
import { TransactionType } from "@/types/transaction-type";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { AxiosResponse } from "axios";
import { useLocalSearchParams } from "expo-router";
import React, { useEffect, useState } from "react";
import { ScrollView, View, Image, Text } from "react-native";
import { SummaryAccountDTOModel } from "@/types/summary-account-dto-model";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import MultiCategoryEnvelopeDropdown from "@/components/text/multi-dropdown-input";
import { storage } from "@/data/storage";
import { GetAllEnvelopeCategoriesResponse } from "@/types/get-all-envelope-categories-response";
import { GetPaymentTransactionDTOModel } from "@/types/get-payment-transaction-dto-model";
import { GetTransferTransactionDTOModel } from "@/types/get-transfer-transaction-dto-model";
import { GetAllAccountsResponse } from "@/types/get-all-accounts-response";
import { fromDateOnly } from "@/utils/mappers/dateOnlyMapper";
import {
  emptyForm,
  mapSplitsWithNames,
  TransactionForm,
} from "@/utils/mappers/transactionMapper";
import Input from "@/components/text/text-input";

type NewEditTransactionProps = {
  paramTransactionId: string;
  paramTransactionType: string;
};

export default function NewEditTransaction() {
  // Parameter mapping
  const { paramTransactionId, paramTransactionType } =
    useLocalSearchParams<NewEditTransactionProps>();
  // Get the transaction id from the param
  const transactionId = paramTransactionId
    ? parseInt(paramTransactionId)
    : null;
  // Get the transaction type from the param
  const transactionType: TransactionType = paramTransactionType
    ? (parseInt(paramTransactionType) as TransactionType)
    : TransactionType.Unknown;
  // Check if the screen is creating a new transaction
  const isNew = transactionId === null;

  // Budget id
  const [budgetId, setBudgetId] = useState<number>(0);

  // Transactions
  const [transaction, setTransaction] = useState<TransactionForm>(emptyForm);
  const [originalTransaction, setOriginalTransaction] = useState<
    GetPaymentTransactionDTOModel | GetTransferTransactionDTOModel | null
  >(null);

  // Accounts
  const [accounts, setAccounts] = useState<SummaryAccountDTOModel[]>([]);
  // Envelope Categories
  const [envelopeCategories, setEnvelopeCategories] = useState<
    SummaryEnvelopeCategoryResponse[]
  >([]);

  // New edit transaction constructor
  useEffect(() => {
    const main = async () => {
      // Load budget id from storage
      const storedBudgetId = await storage.getBudgetId();
      setBudgetId(storedBudgetId || 0);

      // Get the list of envelope categories
      try {
        // Get the envelope categories
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `api/envelope-categories/budget/${storedBudgetId}`,
        );

        // Get the response
        const response: GetAllEnvelopeCategoriesResponse = axiosResponse.data;
        // Log the response
        //console.log("Envelope Categories Response:", response);
        // Set the envelope categories
        setEnvelopeCategories(response.envelopeCategories);
      } catch (error: any) {
        console.log("Caught error:", error);
        handleAxiosError(error);
      }

      // Get the list of accounts
      try {
        // Get the accounts
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `api/accounts/budget/${storedBudgetId}`,
        );

        // Get the response
        const response: GetAllAccountsResponse = axiosResponse.data;
        // Log the response
        //console.log("Accounts Response:", response);
        // Set the accounts
        setAccounts(response.accounts);
      } catch (error: any) {
        console.log("Caught error:", error);
        handleAxiosError(error);
      }

      // Check if this is editing a transaction
      if (!isNew) {
        if (transactionType == TransactionType.Payment) {
          // Try-catch for the axios call
          try {
            // Get the payment transaction to edit
            const axiosResponse: AxiosResponse = await makesCentsAxios.get(
              `api/payment-transactions/${transactionId}`,
            );

            // Get the response
            const response: GetPaymentTransactionResponse = axiosResponse.data;
            // Log the response
            //console.log("Response:", response);

            // Get the transaction
            const paymentTransaction: GetPaymentTransactionDTOModel =
              response.paymentTransaction;
            // Set the original transaction for comparison
            setOriginalTransaction(paymentTransaction);
            // Set the transaction for display
            const paymentForm: TransactionForm = {
              type: paymentTransaction.transactionType,
              amount: paymentTransaction.totalAmount,
              date: fromDateOnly(paymentTransaction.transactionDate),
              notes: paymentTransaction.notes,
              accountId: paymentTransaction.accountId,
              merchantName: paymentTransaction.merchantSourceName,
              checkNumber: paymentTransaction.checkNumber,
              paymentTransactionType: paymentTransaction.paymentTransactionType,
              splits: mapSplitsWithNames(
                paymentTransaction.transactionSplits,
                envelopeCategories,
              ),
              transferTransactionType: null,
              fromId: null,
              toId: null,
            };
            setTransaction(paymentForm);
          } catch (error: any) {
            console.log("Caught error:", error);
            handleAxiosError(error);
          }
        } else if (transactionType == TransactionType.Transfer) {
          //console.log("In transaction type is transfer");
          // Try-catch for the axios call
          try {
            // Get the payment transaction to edit
            const axiosResponse: AxiosResponse = await makesCentsAxios.get(
              `api/transfer-transactions/${transactionId}`,
            );

            // Get the response
            const response: GetTransferTransactionDTOResponse =
              axiosResponse.data;
            // Log the response
            //console.log("Response:", response);
            // Get the transaction
            const transferTransaction: GetTransferTransactionDTOModel =
              response.transferTransaction;
            // Set the original transaction for comparison
            setOriginalTransaction(transferTransaction);
            // Set the transaction for display
            const transferForm: TransactionForm = {
              type: transferTransaction.transactionType,
              amount: transferTransaction.totalAmount,
              date: fromDateOnly(transferTransaction.transactionDate),
              notes: transferTransaction.notes,
              accountId: null,
              merchantName: null,
              checkNumber: null,
              paymentTransactionType: null,
              splits: null,
              transferTransactionType:
                transferTransaction.transferTransactionType,
              fromId: transferTransaction.transferFromId,
              toId: transferTransaction.transferToId,
            };
            setTransaction(transferForm);
          } catch (error: any) {
            console.log("Caught error:", error);
            handleAxiosError(error);
          }
        }
        // Update the values for the use states
      } else {
        setTransaction(emptyForm);
      }
    };

    // Call to main
    main();
  }, []);

  // Method to update single field K in transaction
  const updateTransaction = <K extends keyof TransactionForm>(
    key: K,
    value: TransactionForm[K],
  ) => {
    setTransaction((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <ScreenWrapper>
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <Text style={globalStyles.logoTitle}>
          {isNew ? "New" : "Edit"} Transaction
        </Text>
      </View>
      {/* Radio buttons for Expense, Income, and Transfer */}
      <ScrollView style={{ marginVertical: 0 }}>
        <CalendarInput
          name="Date"
          value={transaction.date}
          onChange={(text) => updateTransaction("date", text)}
        />
        <MoneyInput
          name="Amount"
          value={transaction.amount}
          onChangeValue={(amount) => updateTransaction("amount", amount)}
        />

        <SingleDropdownInput
          name="Account"
          value={accounts.find(
            (account) => account.accountId === transaction.accountId,
          )}
          items={accounts}
          getLabel={(a) => a!.accountName}
          getValue={(a) => a!.accountId.toString()}
          onChange={(account) =>
            updateTransaction("accountId", account?.accountId!)
          }
        />
        <MultiCategoryEnvelopeDropdown
          name="Select Envelopes"
          categories={envelopeCategories}
          selectedEnvelopes={transaction.splits!}
          onChange={(splits) => updateTransaction("splits", splits)}
        />
        <Input
          name="Notes (optional)"
          placeholder="Notes"
          type="text"
          value={transaction.notes || ""}
          onChangeText={(notes) => updateTransaction("notes", notes)}
          boxStyle={{ height: 50 }}
        />
      </ScrollView>
      <BottomNavBar />
    </ScreenWrapper>
  );
}
