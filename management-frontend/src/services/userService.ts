/* eslint-disable @typescript-eslint/no-explicit-any */
import type { UserForm } from "../types/UserForm";
import api from "./axiosInstance";

export const getAllUsersAsync = () => {
    try {
        const response = api.get('/users');
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const getUserByIdAsync = (id: string) => {
    try {
        const response = api.get(`/users/${id}`);
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const getUserProfileAsync = () => {
    try {
        const response = api.get(`/users/profile`);
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const createUserAsync = (createUserRequest: UserForm) => {
    try {
        const response = api.post('/auth/create-user', createUserRequest);
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const updateUserAsync = (id: string, updateUserRequest: UserForm) => {
    try {
        const response = api.put(`/users/${id}`, updateUserRequest);
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const deleteUserAsync = (id: string) => {
    try {
        const response = api.delete(`/users/${id}`);
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const getAllRolesAsync = () => {
    try {
        const response = api.get(`/roles`);
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}