import BottomNavBar from "@/components/bottom-nav-bar";
import { Button } from "@/components/buttons/button";
import CalendarInput from "@/components/text/calendar-input";
import IntInput from "@/components/text/int-input";
import MoneyInput from "@/components/text/money-input";
import RadioInput from "@/components/text/radio-input";
import SingleDropdownInput from "@/components/text/single-dropdown-input";
import Input from "@/components/text/text-input";
import TitleRadioInput, {
  TitleRadioOption,
} from "@/components/text/title-radio-input";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import { storage } from "@/data/storage";
import { AccountType } from "@/types/account-type";
import { BankAccountType } from "@/types/bank-account-type";
import { CreateBankAccountResponse } from "@/types/create-bank-account-response";
import { DebtAccountType } from "@/types/debt-account-type";
import { DebtPaymentRegularity } from "@/types/debt-payment-regularity";
import { InvestmentAccountType } from "@/types/investment-account-type";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { AccountForm, emptyAccountForm } from "@/utils/mappers/accountMapper";
import { createBankAccount } from "@/utils/new-edit-helpers/newEditAccountHelper";
import { router } from "expo-router";
import React, { useEffect } from "react";
import { useState } from "react";
import { StyleSheet, View, Image, Text, ScrollView } from "react-native";

export default function NewAccount() {
  const [account, setAccount] = useState<AccountForm>(emptyAccountForm);
  const [budgetId, setBudgetId] = useState<number>(0);

  const titleRadioOptions: TitleRadioOption[] = [
    { label: "Bank", value: "Bank", type: AccountType.Bank },
    { label: "Debt", value: "Debt", type: AccountType.Debt },
    { label: "Investment", value: "Investment", type: AccountType.Investment },
  ];

  useEffect(() => {
    const main = async () => {
      // Load budget id from storage
      setBudgetId((await storage.getBudgetId()) || 0);
    };

    // Call main
    main();
  }, []);

  // Method to update single field K in an account
  const updateAccountForm = <K extends keyof AccountForm>(
    key: K,
    value: AccountForm[K],
  ) => {
    setAccount((prev) => ({ ...prev, [key]: value }));
  };

  const handleCancelClickEH = () => {
    router.back();
  };

  const handleDoneClickEH = async () => {
    // Set up the try catch
    try {
      if (account.accountType === AccountType.Bank) {
        // Call the helper method ot create the bank account
        const response: CreateBankAccountResponse = await createBankAccount(
          account,
          budgetId,
        );
      } else if (account.accountType === AccountType.Debt) {
        // Call the helper method ot create the debt account
        const response: CreateBankAccountResponse = await createBankAccount(
          account,
          budgetId,
        );
      } else if (account.accountType === AccountType.Investment) {
        // Call the helper method ot create the investment account
        const response: CreateBankAccountResponse = await createBankAccount(
          account,
          budgetId,
        );
      }
      // Go back to the accounts page
      router.replace("/accounts");
    } catch (error: any) {
      handleAxiosError(error);
    }
  };

  // Constants for different screen layouts (bank, debt, investment)
  const renderCommonAccountView = () => {
    return (
      <View style={styles.inputsView}>
        {renderAccountNameInput()}
        {renderInstitutionInput()}
        {renderBalanceInput()}
      </View>
    );
  };

  const renderBankAccountView = () => {
    return (
      <View style={styles.inputsView}>{renderBankAccountTypeInput()}</View>
    );
  };

  const renderDebtAccountView = () => {
    return (
      <View style={styles.inputsView}>
        {renderDebtAccountTypeInput()}
        {renderAccountNumberInput()}
        {renderNextBillInputs()}
      </View>
    );
  };

  const renderInvestmentAccountView = () => {
    return (
      <View style={styles.inputsView}>
        {renderInvestmentAccountTypeInput()}
        {renderAccountNumberInput()}
        {renderTaxStatusInput()}
      </View>
    );
  };

  // Constants for shared inputs
  const renderAccountNameInput = () => {
    return (
      <Input
        name="Account Name"
        placeholder="Name"
        type="text"
        value={account.accountName || ""}
        onChangeText={(text) => updateAccountForm("accountName", text)}
        autoCapitalize="words"
      />
    );
  };

  const renderInstitutionInput = () => {
    return (
      <Input
        name="Institution"
        placeholder="Institution"
        type="text"
        value={account.institution || ""}
        onChangeText={(text) => updateAccountForm("institution", text)}
        autoCapitalize="words"
      />
    );
  };

  const renderBalanceInput = () => {
    return (
      <MoneyInput
        name="Balance"
        value={account.balance}
        onChangeValue={(amount) => updateAccountForm("balance", amount)}
      />
    );
  };

  const renderBankAccountTypeInput = () => {
    const accountTypeOptions = Object.values(BankAccountType).filter(
      (v) => typeof v === "number" && v !== 1,
    );
    return (
      <View>
        <SingleDropdownInput
          name="Bank Account Type"
          value={account.bankAccountType}
          items={accountTypeOptions}
          getLabel={(type) => BankAccountType[type!]} // enum label
          getValue={(type) => type!.toString()}
          onChange={(type) =>
            updateAccountForm("bankAccountType", type ?? null)
          }
        />
      </View>
    );
  };

  const renderDebtAccountTypeInput = () => {
    const accountTypeOptions = Object.values(DebtAccountType).filter(
      (v) => typeof v === "number" && v !== 1,
    );
    return (
      <View>
        <SingleDropdownInput
          name="Debt Account Type"
          value={account.debtAccountType}
          items={accountTypeOptions}
          getLabel={(type) => DebtAccountType[type!]} // enum label
          getValue={(type) => type!.toString()}
          onChange={(type) =>
            updateAccountForm("debtAccountType", type ?? null)
          }
        />
      </View>
    );
  };

  const renderInvestmentAccountTypeInput = () => {
    const accountTypeOptions = Object.values(InvestmentAccountType).filter(
      (v) => typeof v === "number" && v !== 1,
    );
    return (
      <View>
        <SingleDropdownInput
          name="Account Type"
          value={account.investmentAccountType}
          items={accountTypeOptions}
          getLabel={(type) => InvestmentAccountType[type!]} // enum label
          getValue={(type) => type!.toString()}
          onChange={(type) =>
            updateAccountForm("investmentAccountType", type ?? null)
          }
        />
      </View>
    );
  };

  const renderAccountNumberInput = () => {
    return (
      <IntInput
        name="Account Number (Optional)"
        value={account.accountNumber || null}
        placeHolder="Account Number"
        onChangeValue={(number) => updateAccountForm("accountNumber", number)}
      />
    );
  };

  const renderNextBillInputs = () => {
    const regularityOptions = Object.values(DebtPaymentRegularity).filter(
      (v) => typeof v === "number",
    );
    return (
      <View>
        <CalendarInput
          name="Date of Next Bill (Optional)"
          value={account.dateOfNextBill || null}
          onChange={(text) => updateAccountForm("dateOfNextBill", text)}
        />
        <MoneyInput
          name="Amount of Next Bill (Optional)"
          value={account.amountOfNextBill || 0}
          onChangeValue={(amount) =>
            updateAccountForm("amountOfNextBill", amount!)
          }
        />
        <SingleDropdownInput
          name="Bill Regularity (Optional)"
          value={account.debtPaymentRegularity}
          items={regularityOptions}
          getLabel={(type) => DebtPaymentRegularity[type!]} // enum label
          getValue={(type) => type!.toString()}
          onChange={(type) =>
            updateAccountForm("debtPaymentRegularity", type ?? null)
          }
        />
      </View>
    );
  };

  const renderTaxStatusInput = () => {
    const options = [
      { label: "Yes", value: "true" },
      { label: "No", value: "false" },
    ];

    return (
      <View>
        <RadioInput
          name="Is Account Tax Deferred? (Optional)"
          value={
            account.isTaxDeferred === true
              ? "true"
              : account.isTaxDeferred === false
                ? "false"
                : ""
          }
          onChange={(value) =>
            updateAccountForm("isTaxDeferred", value === "true")
          }
          options={options}
          containerStyle={{ gap: screenWidth * 0.2 }}
        />
        <RadioInput
          name="Is Account Tax Exempt? (Optional)"
          value={
            account.isTaxExempt === true
              ? "true"
              : account.isTaxExempt === false
                ? "false"
                : ""
          }
          onChange={(value) =>
            updateAccountForm("isTaxExempt", value === "true")
          }
          options={options}
          containerStyle={{ gap: screenWidth * 0.2 }}
        />
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
          <Text style={globalStyles.logoTitle}>New Account</Text>
        </View>
      </View>
      <ScrollView
        style={{ marginVertical: 0 }}
        contentContainerStyle={{ paddingBottom: 75 }}
      >
        {/* Radio buttons for Bank, Debt, and Investment */}
        <TitleRadioInput
          value={
            titleRadioOptions.find((o) => o.type === account.accountType)
              ?.value || ""
          } // string for UI selection
          options={titleRadioOptions}
          onChange={(selectedValue) => {
            const selectedOption = titleRadioOptions.find(
              (o) => o.value === selectedValue,
            )!;

            // Update type label and internal type
            updateAccountForm(
              "accountType",
              selectedOption.type as AccountType,
            );
          }}
        />
        {renderCommonAccountView()}
        {account.accountType === AccountType.Bank && renderBankAccountView()}
        {account.accountType === AccountType.Debt && renderDebtAccountView()}
        {account.accountType === AccountType.Investment &&
          renderInvestmentAccountView()}
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
