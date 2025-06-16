/* eslint-disable @typescript-eslint/no-explicit-any */
import axios from "axios";
import API_BASE_URL from "../utils/constants";
import type { registerForm } from "../types/registerForm";
import type { loginForm } from "../types/loginForm";
import type { refreshToken } from "../types/refreshToken";
import type { forgotPassword } from "../types/forgotPassword";
import type { resetPassword } from "../types/resetPassword";
import type { changePassword } from "../types/changePassword";
import api from "./axiosInstance";

export const registerAsync = async (registerRequest: registerForm) => {
    try {
        console.log(registerRequest);
        const response = await axios.post(`${API_BASE_URL}/auth/register`, registerRequest,
            {
                headers: {
                    "Content-Type": "application/json",
                },
            });
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const loginAsync = async (loginRequest: loginForm) => {
    try {
        // console.log(loginRequest);
        const response = await axios.post(`${API_BASE_URL}/auth/login`, loginRequest,
            {
                headers: {
                    "Content-Type": "application/json",
                },
            });
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const logoutAsync = async () => {
    try {
        const { data } = await axios.post(`${API_BASE_URL}/auth/logout`);
        return data;
    } catch (error: any) {
        throw new Error(error.response?.data?.message);
    }
}

export const refreshTokenAsync = async (refreshTokenRequest: refreshToken) => {
    try {
        const refreshToken = localStorage.getItem("refreshToken") || sessionStorage.getItem("refreshToken");
        if (!refreshToken)
            throw new Error("No refresh token found.");
        const { data } = await axios.post(`${API_BASE_URL}/auth/refresh-token`, refreshTokenRequest);

        if (localStorage.getItem("refreshToken")) {
            localStorage.setItem("token", data.accessToken);
        }
        else {
            sessionStorage.setItem("token", data.accessToken);
        }

        return data.accessToken;
    } catch (error: any) {
        throw new Error(error.response?.data?.message);
    }
}

export const forgotPasswordAsync = async (forgotPasswordRequest: forgotPassword) => {
    try {
        const { data } = await api.post(`/auth/forgot-password`, forgotPasswordRequest);
        return data;
    }
    catch (error: any) {
        throw new Error(error.response?.data?.message);
    }
}

export const resetPasswordAsync = async (resetPasswordRequest: resetPassword) => {
    try {
        const { data } = await api.post(`/auth/reset-password`, resetPasswordRequest);
        return data;
    } catch (error: any) {
        throw new Error(error.response?.data?.message);
    }
}

export const confirmEmailAsync = async (userId: string, token: string) => {
    try {
        const { data } = await api.post(`/auth/confirm-email`, { userId, token });
        return data;
    } catch (error: any) {
        throw new Error(error.response?.data?.message);
    }
}

export const resendConfirmEmailAsync = async (email: string) => {
    try {
        const { data } = await api.post(`/auth/resend-confirm-email`, { email });
        return data;
    } catch (error: any) {
        throw new Error(error.response?.data?.message);
    }
}

export const changePasswordAsync = async (changePasswordRequest: changePassword) => {
    try {
        const { data } = await api.post(`/auth/change-password`, changePasswordRequest);
        return data;
    }
    catch (error: any) {
        throw new Error(error.response?.data?.message);
    }
}