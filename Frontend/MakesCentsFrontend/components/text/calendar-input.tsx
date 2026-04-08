import { Colors } from "@/constants/theme";
import { globalStyles, screenWidth } from "@/css/globalStyles";
import { Ionicons } from "@expo/vector-icons";
import React, { forwardRef, useState } from "react";
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  Platform,
} from "react-native";
import DateTimePickerModal from "react-native-modal-datetime-picker";

type InputProps = {
  name: string;
  value: Date | null;
  onChange: (date: Date) => void;
  placeholder?: string;
  returnKeyType?: "next" | "done" | "go" | "search" | "send";
  onSubmitEditing?: () => void;
  onBlur?: () => void;
};

const CalendarInput = forwardRef<TextInput, InputProps>((props, ref) => {
  const [isPickerVisible, setPickerVisible] = useState(false);
  const [inputValue, setInputValue] = useState(
    props.value ? formatDate(props.value) : "",
  );

  const showPicker = () => setPickerVisible(true);
  const hidePicker = () => setPickerVisible(false);

  const handleConfirm = (date: Date) => {
    const formatted = formatDate(date);
    setInputValue(formatted);
    props.onChange?.(date);
    hidePicker();
  };

  const handleTextChange = (text: string) => {
    setInputValue(text);

    const parsed = parseDate(text);
    if (parsed) {
      props.onChange?.(parsed);
    }
  };

  return (
    <View style={styles.inputContainer}>
      <Text style={globalStyles.textHeader}>{props.name}</Text>
      <View style={styles.container}>
        <TextInput
          style={styles.input}
          value={inputValue}
          onChangeText={handleTextChange}
          placeholder={props.placeholder || "MM/DD/YYYY"}
          keyboardType="numeric"
          onBlur={props.onBlur}
        />

        <TouchableOpacity onPress={showPicker} style={styles.icon}>
          <Ionicons name="calendar-outline" size={22} />
        </TouchableOpacity>

        <DateTimePickerModal
          isVisible={isPickerVisible}
          mode="date"
          onConfirm={handleConfirm}
          onCancel={hidePicker}
          date={props.value || new Date()}
        />
      </View>
    </View>
  );
});

export default CalendarInput;

const styles = StyleSheet.create({
  inputContainer: {
    paddingHorizontal: screenWidth * 0.08,
    paddingTop: screenWidth * 0.05,
  },
  container: {
    flexDirection: "row",
    alignItems: "center",
    paddingHorizontal: 10,
    paddingLeft: screenWidth * 0.02,
    backgroundColor: "#FFF",
    borderRadius: 10,
    borderColor: "#C0C0C0",
    borderWidth: 1,
  },
  input: {
    flex: 1,
    height: 45,
    paddingLeft: 5,
  },
  icon: {
    padding: 5,
  },
});

const formatDate = (date: Date) => {
  //console.log("In formatDate");
  const mm = String(date.getMonth() + 1).padStart(2, "0");
  const dd = String(date.getDate()).padStart(2, "0");
  const yyyy = date.getFullYear();
  return `${mm}/${dd}/${yyyy}`;
};

const parseDate = (text: string): Date | null => {
  const parts = text.split("/");
  if (parts.length !== 3) return null;

  const [mm, dd, yyyy] = parts.map(Number);
  const date = new Date(yyyy, mm - 1, dd);

  return isNaN(date.getTime()) ? null : date;
};
