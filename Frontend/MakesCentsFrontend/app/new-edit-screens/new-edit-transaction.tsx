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
import { router, useLocalSearchParams } from "expo-router";
import React, { useEffect, useState } from "react";
import {
  ScrollView,
  View,
  Image,
  Text,
  Keyboard,
  StyleSheet,
} from "react-native";
import { SummaryAccountDTOModel } from "@/types/summary-account-dto-model";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import MultiCategoryEnvelopeDropdown from "@/components/text/envelope-dropdown-input";
import { storage } from "@/data/storage";
import { GetAllEnvelopeCategoriesResponse } from "@/types/get-all-envelope-categories-response";
import { GetPaymentTransactionDTOModel } from "@/types/get-payment-transaction-dto-model";
import { GetTransferTransactionDTOModel } from "@/types/get-transfer-transaction-dto-model";
import { GetAllAccountsResponse } from "@/types/get-all-accounts-response";
import { fromDateOnly } from "@/utils/mappers/dateOnlyMapper";
import {
  emptyTransactionForm,
  mapSplitsWithNames,
  TransactionForm,
} from "@/utils/mappers/transactionMapper";
import Input from "@/components/text/text-input";
import RadioInput from "@/components/text/radio-input";
import { TransferTransactionType } from "@/types/transfer-transaction-type";
import TitleRadioInput, {
  TitleRadioOption,
} from "@/components/text/title-radio-input";
import { Button } from "@/components/buttons/button";
import {
  createPaymentTransaction,
  createTransferTransaction,
  deleteTransaction,
  updatePaymentTransaction,
  updateTransferTransaction,
} from "@/utils/new-edit-helpers/newEditTransactionHelper";
import IntInput from "@/components/text/int-input";
import { PaymentTransactionType } from "@/types/payment-transaction-type";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import { CreatePaymentTransactionResponse } from "@/types/create-payment-transaction-response";
import { CreateTransferTransactionResponse } from "@/types/create-transfer-transaction-response";
import { UpdatePaymentTransactionResponse } from "@/types/update-payment-transaction-response";
import { BaseIdResponse } from "@/types/base-id-response";

type NewEditTransactionProps = {
  paramTransactionId: string;
  paramTransactionType: string;
};

export default function NewEditTransaction() {
  //console.log("URL Params:", useLocalSearchParams());
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
  //console.log("Transaction ID:", transactionId);
  //console.log("Transaction Type:", TransactionType[transactionType]);
  // Check if the screen is creating a new transaction
  const isNew = transactionId === null;

  // Budget id
  const [budgetId, setBudgetId] = useState<number>(0);

  // Transactions
  const [transaction, setTransaction] =
    useState<TransactionForm>(emptyTransactionForm);
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
      // Variables
      let categories: SummaryEnvelopeCategoryResponse[] = [];

      // Load budget id from storage
      const storedBudgetId = await storage.getBudgetId();
      setBudgetId(storedBudgetId || 0);
      //console.log("Budget ID:", storedBudgetId);

      //console.log("Transaction Id:", transactionId);
      //console.log("Transaction Type:", TransactionType[transactionType]);

      // Get the list of envelope categories
      try {
        // Get the envelope categories
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `api/envelope-categories/budget/${storedBudgetId}`,
        );

        // Get the response
        const response: GetAllEnvelopeCategoriesResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );
        // Log the response
        //console.log("Envelope Categories Response:", response);
        // Set the envelope categories
        categories = response.envelopeCategories;

        setEnvelopeCategories(categories);

        //console.log("Envelope Categories:", response.envelopeCategories);
        //console.log(
        //  "Flat Envelopes:",
        //  response.envelopeCategories.flatMap((c) => c.envelopes),
        //);
      } catch (error: any) {
        console.log("Caught error:", error);
        handleAxiosError(error);
      }

      // Get the list of accounts
      try {
        //console.log("Getting accounts for budget id:", storedBudgetId);
        // Get the accounts
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `api/accounts/budget/${storedBudgetId}`,
        );

        // Get the response
        const response: GetAllAccountsResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );
        // Log the response
        //console.log("Accounts Response:", response);
        // Set the accounts
        setAccounts(response.accounts);
        //console.log("Accounts:", response.accounts);
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
            const response: GetPaymentTransactionResponse = JSON.parse(
              JSON.stringify(axiosResponse.data),
              jsonReviver,
            );
            // Log the response
            //console.log("Response:", response);

            // Get the transaction
            const paymentTransaction: GetPaymentTransactionDTOModel =
              response.paymentTransaction;
            //console.log("Payment Transaction found:", paymentTransaction);
            // Set the original transaction for comparison
            setOriginalTransaction(paymentTransaction);
            // Set the transaction for display
            const paymentForm: TransactionForm = {
              type: paymentTransaction.transactionType,
              typeLabel:
                paymentTransaction.totalAmount < 0 ? "Expense" : "Income", // Determine label based on amount
              amount: paymentTransaction.totalAmount,
              date: fromDateOnly(paymentTransaction.transactionDate),
              notes: paymentTransaction.notes,
              accountId: paymentTransaction.accountId,
              merchantName: paymentTransaction.merchantSourceName,
              checkNumber: paymentTransaction.checkNumber,
              paymentTransactionType: paymentTransaction.paymentTransactionType,
              splits: mapSplitsWithNames(
                paymentTransaction.transactionSplits,
                categories,
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

          //console.log("Transaction ID:", transactionId);
          try {
            // Get the payment transaction to edit
            const axiosResponse: AxiosResponse = await makesCentsAxios.get(
              `api/transfer-transactions/${transactionId}`,
            );

            // Get the response
            const response: GetTransferTransactionDTOResponse = JSON.parse(
              JSON.stringify(axiosResponse.data),
              jsonReviver,
            );
            // Log the response
            //console.log("Response:", response);
            // Get the transaction
            const transferTransaction: GetTransferTransactionDTOModel =
              response.transferTransaction;

            //console.log("Transfer Transaction found:", transferTransaction);
            // Set the original transaction for comparison
            setOriginalTransaction(transferTransaction);
            // Set the transaction for display
            const transferForm: TransactionForm = {
              type: transferTransaction.transactionType,
              typeLabel: "Transfer",
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
            //console.log("Transfer Form:", transferForm);
            setTransaction(transferForm);
          } catch (error: any) {
            console.log("Caught error:", error);
            handleAxiosError(error);
          }
        }
        // Update the values for the use states
      } else {
        setTransaction(emptyTransactionForm);
      }
    };

    // Call to main
    main();
  }, []);

  const titleRadioOptions: TitleRadioOption[] = [
    { label: "Expense", value: "Expense", type: TransactionType.Payment },
    { label: "Income", value: "Income", type: TransactionType.Payment },
    { label: "Transfer", value: "Transfer", type: TransactionType.Transfer },
  ];

  // Method to update single field K in transaction
  const updateTransaction = <K extends keyof TransactionForm>(
    key: K,
    value: TransactionForm[K],
  ) => {
    setTransaction((prev) => ({ ...prev, [key]: value }));
  };

  const handleDeleteClickEH = () => {
    deleteTransaction(originalTransaction!.transactionId);
    router.back();
  };

  // Click EHs for cancel and done (cancel just goes back, done calls axios to update or create transaction and then goes back)
  const handleCancelClickEH = () => {
    router.back();
  };

  const handleDoneClickEH = async () => {
    if (isNew) {
      // Call axios to create the transaction
      // Set up the try catch
      try {
        if (transaction.type === TransactionType.Payment) {
          // Call the helper method to create the transaction
          const response: CreatePaymentTransactionResponse =
            await createPaymentTransaction(transaction, budgetId);
        } else if (transaction.type === TransactionType.Transfer) {
          // Call the helper method to create the transaction
          const response: CreateTransferTransactionResponse =
            await createTransferTransaction(transaction, budgetId);
        }
        // Go back to the transactions list
        router.replace("/transactions");
      } catch (error: any) {
        console.log("Caught error:", error);
        handleAxiosError(error);
      }
    } else {
      // Call axios to update the transaction
      // Set up the try catch
      try {
        if (transaction.type === TransactionType.Payment) {
          if (originalTransaction!.transactionType !== transaction.type) {
            deleteTransaction(originalTransaction!.transactionId);
            const response: CreateTransferTransactionResponse =
              await createTransferTransaction(
                transaction,
                originalTransaction!.budgetId,
              );
          }
          const response: UpdatePaymentTransactionResponse =
            await updatePaymentTransaction(
              originalTransaction as GetPaymentTransactionDTOModel,
              transaction,
            );
        } else if (transaction.type === TransactionType.Transfer) {
          if (originalTransaction!.transactionType !== transaction.type) {
            deleteTransaction(originalTransaction!.transactionId);
            const response: CreatePaymentTransactionResponse =
              await createPaymentTransaction(
                transaction,
                originalTransaction!.budgetId,
              );
          }
          const response: BaseIdResponse = await updateTransferTransaction(
            originalTransaction as GetTransferTransactionDTOModel,
            transaction,
          );
        }
        // Go back to the transactions list
        router.replace("/transactions");
      } catch (error: any) {
        console.log("Caught error:", error);
        handleAxiosError(error);
      }
    }
  };

  // Constants for different screen layouts (expense, income, envelope transfer, account transfer)
  const renderExpenseTransactionView = () => {
    return (
      <View style={styles.inputsView}>
        {renderCalendarInput()}
        {renderAmountInput("negative")}
        {renderPaymentAccountInput()}
        {renderPaymentTypeInput()}
        <Input
          name="Merchant Name"
          placeholder="Merchant"
          type="text"
          value={transaction.merchantName || ""}
          onChangeText={(text) => updateTransaction("merchantName", text)}
          autoCapitalize="words"
        />
        {renderPaymentEnvelopeSelectInput()}
        {renderCheckNumberInput()}
        {renderNotesInput()}
        {renderDeleteButton()}
      </View>
    );
  };

  const renderIncomeTransactionView = () => {
    return (
      <View style={styles.inputsView}>
        {renderCalendarInput()}
        {renderAmountInput("positive")}
        {renderPaymentAccountInput()}
        {renderPaymentTypeInput()}
        <Input
          name="Source Name"
          placeholder="Source"
          type="text"
          value={transaction.merchantName || ""}
          onChangeText={(text) => updateTransaction("merchantName", text)}
          autoCapitalize="words"
        />
        {renderPaymentEnvelopeSelectInput()}
        {renderCheckNumberInput()}
        {renderNotesInput()}
        {renderDeleteButton()}
      </View>
    );
  };

  const renderEnvelopeTransferTransactionView = () => {
    return (
      <View style={styles.inputsView}>
        {renderTransferTypeSelection()}
        {renderAmountInput("neutral")}
        <SingleDropdownInput
          name="From Envelope"
          value={envelopeCategories
            .flatMap((c) => c.envelopes)
            .find((e) => e.envelopeId === transaction.fromId)}
          items={envelopeCategories.flatMap((c) => c.envelopes)}
          getLabel={(env) => env!.envelopeName}
          getValue={(env) => env!.envelopeId.toString()}
          groupBy={(env) => {
            const category = envelopeCategories.find((cat) =>
              cat.envelopes.some((e) => e.envelopeId === env!.envelopeId),
            );
            return category?.envelopeCategoryName || null;
          }}
          onChange={(env) => updateTransaction("fromId", env?.envelopeId!)}
        />
        <SingleDropdownInput
          name="To Envelope"
          value={envelopeCategories
            .flatMap((c) => c.envelopes)
            .find((e) => e.envelopeId === transaction.toId)}
          items={envelopeCategories.flatMap((c) => c.envelopes)}
          getLabel={(env) => env!.envelopeName}
          getValue={(env) => env!.envelopeId.toString()}
          groupBy={(env) => {
            const category = envelopeCategories.find((cat) =>
              cat.envelopes.some((e) => e.envelopeId === env!.envelopeId),
            );
            return category?.envelopeCategoryName || null;
          }}
          onChange={(env) => updateTransaction("toId", env?.envelopeId!)}
        />
        {renderNotesInput()}
        {renderDeleteButton()}
      </View>
    );
  };

  const renderAccountTransferTransactionView = () => {
    return (
      <View style={styles.inputsView}>
        {renderTransferTypeSelection()}
        {renderAmountInput("neutral")}
        <SingleDropdownInput
          name="From Account"
          value={accounts.find(
            (account) => account.accountId === transaction.fromId,
          )}
          items={accounts}
          getLabel={(a) => a!.accountName}
          getValue={(a) => a!.accountId.toString()}
          onChange={(account) =>
            updateTransaction("fromId", account?.accountId!)
          }
        />
        <SingleDropdownInput
          name="To Account"
          value={accounts.find(
            (account) => account.accountId === transaction.toId,
          )}
          items={accounts}
          getLabel={(a) => a!.accountName}
          getValue={(a) => a!.accountId.toString()}
          onChange={(account) => updateTransaction("toId", account?.accountId!)}
        />
        {renderNotesInput()}
        {renderDeleteButton()}
      </View>
    );
  };

  // Constants for shared sections of the screen (date, amount, notes)

  const renderTransferTypeSelection = () => {
    const options = [
      {
        label: "Account",
        value: TransferTransactionType.Account.toString(),
      },
      {
        label: "Envelope",
        value: TransferTransactionType.Envelope.toString(),
      },
    ];
    return (
      <View>
        {renderCalendarInput()}
        <RadioInput
          name="Type of Transfer"
          value={
            transaction.transferTransactionType !== null
              ? transaction.transferTransactionType.toString()
              : ""
          }
          onChange={(value) =>
            updateTransaction(
              "transferTransactionType",
              Number(value) as TransferTransactionType,
            )
          }
          options={options}
        />
      </View>
    );
  };

  const renderCalendarInput = () => {
    return (
      <View>
        <CalendarInput
          name="Date"
          value={transaction.date}
          onChange={(text) => updateTransaction("date", text)}
        />
      </View>
    );
  };

  const renderAmountInput = (sign: "positive" | "negative" | "neutral") => {
    const math =
      sign === "positive"
        ? Math.abs
        : sign === "negative"
          ? (x: number) => -Math.abs(x)
          : (x: number) => x;
    return (
      <MoneyInput
        name="Amount"
        value={transaction.amount}
        onChangeValue={(amount) => updateTransaction("amount", math(amount!))}
      />
    );
  };

  const renderPaymentAccountInput = () => {
    return (
      <View>
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
      </View>
    );
  };

  const renderPaymentTypeInput = () => {
    const paymentTypeOptions = Object.values(PaymentTransactionType).filter(
      (v) => typeof v === "number",
    );
    return (
      <View>
        <SingleDropdownInput
          name="Payment Type"
          value={transaction.paymentTransactionType}
          items={paymentTypeOptions}
          getLabel={(type) => PaymentTransactionType[type!]} // enum label
          getValue={(type) => type!.toString()}
          onChange={(type) =>
            updateTransaction("paymentTransactionType", type ?? null)
          }
        />
      </View>
    );
  };

  const renderPaymentEnvelopeSelectInput = () => {
    return (
      <View>
        <MultiCategoryEnvelopeDropdown
          name="Select Envelopes"
          categories={envelopeCategories}
          selectedEnvelopes={transaction.splits!}
          onChange={(splits) => updateTransaction("splits", splits)}
          totalAmount={transaction.amount!}
        />
      </View>
    );
  };

  const renderCheckNumberInput = () => {
    return (
      <IntInput
        name="Check Number (Optional)"
        value={transaction.checkNumber || null}
        placeHolder="Check Number"
        onChangeValue={(checkNumber) =>
          updateTransaction("checkNumber", checkNumber)
        }
      />
    );
  };

  const renderNotesInput = () => {
    return (
      <View>
        <Input
          name="Notes (Optional)"
          placeholder="Notes"
          type="text"
          value={transaction.notes || ""}
          onChangeText={(notes) => updateTransaction("notes", notes)}
          boxStyle={{ height: 125 }}
          line="multi"
          autoCapitalize="sentences"
          onSubmitEditing={Keyboard.dismiss}
          returnKeyType="default"
        />
      </View>
    );
  };

  const renderDeleteButton = () => {
    return (
      <View>
        <Button name="Delete" onPress={handleDeleteClickEH} variant="delete" />
      </View>
    );
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
            {isNew ? "New" : "Edit"} Transaction
          </Text>
        </View>
      </View>

      <ScrollView
        style={{ marginVertical: 0 }}
        contentContainerStyle={{ paddingBottom: 10 }}
      >
        {/* Radio buttons for Expense, Income, and Transfer */}
        <TitleRadioInput
          value={transaction.typeLabel || ""} // string for UI selection
          options={titleRadioOptions}
          onChange={(selectedValue) => {
            const selectedOption = titleRadioOptions.find(
              (o) => o.value === selectedValue,
            )!;

            // Update type label and internal type
            updateTransaction("typeLabel", selectedOption.value);
            updateTransaction("type", selectedOption.type as TransactionType);

            // Adjust amount sign if this is a payment transaction
            if (selectedOption.value === "Expense") {
              updateTransaction("amount", -Math.abs(transaction.amount!));
            } else if (
              selectedOption.value === "Income" ||
              selectedOption.value === "Transfer"
            ) {
              updateTransaction("amount", Math.abs(transaction.amount!));
            }
            // For "Transfer", can leave amount
          }}
        />
        {/* Conditionally render the rest of the form based on the transaction type */}
        {transaction.type === TransactionType.Payment &&
          transaction.typeLabel === "Expense" &&
          renderExpenseTransactionView()}
        {transaction.type === TransactionType.Payment &&
          transaction.typeLabel === "Income" &&
          renderIncomeTransactionView()}
        {transaction.type === TransactionType.Transfer &&
          transaction.transferTransactionType ===
            TransferTransactionType.Envelope &&
          renderEnvelopeTransferTransactionView()}
        {transaction.type === TransactionType.Transfer &&
          transaction.transferTransactionType ===
            TransferTransactionType.Account &&
          renderAccountTransferTransactionView()}
      </ScrollView>
      {renderCancelDoneButtons()}
      <BottomNavBar />
    </ScreenWrapper>
  );
}

const styles = StyleSheet.create({
  inputsView: {
    marginBottom: 10,
  },
});
