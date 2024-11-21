import {LoginInput} from "../contracts/auth.ts";
import {axiosInstance} from "./axiosInstance.ts";

export const login = async (requestBody: LoginInput) => {
    const response = await axiosInstance.post("api/login", requestBody, {
        headers: {
            "Content-Type": "application/json"
        }
    });

  if (response.status !== 200) {
      throw new Error("Network response was not ok")
  }

  return response.data;
}

export const fetchDocuments = async () => {
    const response = await axiosInstance.get("api/documents", {
        headers: {
            "Content-Type": "application/json"
        }
    });

    if (response.status !== 200) {
        throw new Error("Network response was not ok")
    }

    return response.data;
}