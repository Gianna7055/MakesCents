import axios from 'axios';

export const makesCentsUrl: string = 'http://172.24.215.173:5047';

const makesCentsAxios = axios.create({
  baseURL: makesCentsUrl,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
});

export default makesCentsAxios;