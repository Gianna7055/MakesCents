export const handleBlur = <T extends Record<string, boolean | undefined>>(
  field: keyof T,
  setTouched: React.Dispatch<React.SetStateAction<T>>,
) => {
  setTouched((prev) => ({ ...prev, [field]: true }));
};

export const touchAll = <T extends Record<string, boolean | undefined>>(
  setTouched: React.Dispatch<React.SetStateAction<T>>,
) => {
  setTouched((prev) => {
    const all = (Object.keys(prev) as Array<keyof T>).reduce(
      (acc, key) => ({ ...acc, [key]: true }),
      {} as T,
    );
    return all;
  });
};
