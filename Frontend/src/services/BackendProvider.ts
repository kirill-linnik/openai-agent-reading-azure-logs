import axios, { AxiosRequestConfig } from "axios";

const requestConfig: AxiosRequestConfig = {
  baseURL: process.env.BACKEND_HOST_URL,
};

export const backendProvider = axios.create(requestConfig);

export function extractError(error: any): string {
  let messageStr = "";
  if (error) {
    if (error.response?.status) {
      messageStr += error.response.status + ": ";
    }
    if (error.message) {
      messageStr += error.message;
    } else if (error.response?.data) {
      messageStr += error.response.data;
    } else {
      messageStr = error as string;
    }
  }
  return messageStr;
}
