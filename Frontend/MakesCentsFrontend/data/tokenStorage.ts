import AsyncStorage from '@react-native-async-storage/async-storage';

// Constant standardized key for storing the JWT (prevents typos)
const TOKEN_KEY = 'jwt';

/**
 * Save a JWT token to persistent storage
 *
 * @param token - JWT string returned from the API
 */
export const storeToken = async (token: string): Promise<void> => {
  try {
    await AsyncStorage.setItem(TOKEN_KEY, token);
    console.log('JWT stored successfully');
  } catch (error) {
    console.error('Error storing JWT:', error);
  }
};

/**
 * Retrieve the stored JWT token
 *
 * @returns the token string or null if not found
 */
export const getToken = async (): Promise<string | null> => {
  try {
    const token = await AsyncStorage.getItem(TOKEN_KEY);
    return token;
  } catch (error) {
    console.error('Error reading JWT:', error);
    return null;
  }
};

/**
 * Remove the stored JWT token (logout)
 */
export const removeToken = async (): Promise<void> => {
  try {
    await AsyncStorage.removeItem(TOKEN_KEY);
    console.log('JWT removed');
  } catch (error) {
    console.error('Error removing JWT:', error);
  }
};

// Single object export for all functions
export const tokenStorage = {
  saveToken: storeToken,
  getToken: getToken,
  removeToken: removeToken,
};
