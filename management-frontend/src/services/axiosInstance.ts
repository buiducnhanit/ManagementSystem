import axios from "axios";
import { refreshTokenAsync } from "./authService";
import API_BASE_URL from "../utils/constants";

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: { "Content-Type": "application/json" }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem("token") || sessionStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(
    response => response,
    async error => {
        const originalRequest = error.config;
        if (
            error.response &&
            error.response.status === 401 &&
            !originalRequest._retry
        ) {
            originalRequest._retry = true;
            try {
                const oldRefreshToken = localStorage.getItem("refreshToken") || sessionStorage.getItem("refreshToken");
                if (!oldRefreshToken) {
                    return Promise.reject(new Error("No refresh token available"));
                }
                const refreshResponse = await refreshTokenAsync({ refreshToken: oldRefreshToken });
                const newAccessToken = refreshResponse.data.data.AccessToken;
                if (newAccessToken) {
                    localStorage.setItem("token", JSON.stringify(newAccessToken))
                    originalRequest.headers["Authorization"] = `Bearer ${newAccessToken}`;
                    return api(originalRequest);
                }
            } catch (refreshError) {
                return Promise.reject(refreshError);
            }
        }
        return Promise.reject(error);
    }
);

export default api;