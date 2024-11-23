import {LoginInput} from "../contracts/auth.ts";
import {axiosInstance} from "./axiosInstance.ts";
import {FileUploadInput} from "../contracts/document.ts";

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

export const register = async (requestBody: LoginInput) => {
    const response = await axiosInstance.post("api/register", requestBody, {
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

export const uploadFiles = async (formData: FileUploadInput, callback: () => void) => {
    const requestBody = new FormData();

    if (!formData.files) return;

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

    const response = await axiosInstance.get(`api/documents/${fileId}/download`, {
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
    const response = await axiosInstance.get(`api/documents/${fileId}/share`, {
        headers: {
            "Content-Type": "text/plain"
        }
    });

    if (response.status !== 200) {
        throw new Error("Network response was not ok")
    }

    return response.data;
}

export const getPreview = async (fileId?: string) => {
    if (!fileId) return;

    const response = await axiosInstance.get(`api/documents/${fileId}/preview`, {
        headers: {
            "Content-Type": "image/png"
        },
        responseType: "blob"
    });

    if (response.status !== 200) {
        throw new Error("Network response was not ok")
    }

    return response.data;
}