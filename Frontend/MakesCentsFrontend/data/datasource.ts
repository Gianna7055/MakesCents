import axios from "axios";
import { storage } from "../data/storage";

//export const makesCentsUrl: string = "http://172.24.79.93:5047"; // For Encanto LOPES
//export const makesCentsUrl: string = 'http://172.24.215.173:5047'; // For Engineering LOPES
export const makesCentsUrl: string = "http://172.20.10.7:5047"; // For Hot-spot
// export const makesCentsUrl: string = 'http://172.24.215.173:5047'; // For Pi

// Unauthenticated axios for login/register
export const makesCentsPublicAxios = axios.create({
  baseURL: makesCentsUrl,
  timeout: 10000,
  headers: { "Content-Type": "application/json" },
});

// Authenticated axios for everything else
const makesCentsAxios = axios.create({
  baseURL: makesCentsUrl,
  timeout: 10000,
  headers: { "Content-Type": "application/json" },
});

// Interceptor to automatically add the Bearer token
makesCentsAxios.interceptors.request.use(async (config) => {
  const token = await storage.getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default makesCentsAxios;
