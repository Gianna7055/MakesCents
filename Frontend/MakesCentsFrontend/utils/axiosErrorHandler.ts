import { storage } from "@/data/storage";
import axios from "axios";
import { router } from "expo-router";

export const handleAxiosError = (
  error: any,
  onError?: (error: any) => void,
) => {
  console.log("Error:", error);
  if (axios.isAxiosError(error)) {
    console.log("Axios error:", error.response?.status, error.response?.data);
    if (error.response?.status === 401) {
      storage.removeBudgetId();
      storage.removeToken();
      router.replace("/login-register/login");
    }
    onError?.(error);
  } else {
    console.log("Error:", error);
    onError?.(error);
  }
};
