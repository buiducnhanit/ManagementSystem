/* eslint-disable @typescript-eslint/no-explicit-any */
import axios from "axios";
import { API_BASE_URL } from "../utils/constants";
import type { registerForm } from "../types/registerForm";
import type { loginForm } from "../types/loginForm";
import type { changePassword } from "../types/changePassword";
import api from "./axiosInstance";

export const registerAsync = async (registerRequest: registerForm) => {
    try {
        // console.log(registerRequest);
        const response = await axios.post(`${API_BASE_URL}/auth/register`, registerRequest, {
            headers: { "Content-Type": "application/json" }
        });
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const loginAsync = async (loginRequest: loginForm) => {
    try {
        // console.log(loginRequest);
        const response = await axios.post(`${API_BASE_URL}/auth/login`, loginRequest, {
            headers: { "Content-Type": "application/json" }
        });
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const logoutAsync = async () => {
    try {
        const response = await api.post(`${API_BASE_URL}/auth/logout`);
        return response;
    } catch (error: any) {
        throw error.response;
    }
}

export const refreshTokenAsync = async (userId: string, refreshToken: string) => {
    try {
        if (!refreshToken)
            throw new Error("No refresh token found.");
        const response = await axios.post(`${API_BASE_URL}/auth/refresh-token`, { userId: userId, refreshToken: refreshToken });

        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const forgotPasswordAsync = async (email: string) => {
    try {
        const response = await api.post(`/auth/forgot-password`, { email });
        return response;
    }
    catch (error: any) {
        throw error.response?.data;
    }
}

export const resetPasswordAsync = async (userId: string, token: string, password: string) => {
    try {
        const response = await api.post(`/auth/reset-password`, { userId: userId, token: token, newPassword: password });
        return response;
    } catch (error: any) {
        console.log(error.response?.data)
        throw error.response?.data;
    }
}

export const confirmEmailAsync = async (userId: string, token: string) => {
    try {
        const response = await api.post(`/auth/confirm-email`, { userId, token });
        return response;
    } catch (error: any) {
        throw error.response.data;
    }
}

export const resendConfirmEmailAsync = async (email: string) => {
    try {
        const response = await api.post(`/auth/resend-confirm-email`, { email });
        return response;
    } catch (error: any) {
        throw error.response.data;
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

export const addUserRoleAsync = async (userId: string, roles: string[]) => {
    try {
        const response = await api.post(`/roles/add-roles`, { userId, roleNames: roles });
        return response;
    } catch (error: any) {
        console.log(error.response?.data)
        throw error.response?.data;
    }
}

export const removeUserRoleAsync = async (userId: string, roles: string[]) => {
    try {
        const response = await api.post(`/roles/remove-roles`, { userId, roleNames: roles });
        return response;
    } catch (error: any) {
        console.log(error.response?.data)
        throw error.response?.data;
    }
}

export const unlockUserAsync = async (userId: string) => {
    try {
        const response = await api.post(`/auth/unlockout`, { id: userId });
        return response;
    } catch (error: any) {
        console.log(error.response?.data);
        throw error.response?.data;
    }
}