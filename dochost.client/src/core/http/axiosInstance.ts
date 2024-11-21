import axios from "axios";

export const axiosInstance = axios.create();

export const attachToken = (token: string) => {
    axiosInstance.interceptors.request.use((config) => {
        config.headers['Authorization'] = `Bearer ${token}`;
        return config;
    }, error => {
        return Promise.reject(error);
    });
}
