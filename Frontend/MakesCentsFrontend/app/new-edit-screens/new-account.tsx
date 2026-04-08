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
import { CreateDebtAccountResponse } from "@/types/create-debt-account-response";
import { CreateInvestmentAccountResponse } from "@/types/create-investment-account-response";
import { DebtAccountType } from "@/types/debt-account-type";
import { DebtPaymentRegularity } from "@/types/debt-payment-regularity";
import { InvestmentAccountType } from "@/types/investment-account-type";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { AccountForm, emptyAccountForm } from "@/utils/mappers/accountMapper";
import {
  createBankAccount,
  createDebtAccount,
  createInvestmentAccount,
} from "@/utils/new-edit-helpers/newEditAccountHelper";
import { handleBlur } from "@/utils/touched";
import { router } from "expo-router";
import React, { use, useEffect } from "react";
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

  const [touched, setTouched] = useState<
    Partial<Record<keyof AccountForm, boolean>>
  >({});
  const [formError, setFormError] = useState<string | null>(null);
  const [submitted, setSubmitted] = useState<boolean>(false);

  useEffect(() => {
    const main = async () => {
      // Load budget id from storage
      setBudgetId((await storage.getBudgetId()) || 0);
    };

    // Call main
    main();
    //console.log("New Account Budget Id:", budgetId);
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
    // Set the form to submitted
    setSubmitted(true);
    // Make sure the needed general info was sent
    if (
      !account.accountType ||
      !account.accountName ||
      !account.institution ||
      !account.balance
    )
      return;
    // Set up the try catch
    try {
      if (account.accountType === AccountType.Bank) {
        // Make sure the needed bank info was sent
        if (!account.bankAccountType) return;
        // Call the helper method ot create the bank account
        const response: CreateBankAccountResponse = await createBankAccount(
          account,
          budgetId,
        );
      } else if (account.accountType === AccountType.Debt) {
        // Make sure the needed debt info was sent
        if (!account.debtAccountType) return;
        // Call the helper method ot create the debt account
        const response: CreateDebtAccountResponse = await createDebtAccount(
          account,
          budgetId,
        );
      } else if (account.accountType === AccountType.Investment) {
        // Make sure the needed investment info was sent
        if (
          !account.investmentAccountType ||
          account.isTaxDeferred == null ||
          account.isTaxExempt == null
        )
          return;
        // Call the helper method ot create the investment account
        const response: CreateInvestmentAccountResponse =
          await createInvestmentAccount(account, budgetId);
      }
      // Go back to the accounts page
      router.replace("/accounts");
    } catch (error: any) {
      setFormError("Issue with creation");
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
      <View>
        <Input
          name="Account Name"
          placeholder="Name"
          type="text"
          value={account.accountName || ""}
          onChangeText={(text) => updateAccountForm("accountName", text)}
          autoCapitalize="words"
          onBlur={() => handleBlur("accountName", setTouched)}
        />
        {(touched.accountName || submitted) && !account.accountName && (
          <Text style={globalStyles.errorText}>Account name is required.</Text>
        )}
      </View>
    );
  };

  const renderInstitutionInput = () => {
    return (
      <View>
        <Input
          name="Institution"
          placeholder="Institution"
          type="text"
          value={account.institution || ""}
          onChangeText={(text) => updateAccountForm("institution", text)}
          autoCapitalize="words"
          onBlur={() => handleBlur("institution", setTouched)}
        />
        {(touched.institution || submitted) && !account.institution && (
          <Text style={globalStyles.errorText}>Institution is required.</Text>
        )}
      </View>
    );
  };

  const renderBalanceInput = () => {
    return (
      <View>
        <MoneyInput
          name="Balance"
          value={account.balance}
          onChangeValue={(amount) => updateAccountForm("balance", amount)}
          onBlur={() => handleBlur("balance", setTouched)}
        />
        {(touched.balance || submitted) && !account.balance && (
          <Text style={globalStyles.errorText}>Balance is required.</Text>
        )}
      </View>
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
          onBlur={() => handleBlur("bankAccountType", setTouched)}
        />
        {(touched.bankAccountType || submitted) && !account.bankAccountType && (
          <Text style={globalStyles.errorText}>
            Bank account type is required.
          </Text>
        )}
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
          onBlur={() => handleBlur("debtAccountType", setTouched)}
        />
        {(touched.debtAccountType || submitted) && !account.debtAccountType && (
          <Text style={globalStyles.errorText}>
            Debt Account Type is required.
          </Text>
        )}
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
          onBlur={() => handleBlur("investmentAccountType", setTouched)}
        />
        {(touched.investmentAccountType || submitted) &&
          !account.investmentAccountType && (
            <Text style={globalStyles.errorText}>
              Investment account type is required.
            </Text>
          )}
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
        onBlur={() => handleBlur("accountNumber", setTouched)}
      />
    );
  };

  const renderNextBillInputs = () => {
    const regularityOptions = Object.values(DebtPaymentRegularity).filter(
      (v) => typeof v === "number" && v != 1,
    );
    return (
      <View>
        <CalendarInput
          name="Date of Next Bill (Optional)"
          value={account.dateOfNextBill || null}
          onChange={(text) => updateAccountForm("dateOfNextBill", text)}
          onBlur={() => handleBlur("dateOfNextBill", setTouched)}
        />
        <MoneyInput
          name="Amount of Next Bill (Optional)"
          value={account.amountOfNextBill || 0}
          onChangeValue={(amount) =>
            updateAccountForm("amountOfNextBill", amount!)
          }
          onBlur={() => handleBlur("amountOfNextBill", setTouched)}
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
          onBlur={() => handleBlur("debtPaymentRegularity", setTouched)}
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
          onBlur={() => handleBlur("isTaxDeferred", setTouched)}
        />
        {(touched.isTaxDeferred || submitted) &&
          account.isTaxDeferred == null && (
            <Text style={globalStyles.errorText}>
              Tax deferred status is required.
            </Text>
          )}
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
          onBlur={() => handleBlur("isTaxExempt", setTouched)}
        />
        {(touched.isTaxExempt || submitted) && account.isTaxExempt == null && (
          <Text style={globalStyles.errorText}>
            Tax exempt status is required.
          </Text>
        )}
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
        {formError && <Text style={globalStyles.errorText}>{formError}</Text>}
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
