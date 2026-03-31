// MultiCategoryEnvelopeDropdown.tsx
import { Colors } from "@/constants/theme";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import React, { useState } from "react";
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Modal,
  FlatList,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import { SummaryEnvelopeCategoryResponse } from "@/types/summary-envelope-category-response";
import { EnvelopeSplit } from "@/utils/mappers/transactionMapper";

type Props = {
  name: string;
  categories: SummaryEnvelopeCategoryResponse[];
  selectedEnvelopes: EnvelopeSplit[];
  onChange: (splits: EnvelopeSplit[]) => void;
};

const MultiCategoryEnvelopeDropdown: React.FC<Props> = ({
  name,
  categories,
  selectedEnvelopes,
  onChange,
}) => {
  const [visible, setVisible] = useState(false);

  const toggleSelect = (envelope: EnvelopeSplit) => {
    if (selectedEnvelopes.find((e) => e.envelopeId === envelope.envelopeId)) {
      onChange(
        selectedEnvelopes.filter((e) => e.envelopeId !== envelope.envelopeId),
      );
    } else {
      onChange([...selectedEnvelopes, envelope]);
    }
  };

  return (
    <View style={styles.inputContainer}>
      {/* Label */}
      <Text style={globalStyles.textHeader}>{name}</Text>

      {/* Selected Chips */}
      {selectedEnvelopes.length > 0 && (
        <View style={styles.selectedContainer}>
          {selectedEnvelopes.map((env) => (
            <View key={env.envelopeId} style={styles.chip}>
              <Text style={styles.chipText}>{env.envelopeName}</Text>
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
                        onPress={() => toggleSelect(env)}
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

  // Selected chips
  selectedContainer: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: 6,
    marginBottom: 8,
  },
  chip: {
    backgroundColor: "#eee",
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 15,
  },
  chipText: {
    fontSize: 12,
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
});
