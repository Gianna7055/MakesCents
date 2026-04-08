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

  getLabel: (item: T) => string;
  getValue: (item: T) => string;

  groupBy?: (item: T) => string | null;

  onChange: (item: T) => void;
  onBlur?: () => void;
};

function SingleDropdownInput<T>(props: Props<T>) {
  const [visible, setVisible] = useState(false);

  const selectedLabel = props.value ? props.getLabel(props.value) : "";

  const groupedItems = props.groupBy
    ? props.items.reduce(
        (acc, item) => {
          const group = props.groupBy!(item) || "__ungrouped";
          if (!acc[group]) acc[group] = [];
          acc[group].push(item);
          return acc;
        },
        {} as Record<string, T[]>,
      )
    : { __all: props.items };

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
              data={Object.entries(groupedItems)}
              keyExtractor={([groupName]) => groupName}
              renderItem={({ item: [groupName, items] }) => (
                <View>
                  {props.groupBy && groupName !== "__ungrouped" && (
                    <Text style={styles.categoryHeader}>{groupName}</Text>
                  )}

                  {items.map((item) => {
                    const isSelected =
                      props.value &&
                      props.getValue(props.value) === props.getValue(item);

                    return (
                      <TouchableOpacity
                        key={props.getValue(item)}
                        style={[
                          styles.option,
                          isSelected && styles.selectedOption,
                        ]}
                        onPress={() => handleSelect(item)}
                      >
                        <Text>{props.getLabel(item)}</Text>
                        {isSelected && <Ionicons name="checkmark" size={20} />}
                      </TouchableOpacity>
                    );
                  })}
                </View>
              )}
              onBlur={props.onBlur}
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
  categoryHeader: {
    fontWeight: "700",
    fontSize: 16,
    paddingVertical: 10,
    paddingHorizontal: 15,
    backgroundColor: "#f9f9f9",
  },
});
