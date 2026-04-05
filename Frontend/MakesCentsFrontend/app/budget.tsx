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
  FlatList,
  ActivityIndicator,
} from "react-native";
import { storage } from "@/data/storage";
import { AxiosResponse } from "axios";
import makesCentsAxios from "@/data/datasource";
import { GetBudgetResponse } from "@/types/get-budget-response";
import { GetBudgetDTOModel } from "@/types/get-budget-dto-model";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import EnvelopeCategoryCard from "@/components/budget/envelope-category-card";
import { Button } from "@/components/buttons/button";
import { globalStyles, screenHeight, screenWidth } from "@/css/globalStyles";
import Animated, {
  runOnJS,
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from "react-native-reanimated";
import { router } from "expo-router";
import ScreenWrapper from "@/components/ui/screen-wrapper";
import { handleAxiosError } from "@/utils/axiosErrorHandler";
import { formatCurrency } from "@/utils/formatCurrency";
import { jsonReviver } from "@/utils/mappers/jsonReplacer";
import { GetYearBudgetsResponse } from "@/types/get-year-budgets-response";
import { GetYearBudgetDTOModel } from "@/types/get-year-budget-dto-model";
import { Month } from "@/types/month";

// A slot in the modal list — either a real budget or an empty month
type BudgetSlot =
  | { exists: true; budget: GetYearBudgetDTOModel }
  | { exists: false; year: number; month: number };

// Build 12 slots ending at (anchorYear, anchorMonth) inclusive,
// filling in real budgets where the API returned them.
const buildSlots = (
  anchorYear: number,
  anchorMonth: number,
  apiBudgets: GetYearBudgetDTOModel[],
): BudgetSlot[] => {
  const slots: BudgetSlot[] = [];

  for (let i = 0; i < 12; i++) {
    // Walk backwards from the anchor
    let month = anchorMonth - i;
    let year = anchorYear;
    while (month < 2) {
      month += 12;
      year -= 1;
    }

    const match = apiBudgets.find((b) => b.year === year && b.month === month);

    slots.push(
      match ? { exists: true, budget: match } : { exists: false, year, month },
    );
  }

  // Reverse so oldest is at the bottom, newest at the top
  return slots;
};

export default function Budget() {
  const [budgetId, setBudgetId] = useState<number>(0);
  const [budget, setBudget] = useState<GetBudgetDTOModel>();
  const [menuVisible, setMenuVisible] = useState<boolean>(false);
  // Consts for choosing a budget
  const [budgetsModalVisible, setBudgetsModalVisible] =
    useState<boolean>(false);
  const [envelopeCategoryModalVisible, setEnvelopeCategoryModalVisible] =
    useState<boolean>(false);
  const [slots, setSlots] = useState<BudgetSlot[]>([]);
  const [loadingMore, setLoadingMore] = useState<boolean>(false);
  // Tracks the oldest anchor fetched so "See More" knows where to continue from
  const [oldestAnchor, setOldestAnchor] = useState<{
    year: number;
    month: number;
  } | null>(null);
  const [emptySlot, setEmptySlot] = useState<{
    year: number;
    month: number;
  } | null>(null);

  const translateY = useSharedValue(100);
  const opacity = useSharedValue(0);

  const budgetTitle = budget
    ? budget.budgetName
    : emptySlot
      ? `${Month[emptySlot.month]} ${emptySlot.year}`
      : "Budget";

  // Budget constructor
  useEffect(() => {
    const main = async () => {
      // Load budget id from storage
      const storedBudgetId = await storage.getBudgetId();
      setBudgetId(storedBudgetId || 0);

      try {
        // Get the budget from axios
        const axiosResponse: AxiosResponse = await makesCentsAxios.get(
          `/api/budgets/${storedBudgetId}`,
        );

        // Get the response
        const response: GetBudgetResponse = JSON.parse(
          JSON.stringify(axiosResponse.data),
          jsonReviver,
        );
        // Log the response
        //console.log("Response:", response);
        //console.log("Budget:", response.budget);
        //console.log("Envelope Categories:", response.budget.envelopeCategories);
        //console.log("Envelope 1:", response.budget.envelopeCategories[1]);
        if (response) {
          // Store the budget id in storage
          response.budget.envelopeCategories =
            response.budget.envelopeCategories.sort(
              (a, b) => a.envelopeCategoryId - b.envelopeCategoryId,
            );
          response.budget.envelopeCategories =
            response.budget.envelopeCategories
              .sort((a, b) => a.envelopeCategoryId - b.envelopeCategoryId)
              .map((category) => ({
                ...category,
                envelopes: category.envelopes.sort(
                  (a, b) => a.envelopeId - b.envelopeId,
                ),
              }));
          setBudget(response.budget);
          //console.log("Category 1", budget!.envelopeCategories[0]);
        }
      } catch (error: any) {
        handleAxiosError(error);
      }
    };

    // Main method
    main();
  }, [budgetId]);

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

  const handleCreateCategoryClickEH = () => {
    closeMenu();
    router.push(
      `/new-edit-screens/new-edit-envelope-category?budgetId=${budgetId}`,
    );
  };

  const handleCreateEnvelopeClickEH = () => {
    closeMenu();
    router.push(`/new-edit-screens/new-envelope`);
  };

  const handleEditCategoryClickEH = () => {
    closeMenu();
    setTimeout(() => setEnvelopeCategoryModalVisible(true), 250);
  };

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

  // Fetches one page of budgets anchored at (year, month) and returns the slots
  const fetchSlots = async (
    year: number,
    month: number,
  ): Promise<BudgetSlot[]> => {
    const axiosResponse: AxiosResponse = await makesCentsAxios.get(
      `/api/budgets/all/year/${year}/month/${month}`,
    );
    const response: GetYearBudgetsResponse = JSON.parse(
      JSON.stringify(axiosResponse.data),
      jsonReviver,
    );
    //console.log("Fetched budgets for", month, year, ":", response.budgets);
    return buildSlots(year, month, response.budgets);
  };

  const openBudgetModal = async () => {
    setBudgetsModalVisible(true);

    try {
      const now = new Date();
      const anchorYear = now.getFullYear();
      const anchorMonth = now.getMonth() + 2;

      const initialSlots = await fetchSlots(anchorYear, anchorMonth);
      setSlots(initialSlots);
      setOldestAnchor({ year: anchorYear, month: anchorMonth });
    } catch (error) {
      console.log("Error fetching budgets:", error);
    }
  };

  const handleSeeMore = async () => {
    if (!oldestAnchor || loadingMore) return;
    setLoadingMore(true);

    try {
      const moreSlots = await fetchSlots(
        oldestAnchor.year - 1,
        oldestAnchor.month,
      );
      setSlots((prev) => [...prev, ...moreSlots]);

      const oldest = moreSlots[moreSlots.length - 1];
      setOldestAnchor(
        oldest.exists
          ? { year: oldest.budget.year, month: oldest.budget.month }
          : { year: oldest.year, month: oldest.month },
      );
    } catch (error) {
      console.log("Error fetching more budgets:", error);
    } finally {
      setLoadingMore(false);
    }
  };

  const handleSelectBudget = async (item: GetYearBudgetDTOModel) => {
    await storage.saveBudgetId(item.budgetId);
    setBudgetId(item.budgetId);
    //console.log("Stored budget Id:", await storage.getBudgetId());
    //console.log("Budget Id:", budgetId);
    setEmptySlot(null);
    setBudgetsModalVisible(false);
  };

  const handleSelectEmptySlot = (year: number, month: number) => {
    setBudgetsModalVisible(false);
    setBudget(undefined);
    setEmptySlot({ year, month });
  };

  const renderSlot = ({ item }: { item: BudgetSlot }) => {
    if (item.exists) {
      return (
        <TouchableOpacity
          style={styles.budgetItem}
          onPress={() => handleSelectBudget(item.budget)}
        >
          <Text>
            {Month[item.budget.month]} {item.budget.year}
          </Text>
          <Text>{item.budget.budgetName}</Text>
        </TouchableOpacity>
      );
    }

    return (
      <TouchableOpacity
        style={[styles.budgetItem, styles.budgetItemEmpty]}
        onPress={() => handleSelectEmptySlot(item.year, item.month)}
      >
        <Text style={styles.emptyMonthText}>
          {Month[item.month]} {item.year}
        </Text>
        <Text style={styles.noBudgetText}>No Budget Yet</Text>
      </TouchableOpacity>
    );
  };

  return (
    <ScreenWrapper>
      <View style={globalStyles.noWordsLogoContainer}>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={globalStyles.noWordsLogo}
        />

        <View style={globalStyles.logoTitleContainer}>
          <Text style={globalStyles.logoTitle}>Budget</Text>
        </View>
      </View>
      <View style={styles.budgetStatusContainer}>
        <Text>Your Budget Makes Cents</Text>
        <Image
          source={require("@/assets/images/MakesCentsLogo.png")}
          style={styles.logo}
        />
      </View>
      <View style={styles.budgetNameContainer}>
        <TouchableOpacity onPress={openBudgetModal}>
          <View style={styles.budgetNameGroup}>
            <Text
              style={[globalStyles.centeredTitle, styles.budgetName]}
              numberOfLines={1}
              ellipsizeMode="tail"
            >
              {budgetTitle}
            </Text>
            <Text style={globalStyles.centeredTitle}>▼</Text>
          </View>
        </TouchableOpacity>
        <Text style={globalStyles.centeredTitle}>
          {budget ? formatCurrency(getTotalBudgetRemaining()) : ""}
        </Text>
      </View>
      {budget ? (
        <ScrollView>
          {budget.envelopeCategories.map((category) => (
            <EnvelopeCategoryCard
              key={category.envelopeCategoryId}
              envelopeCategory={category}
            />
          ))}
        </ScrollView>
      ) : (
        <View style={styles.noBudgetContainer}>
          <Text style={styles.noBudgetScreenText}>No Budget Exists Yet</Text>
          <Button
            name="Create Budget"
            onPress={() =>
              router.push(
                `/new-edit-screens/new-budget?year=${emptySlot?.year}&month=${emptySlot?.month}`,
              )
            }
            variant="primary"
          />
        </View>
      )}
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

      {/* Ellipsis menu modal */}
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
          <Animated.View
            style={[styles.modalView, animatedStyle]}
            onStartShouldSetResponder={() => true}
          >
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

      {/* Budget picker modal */}
      <Modal visible={budgetsModalVisible} transparent animationType="slide">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={[globalStyles.centeredTitle, { marginBottom: 15 }]}>
              Select a Budget
            </Text>
            <FlatList
              data={slots}
              keyExtractor={(item, index) =>
                item.exists
                  ? item.budget.budgetId.toString()
                  : `empty-${item.year}-${item.month}-${index}`
              }
              renderItem={renderSlot}
              ListFooterComponent={
                <TouchableOpacity
                  style={styles.seeMoreButton}
                  onPress={handleSeeMore}
                  disabled={loadingMore}
                >
                  {loadingMore ? (
                    <ActivityIndicator size="small" />
                  ) : (
                    <Text style={styles.seeMoreText}>See More</Text>
                  )}
                </TouchableOpacity>
              }
            />
            <TouchableOpacity onPress={() => setBudgetsModalVisible(false)}>
              <Text style={styles.closeText}>Close</Text>
            </TouchableOpacity>
          </View>
        </View>
      </Modal>

      {/* Envelope category edit picker modal */}
      <Modal
        visible={envelopeCategoryModalVisible}
        transparent
        animationType="slide"
      >
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={[globalStyles.centeredTitle, { marginBottom: 15 }]}>
              Select an Envelope Category to Edit
            </Text>
            <FlatList
              data={budget?.envelopeCategories || []}
              keyExtractor={(item) => item.envelopeCategoryId.toString()}
              renderItem={({ item }) => (
                <TouchableOpacity
                  style={styles.budgetItem}
                  onPress={() => {
                    setEnvelopeCategoryModalVisible(false);
                    router.push(
                      `/new-edit-screens/new-edit-envelope-category?budgetId=${item.budgetId}&envelopeCategoryId=${item.envelopeCategoryId}`,
                    );
                  }}
                >
                  <Text>{item.envelopeCategoryName}</Text>
                </TouchableOpacity>
              )}
            />
            <TouchableOpacity
              onPress={() => setEnvelopeCategoryModalVisible(false)}
            >
              <Text style={styles.closeText}>Close</Text>
            </TouchableOpacity>
          </View>
        </View>
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
    gap: 5,
  },
  budgetName: {
    maxWidth: screenWidth * 0.5,
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
  modalOverlay: {
    flex: 1,
    justifyContent: "center",
    backgroundColor: "rgba(0,0,0,0.5)",
  },
  modalContent: {
    margin: 20,
    backgroundColor: "white",
    borderRadius: 10,
    padding: 20,
    maxHeight: "70%",
  },
  budgetItem: {
    padding: 15,
    borderBottomWidth: 1,
    borderColor: "#eee",
    flexDirection: "row",
    justifyContent: "space-between",
  },
  budgetItemEmpty: {
    opacity: 0.5,
  },
  emptyMonthText: {
    color: "#555",
  },
  noBudgetText: {
    color: "#aaa",
    fontStyle: "italic",
  },
  seeMoreButton: {
    paddingVertical: 12,
    alignItems: "center",
    borderBottomWidth: 1,
    borderColor: "#eee",
    marginBottom: 4,
  },
  seeMoreText: {
    color: "#007AFF",
    fontWeight: "600",
  },
  closeText: {
    textAlign: "center",
    marginTop: 10,
  },
  noBudgetContainer: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    gap: 15,
  },
  noBudgetScreenText: {
    fontSize: 16,
    color: "#555",
  },
});
