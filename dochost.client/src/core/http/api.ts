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

export const uploadFiles = async (formData, callback) => {
    const requestBody = new FormData();

    for (const formDatum of formData.files) {
        requestBody.append("formFiles", formDatum);
    }

    const response = await axiosInstance.post("api/documents/upload", requestBody, {
        headers: {
            "Content-Type": "multipart/form-data"
        }
    });

    if (response.status !== 200) {
        throw new Error("Network response was not ok")
    }

    callback();
}

export const downloadFile = async (fileId: string, fileExt: string, fileName: string) => {
    let contentType = "image/png"

    switch (fileExt) {
        case ".pdf":
            contentType = "application/pdf"
            break;
        case ".jpg":
            contentType = "image/jpeg"
            break;
    }

    const response = await axiosInstance.get(`api/documents/download/${fileId}`, {
        headers: {
            "Content-Type": contentType
        },
        responseType: "blob"
    });

    if (response.status !== 200) {
        throw new Error("Network response was not ok")
    }

    const url = window.URL.createObjectURL(new Blob([response.data], {type: contentType}));
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName || "downloaded-file";
    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
}

export const getShareableLink = async (fileId: string) => {
    const response = await axiosInstance.get(`api/documents/share/${fileId}`, {
        headers: {
            "Content-Type": "text/plain"
        }
    });

    if (response.status !== 200) {
        throw new Error("Network response was not ok")
    }

    return response.data;
}