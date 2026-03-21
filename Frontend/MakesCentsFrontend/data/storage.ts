// data/storage.ts
import AsyncStorage from '@react-native-async-storage/async-storage';

const KEYS = {
  token: 'token',
  budgetId: 'budgetId',
};

export const storage = {
  saveToken: (token: string) => AsyncStorage.setItem(KEYS.token, token),
  getToken: () => AsyncStorage.getItem(KEYS.token),
  removeToken: () => AsyncStorage.removeItem(KEYS.token),

  saveBudgetId: (id: number) => AsyncStorage.setItem(KEYS.budgetId, id.toString()),
  getBudgetId: async () => {
    const id = await AsyncStorage.getItem(KEYS.budgetId);
    return id ? parseInt(id) : null;
  },
  removeBudgetId: () => AsyncStorage.removeItem(KEYS.budgetId),
};