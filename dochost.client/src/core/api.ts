import {LoginInput} from "./contracts/auth.ts";

export const login = async (requestBody: LoginInput) => {
  const response = await fetch("login", {
      method: "POST",
      headers: {
          "Content-Type": "application/json"
      },
      body: JSON.stringify(requestBody)
  });

  return await response.json();
}