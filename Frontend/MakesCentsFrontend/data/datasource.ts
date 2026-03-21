import axios from 'axios';
import { storage } from '../data/storage';

export const makesCentsUrl: string = 'http://172.24.215.173:5047';

// Unauthenticated axios for login/register
export const makesCentsPublicAxios = axios.create({
  baseURL: makesCentsUrl,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
});

// Authenticated axios for everything else
const makesCentsAxios = axios.create({
  baseURL: makesCentsUrl,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
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