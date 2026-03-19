// app/index.tsx
import React from "react";
import { Redirect } from "expo-router";

export default function Index() {
  console.log("In index");
  //return <Redirect href="./login-register/login.tsx" />;
  return <Redirect href="/login-register/login" />;
}
