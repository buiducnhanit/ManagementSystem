import axios from "axios";
import { API_BASE_URL } from "../utils/constants";
import { logout } from "../redux/slices/authSlice";
import { store } from "../redux/store";
import { refreshTokenAsync } from "./authService";

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
            const errorCode = error.response.data?.error;

            if (errorCode === "TokenExpired") {
                originalRequest._retry = true;
                try {
                    const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");
                    const oldRefreshToken = localStorage.getItem('refreshToken') || sessionStorage.getItem('refreshToken');
                    if (!oldRefreshToken || !userId) {
                        store.dispatch(logout());
                        window.location.href = '/login';
                        return Promise.reject(new Error("No refresh token or user ID available"));
                    }

                    const refreshResponse = await refreshTokenAsync(userId, oldRefreshToken);
                    const newAccessToken = refreshResponse.data.data.AccessToken;
                    if (newAccessToken) {
                        localStorage.setItem("token", JSON.stringify(newAccessToken));
                        originalRequest.headers["Authorization"] = `Bearer ${newAccessToken}`;
                        return api(originalRequest);
                    }
                } catch (refreshError) {
                    store.dispatch(logout());
                    window.location.href = '/login';
                    return Promise.reject(refreshError);
                }
            } else if (errorCode === "InvalidToken" || errorCode === "InvalidSession") {
                store.dispatch(logout());
                window.location.href = '/login';
                return Promise.reject(new Error("Invalid or tampered token"));
            }
        }

        return Promise.reject(error);
    }
);

export default api;
