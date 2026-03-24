import React, { useEffect, useState } from "react";
import BottomNavBar from "@/components/bottom-nav-bar";
import {
  ScrollView,
  Text,
  Image,
  View,
  StyleSheet,
  Modal,
  TouchableOpacity,
} from "react-native";
import { storage } from "@/data/storage";
import axios, { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { GetBudgetResponse } from "@/types/get-budget-response";
import { GetBudgetDTOModel } from "@/types/get-budget-dto-model";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import EnvelopeCategoryCard from "@/components/budget/envelope-category-card";
import { Button } from "@/components/buttons/button";
import {
  formatCurrency,
  globalStyles,
  screenHeight,
  screenWidth,
} from "@/css/globalStyles";
import Animated, {
  runOnJS,
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from "react-native-reanimated";
import { router } from "expo-router";
import ScreenWrapper from "@/components/ui/screen-wrapper";

export default function Budget() {
  const [budgetId, setBudgetId] = useState<number>(0);
  const [budget, setBudget] = useState<GetBudgetDTOModel>();
  const [menuVisible, setMenuVisible] = useState<boolean>(false);

  // Budget constructor
  useEffect(() => {
    const main = async () => {
      // Load budget id from storage
      const storedBudgetId = await storage.getBudgetId();
      setBudgetId(storedBudgetId || 0);

      try {
        // Get the budget id from axios
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/budgets/${storedBudgetId}`,
        );

        // Get the response
        const response: GetBudgetResponse = axiosResponse.data;
        // Log the response
        //console.log("Response:", response);
        //console.log("Budget:", response.budget);
        //console.log("Envelope Categories:", response.budget.envelopeCategories);
        //console.log("Envelope 1:", response.budget.envelopeCategories[1]);
        if (response) {
          // Store the budget id in storage
          setBudget(response.budget);
          //console.log("Category 1", budget!.envelopeCategories[0]);
        }
      } catch (error: any) {
        if (axios.isAxiosError(error)) {
          console.log(
            "Axios error:",
            error.response?.status,
            error.response?.data,
          );
          if (error.response?.status == 401)
            router.replace("/login-register/login");
        } else {
          console.log("Error:", error);
        }
      }
    };

    // Main method
    main();
  }, []);

  // Total the budget
  const getTotalBudgetRemaining = (): number => {
    if (!budget) return 0;
    return budget.envelopeCategories.reduce((total, category) => {
      return total + getCategoryTotal(category);
    }, 0);
  };

  // Total a category
  const getCategoryTotal = (
    category: SummaryEnvelopeCategoryResponse,
  ): number => {
    return category.envelopes.reduce((total, envelope) => {
      return total + envelope.remainingAmount;
    }, 0);
  };

  const handleEllipsisClickEH = () => {
    // Open or close the modal
    if (menuVisible) closeMenu();
    else openMenu();
  };

  const handleCreateCategoryClickEH = () => {};

  const handleCreateEnvelopeClickEH = () => {};

  const handleEditCategoryClickEH = () => {};

  const translateY = useSharedValue(100);
  const opacity = useSharedValue(0);

  const openMenu = () => {
    setMenuVisible(true);
    translateY.value = withTiming(0, { duration: 200 });
    opacity.value = withTiming(1, { duration: 200 });
  };

  const closeMenu = () => {
    translateY.value = withTiming(100, { duration: 200 });
    opacity.value = withTiming(0, { duration: 200 }, (finished) => {
      if (finished) {
        runOnJS(setMenuVisible)(false);
      }
    });
  };

  const animatedStyle = useAnimatedStyle(() => ({
    transform: [{ translateY: translateY.value }],
    opacity: opacity.value,
  }));

  return (
    <ScreenWrapper>
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />
        <Text style={globalStyles.logoTitle}>Budget</Text>
      </View>
      <View style={styles.budgetStatusContainer}>
        <Text>Your Budget Makes Cents</Text>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={styles.logo}
        />
      </View>
      {budget ? ( // Block if budget exists
        <View style={styles.budgetNameContainer}>
          <View style={styles.budgetNameGroup}>
            <Text
              style={[globalStyles.centeredTitle, styles.budgetName]}
              numberOfLines={1}
              ellipsizeMode="tail"
            >
              {budget.budgetName}
            </Text>
            <Text style={globalStyles.centeredTitle}>▼</Text>
          </View>
          <Text style={globalStyles.centeredTitle}>
            {formatCurrency(getTotalBudgetRemaining())}
          </Text>
        </View>
      ) : (
        // Block if budget does not exist
        <Text style={globalStyles.centeredTitle}>Budget</Text>
      )}
      <ScrollView>
        {budget?.envelopeCategories.map((category) => (
          <EnvelopeCategoryCard
            key={category.envelopeCategoryId}
            envelopeCategory={category}
          />
        ))}
      </ScrollView>
      <Button
        name="..."
        onPress={handleEllipsisClickEH}
        containerStyle={{
          position: "absolute",
          right: screenWidth * 0.03,
          bottom: screenHeight * 0.115,
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
          lineHeight: 44,
          //marginBottom: 10, // To vertically center
          letterSpacing: 4,
        }}
        variant="primary"
      />
      <Modal
        visible={menuVisible}
        transparent={true}
        animationType="none"
        onRequestClose={handleEllipsisClickEH}
      >
        <TouchableOpacity
          style={{ flex: 1, backgroundColor: "rgba(0,0,0,0.5)" }}
          onPress={handleEllipsisClickEH}
        >
          <Animated.View style={[styles.modalView, animatedStyle]}>
            <Button
              name="Add new envelope category"
              onPress={handleCreateCategoryClickEH}
            ></Button>
            <Button
              name="Add new envelope"
              onPress={handleCreateEnvelopeClickEH}
            ></Button>
            <Button
              name="Edit categories"
              onPress={handleEditCategoryClickEH}
            ></Button>
          </Animated.View>
        </TouchableOpacity>
      </Modal>
      <BottomNavBar />
    </ScreenWrapper>
  );
}

const styles = StyleSheet.create({
  budgetStatusContainer: {
    backgroundColor: "#FFF",
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 5,
  },
  logo: {
    width: 30,
    height: 30,
    marginVertical: 4,
  },
  budgetNameContainer: {
    flexDirection: "row",
    paddingVertical: 7,
    justifyContent: "space-around",
    borderBottomWidth: 1,
    borderColor: "#B2B2B2",
    marginBottom: 15,
  },
  budgetNameGroup: {
    flexDirection: "row",
    justifyContent: "space-between",
  },
  budgetName: {
    width: screenWidth * 0.5,
  },
  modalView: {
    position: "absolute",
    bottom: screenHeight * 0.21, // slightly above the button
    right: screenWidth * 0.02,
    padding: 10,
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.25,
    shadowRadius: 4,
    elevation: 5,
    alignItems: "flex-end",
  },
});
