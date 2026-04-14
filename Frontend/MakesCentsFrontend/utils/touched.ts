export const handleBlur = <T extends Record<string, boolean | undefined>>(
  field: keyof T,
  setTouched: React.Dispatch<React.SetStateAction<T>>,
) => {
  setTouched((prev) => ({ ...prev, [field]: true }));
};
