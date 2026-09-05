import { storage } from "@/data/storage";
import axios from "axios";
import { router } from "expo-router";

export const handleAxiosError = (
  error: any,
  onError?: (error: any) => void,
) => {
  // Grab the call stack right here, before anything else runs
  const callerStack = new Error().stack
    ?.split("\n")
    .slice(2, 4) // skip "Error" line and this function's own frame
    .join("\n");
  console.log("Error occurred in:\n", callerStack);

  console.log("Error:", error);
  if (axios.isAxiosError(error)) {
    console.log(
      "Axios error:",
      error.code, // e.g. "ECONNABORTED" for timeouts
      error.response?.status,
      error.response?.data,
    );
    console.log(
      "Failed request:",
      error.config?.method?.toUpperCase(),
      error.config?.url,
    );

    if (error.response?.status === 401) {
      storage.removeBudgetId();
      storage.removeToken();
      router.replace("/login-register/login");
    }
    onError?.(error);
  } else {
    onError?.(error);
  }
};
