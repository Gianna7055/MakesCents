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

type Props<T> = {
  name: string;
  value: T | null;
  items: T[];
  placeholder?: string;

  // 👇 key part
  getLabel: (item: T) => string;
  getValue: (item: T) => string;

  onChange: (item: T) => void;
};

function SingleDropdownInput<T>(props: Props<T>) {
  const [visible, setVisible] = useState(false);

  const selectedLabel = props.value ? props.getLabel(props.value) : "";

  const handleSelect = (item: T) => {
    props.onChange(item);
    setVisible(false);
  };

  return (
    <View style={styles.inputContainer}>
      <Text style={globalStyles.textHeader}>{props.name}</Text>

      <TouchableOpacity style={styles.input} onPress={() => setVisible(true)}>
        <Text style={{ color: selectedLabel ? "#000" : "#999" }}>
          {selectedLabel || props.placeholder || "Select an option"}
        </Text>

        <Ionicons name="chevron-down" size={20} />
      </TouchableOpacity>

      <Modal visible={visible} transparent animationType="fade">
        <View style={styles.overlay}>
          <View style={styles.modalContent}>
            {/* Header */}
            <View style={styles.header}>
              <Text style={styles.title}>{props.name}</Text>

              <TouchableOpacity onPress={() => setVisible(false)}>
                <Ionicons name="close" size={24} />
              </TouchableOpacity>
            </View>

            <FlatList
              data={props.items}
              keyExtractor={(item) => props.getValue(item)}
              renderItem={({ item }) => {
                const isSelected =
                  props.value &&
                  props.getValue(props.value) === props.getValue(item);

                return (
                  <TouchableOpacity
                    style={[styles.option, isSelected && styles.selectedOption]}
                    onPress={() => handleSelect(item)}
                  >
                    <Text>{props.getLabel(item)}</Text>

                    {isSelected && <Ionicons name="checkmark" size={20} />}
                  </TouchableOpacity>
                );
              }}
            />
          </View>
        </View>
      </Modal>
    </View>
  );
}

export default SingleDropdownInput;

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

  // Modal styles
  overlay: {
    flex: 1,
    backgroundColor: "rgba(0,0,0,0.4)", // 👈 dark background
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

  option: {
    padding: 15,
  },
  selectedOption: {
    backgroundColor: "#f2f2f2",
  },
  optionText: {
    fontSize: 16,
  },
});
