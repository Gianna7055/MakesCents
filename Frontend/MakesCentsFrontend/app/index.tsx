// app/index.tsx
import { Redirect } from "expo-router";
import React from "react";

export default function Index() {
  return <Redirect href='./login-register/login.tsx'/>
  //return <Redirect href="/login-sign-up/login" />;
}
