import { Optional } from "@/types/optional";

/**
 * Wraps a value in an Optional<T> instance.
 * @param value The value to wrap. If null/undefined, hasValue=false
 */
export const toOptional = <T>(value: T | undefined | null): Optional<T> => {
  const opt = new Optional<T>();
  if (value != null) {
    opt.hasValue = true;
    opt.value = value;
  } else {
    opt.hasValue = false;
    // value is required by type, but ignored if hasValue=false
    opt.value = null as any;
  }
  return opt;
};

/**
 * Converts an Optional<T> instance into a plain value for your form.
 * @param optional Optional<T> instance
 * @param defaultValue Optional default value if hasValue=false
 * @returns T | null
 */
export const fromOptional = <T>(optional: Optional<T> | undefined | null, defaultValue: T | null = null): T | null => {
  if (optional && optional.hasValue) {
    return optional.value;
  }
  return defaultValue;
};