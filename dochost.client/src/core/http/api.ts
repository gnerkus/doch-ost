import {LoginInput} from "../contracts/auth.ts";

export const login = async (requestBody: LoginInput) => {
  const response = await fetch("api/login", {
      method: "POST",
      headers: {
          "Content-Type": "application/json"
      },
      body: JSON.stringify(requestBody)
  });

  if (!response.ok) {
      throw new Error("Network response was not ok")
  }

  return await response.json();
}