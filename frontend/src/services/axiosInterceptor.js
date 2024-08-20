import axios from "axios";
import config from "../config.json";

const API_URL = config.API_URL;

const axiosInstance = axios.create({
  baseURL: API_URL,
});

const configureAxiosInterceptors = async ({ token }) => {
  axiosInstance.interceptors.request.handlers = [];
  axiosInstance.interceptors.request.use((config) => {
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    } else {
      window.location.href = "/auth";
      const cancelTokenSource = axios.CancelToken.source();
      config.cancelToken = cancelTokenSource.token;
      return Promise.reject("Token not Set");
    }
    return config;
  });

  return Promise.resolve();
};

export { axiosInstance, configureAxiosInterceptors };
