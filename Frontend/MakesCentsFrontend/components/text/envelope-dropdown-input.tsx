// MultiCategoryEnvelopeDropdown.tsx
import { Colors } from "@/constants/theme";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import React, { useState, useEffect, useRef } from "react";
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Modal,
  FlatList,
  TextInput,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import { EnvelopeSplit } from "@/utils/mappers/transactionMapper";

type Props = {
  name: string;
  categories: SummaryEnvelopeCategoryResponse[];
  selectedEnvelopes: EnvelopeSplit[];
  onChange: (splits: EnvelopeSplit[]) => void;
  totalAmount: number;
};

// Format cents to string with commas and 2 decimals
const formatCentsToCurrency = (cents: number) => {
  const dollars = cents / 100;
  return dollars.toLocaleString(undefined, {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
};

const MultiCategoryEnvelopeDropdown: React.FC<Props> = ({
  name,
  categories,
  selectedEnvelopes,
  onChange,
  totalAmount,
}) => {
  const [visible, setVisible] = useState(false);
  const [editingIndex, setEditingIndex] = useState<number | null>(null);
  const [centsArray, setCentsArray] = useState<number[]>(
    selectedEnvelopes.map((env) => Math.round(env.amount * 100)),
  );
  const totalCents = Math.abs(Math.round(totalAmount * 100));
  const selectedTotalCents = centsArray.reduce((sum, c) => sum + c, 0);
  const remainingCents = totalCents - selectedTotalCents;
  const showRemaining = selectedEnvelopes.length > 1;
  const prevLengthRef = useRef(selectedEnvelopes.length);

  // Sync external selectedEnvelopes → internal cents array
  useEffect(() => {
    setCentsArray(
      selectedEnvelopes.map((env) => Math.round(Math.abs(env.amount) * 100)),
    );
  }, [selectedEnvelopes]);

  useEffect(() => {
    const prevLength = prevLengthRef.current;
    const currentLength = selectedEnvelopes.length;

    // 1 → multiple → zero everything
    if (prevLength === 1 && currentLength > 1) {
      const updated = selectedEnvelopes.map((env) => ({
        ...env,
        amount: 0,
      }));
      onChange(updated);
    }

    // multiple → 1 → set full amount
    if (currentLength === 1) {
      const updated = [
        {
          ...selectedEnvelopes[0],
          amount: totalAmount,
        },
      ];
      onChange(updated);
    }

    prevLengthRef.current = currentLength;
  }, [selectedEnvelopes.length, totalAmount]);

  const toggleSelect = (envelope: EnvelopeSplit) => {
    if (selectedEnvelopes.find((e) => e.envelopeId === envelope.envelopeId)) {
      onChange(
        selectedEnvelopes.filter((e) => e.envelopeId !== envelope.envelopeId),
      );
    } else {
      onChange([...selectedEnvelopes, envelope]);
    }
  };

  const handleAmountChange = (index: number, text: string) => {
    // Remove all non-digit characters
    const digits = text.replace(/\D/g, "");
    const newCents = digits ? parseInt(digits, 10) : 0;

    const newCentsArray = [...centsArray];
    newCentsArray[index] = newCents;
    setCentsArray(newCentsArray);

    const updatedEnvelopes = [...selectedEnvelopes];
    updatedEnvelopes[index] = {
      ...updatedEnvelopes[index],
      amount:
        totalAmount < 0 ? -Math.abs(newCents / 100) : Math.abs(newCents / 100),
    };
    onChange(updatedEnvelopes);
  };

  return (
    <View style={styles.inputContainer}>
      {/* Label */}
      <View style={styles.headerRow}>
        <Text style={globalStyles.textHeader}>{name}</Text>

        {showRemaining && (
          <View style={styles.remainingContainer}>
            <Text style={styles.remainingLabel}>Remaining</Text>
            <Text
              style={[
                styles.remainingText,
                remainingCents < 0 && styles.remainingNegative,
              ]}
            >
              ${formatCentsToCurrency(remainingCents)}
            </Text>
          </View>
        )}
      </View>

      {/* Selected Envelopes */}
      {selectedEnvelopes.length > 0 && (
        <View style={styles.selectedContainer}>
          {selectedEnvelopes.map((env, index) => (
            <View key={env.envelopeId} style={styles.selectedRow}>
              {/* Minus Button */}
              <TouchableOpacity
                onPress={() =>
                  onChange(
                    selectedEnvelopes.filter(
                      (e) => e.envelopeId !== env.envelopeId,
                    ),
                  )
                }
                style={styles.removeButton}
              >
                <Text style={styles.removeButtonText}>-</Text>
              </TouchableOpacity>

              {/* Envelope Name */}
              <Text
                style={styles.envelopeName}
                numberOfLines={1}
                ellipsizeMode="tail"
              >
                {env.envelopeName}
              </Text>

              {/* Amount Input (ONLY if multiple) */}
              {showRemaining && (
                <View style={styles.amountWrapper}>
                  <Text style={styles.dollarSign}>$</Text>
                  <TextInput
                    style={[
                      styles.amountInput,
                      editingIndex === index && styles.amountInputActive,
                    ]}
                    keyboardType="number-pad"
                    value={formatCentsToCurrency(centsArray[index] || 0)}
                    onFocus={() => setEditingIndex(index)}
                    onBlur={() => setEditingIndex(null)}
                    onChangeText={(text) => handleAmountChange(index, text)}
                  />
                </View>
              )}
            </View>
          ))}
        </View>
      )}

      {/* Open Dropdown */}
      <TouchableOpacity
        style={styles.input}
        onPress={() => setVisible(true)}
        activeOpacity={0.7}
      >
        <Text style={{ color: selectedEnvelopes.length ? "#000" : "#999" }}>
          {selectedEnvelopes.length ? "Edit selection" : "Select envelopes"}
        </Text>
        <Ionicons name="chevron-down" size={20} />
      </TouchableOpacity>

      {/* Modal */}
      <Modal visible={visible} transparent animationType="fade">
        <View style={styles.overlay}>
          <View style={styles.modalContent}>
            {/* Header */}
            <View style={styles.header}>
              <Text style={styles.title}>{name}</Text>
              <TouchableOpacity onPress={() => setVisible(false)}>
                <Text style={styles.doneText}>Done</Text>
              </TouchableOpacity>
            </View>

            {/* Category + Envelopes List */}
            <FlatList
              data={categories}
              keyExtractor={(cat) => cat.envelopeCategoryId.toString()}
              renderItem={({ item: category }) => (
                <View>
                  {/* Category Subheading */}
                  <Text style={styles.categoryHeader}>
                    {category.envelopeCategoryName}
                  </Text>

                  {/* Envelopes */}
                  {category.envelopes.map((env) => {
                    const selected = selectedEnvelopes.some(
                      (e) => e.envelopeId === env.envelopeId,
                    );

                    return (
                      <TouchableOpacity
                        key={env.envelopeId}
                        style={[
                          styles.option,
                          selected && styles.selectedOption,
                        ]}
                        onPress={() =>
                          toggleSelect({
                            envelopeId: env.envelopeId,
                            envelopeName: env.envelopeName,
                            amount:
                              selectedEnvelopes.find(
                                (e) => e.envelopeId === env.envelopeId,
                              )?.amount || 0,
                          })
                        }
                      >
                        <Text style={styles.optionText}>
                          {env.envelopeName}
                        </Text>
                        {selected && <Ionicons name="checkmark" size={20} />}
                      </TouchableOpacity>
                    );
                  })}
                </View>
              )}
            />
          </View>
        </View>
      </Modal>
    </View>
  );
};

export default MultiCategoryEnvelopeDropdown;

// -------------------- Styles --------------------
const styles = StyleSheet.create({
  inputContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingTop: screenWidth * 0.05,
    backgroundColor: Colors.light.background,
  },
  input: {
    paddingVertical: 15,
    paddingHorizontal: 10,
    backgroundColor: "#FFF",
    borderRadius: 10,
    borderColor: "#C0C0C0",
    borderWidth: 1,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },

  // Selected Envelopes
  selectedContainer: {
    marginBottom: 8,
  },
  selectedRow: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    paddingVertical: 8,
    marginBottom: 6,
  },
  removeButton: {
    width: 24,
    height: 24,
    borderRadius: 12,
    backgroundColor: "#000",
    justifyContent: "center",
    alignItems: "center",
    marginRight: 10,
  },
  removeButtonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "600",
  },
  envelopeName: {
    flex: 1,
    fontSize: 14,
    paddingRight: 10,
  },
  amountWrapper: {
    flexDirection: "row",
    alignItems: "center",
    position: "relative",
  },
  dollarSign: {
    position: "absolute",
    left: 0,
    fontSize: 16,
    color: "#555",
    zIndex: 1,
  },
  amountInput: {
    width: 100,
    paddingLeft: 16,
    paddingVertical: 2,
    borderBottomWidth: 1,
    borderBottomColor: "#ccc",
    textAlign: "right",
    fontSize: 16,
    color: "#000",
    backgroundColor: "transparent",
  },
  amountInputActive: {
    borderBottomColor: "#555",
  },

  // Modal
  overlay: {
    flex: 1,
    backgroundColor: "rgba(0,0,0,0.4)",
    justifyContent: "center",
    paddingHorizontal: 20,
  },
  modalContent: {
    backgroundColor: "#FFF",
    borderRadius: 12,
    maxHeight: "70%",
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    padding: 15,
    borderBottomWidth: 1,
    borderBottomColor: "#eee",
  },
  title: {
    fontSize: 18,
    fontWeight: "600",
  },
  doneText: {
    color: "#007AFF",
    fontWeight: "600",
  },

  // Category & envelope
  categoryHeader: {
    fontWeight: "700",
    fontSize: 16,
    paddingVertical: 10,
    paddingHorizontal: 15,
    backgroundColor: "#f9f9f9",
  },
  option: {
    paddingVertical: 12,
    paddingHorizontal: 15,
    flexDirection: "row",
    justifyContent: "space-between",
  },
  selectedOption: {
    backgroundColor: "#f2f2f2",
  },
  optionText: {
    fontSize: 15,
  },

  // Header Row
  headerRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },

  remainingContainer: {
    alignItems: "flex-end",
  },

  remainingLabel: {
    fontSize: 12,
    color: "#888",
  },

  remainingText: {
    fontSize: 15,
    fontWeight: "normal",
    color: "#555",
  },

  remainingNegative: {
    color: "#FF3B30",
  },
});
