import axios from "axios";
import { storage } from "../data/storage";

//export const makesCentsUrl: string = "http://172.24.79.93:5047"; // For LOPES: Encanto
//export const makesCentsUrl: string = 'http://172.24.215.173:5047'; // For LOPES: Engineering
//export const makesCentsUrl: string = "http://172.20.10.7:5047"; // For Hot-spot
// export const makesCentsUrl: string = 'http://172.24.215.173:5047'; // For Pi
//export const makesCentsUrl: string = "http://172.24.79.93:5047"; // For: LOPES Oak Creek
export const makesCentsUrl: string = "http://192.168.4.106:5047"; // For Home: Plans4you

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
