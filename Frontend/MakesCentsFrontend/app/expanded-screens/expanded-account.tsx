import BottomNavBar from "@/components/bottom-nav-bar";
import { Button } from "@/components/buttons/button";
import CalendarInput from "@/components/text/calendar-input";
import IntInput from "@/components/text/int-input";
import MoneyInput from "@/components/text/money-input";
import RadioInput from "@/components/text/radio-input";
import SingleDropdownInput from "@/components/text/single-dropdown-input";
import Input from "@/components/text/text-input";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import makesCentsAxios from "@/data/datasource";
import { storage } from "@/data/storage";
import { AccountType } from "@/types/account-type";
import { BankAccountType } from "@/types/bank-account-type";
import { BaseIdResponse } from "@/types/base-id-response";
import { DebtAccountType } from "@/types/debt-account-type";
import { DebtPaymentRegularity } from "@/types/debt-payment-regularity";
import { GetBankAccountDTOModel } from "@/types/get-bank-account-dto-model";
import { GetBankAccountDTOResponse } from "@/types/get-bank-account-dto-response";
import { GetDebtAccountDTOModel } from "@/types/get-debt-account-dto-model";
import { GetDebtAccountDTOResponse } from "@/types/get-debt-account-dto-response";
import { GetInvestmentAccountDTOModel } from "@/types/get-investment-account-dto-model";
import { GetInvestmentAccountDTOResponse } from "@/types/get-investment-account-dto-response";
import { InvestmentAccountType } from "@/types/investment-account-type";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { AccountForm, emptyAccountForm } from "@/utils/mappers/accountMapper";
import { fromDateOnly } from "@/utils/mappers/dateOnlyMapper";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import {
  updateBankAccount,
  updateDebtAccount,
  updateInvestmentAccount,
} from "@/utils/new-edit-helpers/newEditAccountHelper";
import { handleBlur } from "@/utils/touched";
import { AxiosResponse } from "axios";
import { router, useLocalSearchParams } from "expo-router";
import React from "react";
import { useEffect, useState } from "react";
import { View, Image, Text, ScrollView, StyleSheet } from "react-native";

type ExpandedAccountProps = {
  paramAccountId: string;
  paramAccountType: string;
};

export default function ExpandedAccount() {
  console.log("URL Params:", useLocalSearchParams());
  const { paramAccountId, paramAccountType } =
    useLocalSearchParams<ExpandedAccountProps>();
  // Get the transaction id from the param
  const accountId = paramAccountId ? parseInt(paramAccountId) : null;
  // Get the transaction type from the param
  const accountType: AccountType = paramAccountType
    ? (parseInt(paramAccountType) as AccountType)
    : AccountType.Unknown;

  const [originalAccount, setOriginalAccount] = useState<
    | GetBankAccountDTOModel
    | GetDebtAccountDTOModel
    | GetInvestmentAccountDTOModel
    | null
  >(null);
  const [account, setAccount] = useState<AccountForm>(emptyAccountForm);
  const [budgetId, setBudgetId] = useState<number>(0);

  const [touched, setTouched] = useState<
    Partial<Record<keyof AccountForm, boolean>>
  >({});
  const [formError, setFormError] = useState<string | null>(null);

  useEffect(() => {
    const main = async () => {
      // Load budget id from storage
      setBudgetId((await storage.getBudgetId()) || 0);

      // Check which type of account was sent
      if (accountType === AccountType.Bank) {
        // Set up the try-catch
        try {
          // Call the API to get the bank account
          const axiosResponse: AxiosResponse = await makesCentsAxios.get(
            `/api/bank-accounts/${accountId}`,
          );

          // Get the response
          const response: GetBankAccountDTOResponse = JSON.parse(
            JSON.stringify(axiosResponse.data),
            jsonReviver,
          );
          // Log the response
          console.log("Get Bank Account Response:", response);

          // Get the bank account
          const bankAccount: GetBankAccountDTOModel = response.bankAccount;

          // Set the original account for update comparison
          setOriginalAccount(bankAccount);

          // Set up the account form for for display
          const accountForm: AccountForm = {
            // Base envelope props
            accountType: AccountType.Bank,
            accountName: bankAccount.accountName,
            institution: bankAccount.institution,
            balance: bankAccount.balance,

            // Bank account props
            bankAccountType: bankAccount.bankAccountType,
          };
          // Set the account form
          setAccount(accountForm);
        } catch (error: any) {
          handleAxiosError(error);
        }
      } else if (accountType === AccountType.Debt) {
        // Set up the try-catch
        try {
          // Call the API to get the debt account
          const axiosResponse: AxiosResponse = await makesCentsAxios.get(
            `/api/debt-accounts/${accountId}`,
          );

          // Get the response
          const response: GetDebtAccountDTOResponse = JSON.parse(
            JSON.stringify(axiosResponse.data),
            jsonReviver,
          );
          // Log the response
          console.log("Get Debt Account Response:", response);

          // Get the debt account
          const debtAccount: GetDebtAccountDTOModel = response.debtAccount;

          // Set the original account for update comparison
          setOriginalAccount(debtAccount);

          // Set up the account form for for display
          const accountForm: AccountForm = {
            // Base envelope props
            accountType: AccountType.Debt,
            accountName: debtAccount.accountName,
            institution: debtAccount.institution,
            balance: debtAccount.balance,

            // Debt account props
            debtAccountType: debtAccount.debtAccountType,
            dateOfNextBill: fromDateOnly(debtAccount.dateOfNextBill),
            amountOfNextBill: debtAccount.amountOfNextBill,
            debtPaymentRegularity: debtAccount.debtPaymentRegularity,

            // Shared debt/investment props
            accountNumber: debtAccount.accountNumber,
          };
          // Set the account form
          setAccount(accountForm);
        } catch (error: any) {
          handleAxiosError(error);
        }
      } else if (accountType === AccountType.Investment) {
        // Set up the try-catch
        try {
          // Call the API to get the investment account
          const axiosResponse: AxiosResponse = await makesCentsAxios.get(
            `/api/investment-accounts/${accountId}`,
          );

          // Get the response
          const response: GetInvestmentAccountDTOResponse = JSON.parse(
            JSON.stringify(axiosResponse.data),
            jsonReviver,
          );
          // Log the response
          console.log("Get Investment Account Response:", response);

          // Get the investment account
          const investmentAccount: GetInvestmentAccountDTOModel =
            response.investmentAccount;

          // Set the original account for update comparison
          setOriginalAccount(investmentAccount);

          // Set up the account form for for display
          const accountForm: AccountForm = {
            // Base envelope props
            accountType: AccountType.Investment,
            accountName: investmentAccount.accountName,
            institution: investmentAccount.institution,
            balance: investmentAccount.balance,

            // Investment account props
            investmentAccountType: investmentAccount.investmentAccountType,
            isTaxDeferred: investmentAccount.isTaxDeferred,
            isTaxExempt: investmentAccount.isTaxExempt,

            // Shared debt/investment props
            accountNumber: investmentAccount.accountNumber,
          };
          // Set the account form
          setAccount(accountForm);
        } catch (error: any) {
          handleAxiosError(error);
        }
      }
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
    try {
      setTouched({
        accountType: true,
        accountName: true,
        institution: true,
        balance: true,
      });
      if (account.accountType === AccountType.Bank) {
        setTouched({
          bankAccountType: true,
        });
        const response: BaseIdResponse = await updateBankAccount(
          account,
          originalAccount as GetBankAccountDTOModel,
        );
      } else if (account.accountType === AccountType.Debt) {
        setTouched({
          debtAccountType: true,
        });
        const response: BaseIdResponse = await updateDebtAccount(
          account,
          originalAccount as GetDebtAccountDTOModel,
        );
      } else if (account.accountType === AccountType.Investment) {
        setTouched({
          investmentAccountType: true,
          isTaxDeferred: true,
          isTaxExempt: true,
        });
        const response: BaseIdResponse = await updateInvestmentAccount(
          account,
          originalAccount as GetInvestmentAccountDTOModel,
        );
      }
      // Go back to the accounts list
      router.replace("/accounts");
    } catch (error: any) {
      handleAxiosError(error);
      setFormError("There was an error");
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
        onBlur={() => handleBlur("accountName", setTouched)}
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
        onBlur={() => handleBlur("institution", setTouched)}
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
        onBlur={() => handleBlur("balance", setTouched)}
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
          onBlur={() => handleBlur("bankAccountType", setTouched)}
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
          onBlur={() => handleBlur("debtAccountType", setTouched)}
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
          onBlur={() => handleBlur("investmentAccountType", setTouched)}
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
        onBlur={() => handleBlur("accountNumber", setTouched)}
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
          onBlur={() => handleBlur}
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
          <Text
            style={globalStyles.logoTitle}
            numberOfLines={2}
            ellipsizeMode="tail"
          >
            {account ? account.accountName : "Edit Account"}
          </Text>
        </View>
      </View>

      <ScrollView
        style={{ marginVertical: 0 }}
        contentContainerStyle={{ paddingBottom: 95 }}
      >
        <View style={globalStyles.settingsContainer}>
          <Text style={globalStyles.settingsTitle}>Settings</Text>

          {renderCommonAccountView()}
          {account.accountType === AccountType.Bank && renderBankAccountView()}
          {account.accountType === AccountType.Debt && renderDebtAccountView()}
          {account.accountType === AccountType.Investment &&
            renderInvestmentAccountView()}
        </View>
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
