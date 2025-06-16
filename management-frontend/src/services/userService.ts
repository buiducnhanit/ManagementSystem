import api from "./axiosInstance";

/* eslint-disable @typescript-eslint/no-explicit-any */
export const getAllUsersAsync = () => {
    try {
        const response = api.get('/users');
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const getUserProfileAsync = () => {
    try {
        const response = api.get('');
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const updateUserAsync = () => {
    try {
        const response = api.post('/users');
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}

export const deleteUserAsync = () => {
    try {
        const response = api.delete('/users');
        return response;
    } catch (error: any) {
        throw error.response?.data;
    }
}