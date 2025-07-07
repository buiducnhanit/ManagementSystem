import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
    isAuthenticated: boolean;
    token: string | null;
    refreshToken: string | null;
    expiresIn: string | null;
    userId: string | null;
}

const initialState: AuthState = {
    isAuthenticated: !!localStorage.getItem('token') || !!sessionStorage.getItem('token'),
    token: localStorage.getItem("token") || sessionStorage.getItem("token"),
    refreshToken: localStorage.getItem("refreshToken") || sessionStorage.getItem("refreshToken"),
    expiresIn: localStorage.getItem("expiresIn") || sessionStorage.getItem("expiresIn"),
    userId: localStorage.getItem("userId") || sessionStorage.getItem("userId")
}

const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        loginSuccess: (state, action: PayloadAction<{
            expiresIn: string | null; token: string, refreshToken: string, rememberMe: boolean, userId: string
        }>) => {
            state.isAuthenticated = true;
            state.token = action.payload.token;
            state.refreshToken = action.payload.refreshToken;
            state.expiresIn = action.payload.expiresIn;
            state.userId = action.payload.userId;
            if (action.payload.rememberMe) {
                localStorage.setItem("token", state.token ?? "");
                localStorage.setItem("refreshToken", state.refreshToken ?? "");
                localStorage.setItem("expiresIn", state.expiresIn ?? "");
                localStorage.setItem("userId", state.userId ?? "");
            } else {
                sessionStorage.setItem("token", state.token ?? "");
                sessionStorage.setItem("refreshToken", state.refreshToken ?? "");
                sessionStorage.setItem("expiresIn", state.expiresIn ?? "");
                sessionStorage.setItem("userId", state.userId ?? "");
            }
        },
        logout: (state) => {
            state.isAuthenticated = false;
            state.token = null;
            state.refreshToken = null;
            state.expiresIn = null;

            localStorage.removeItem("token");
            sessionStorage.removeItem("token");
            localStorage.removeItem("refreshToken");
            sessionStorage.removeItem("refreshToken");
            localStorage.removeItem("expiresIn");
            sessionStorage.removeItem("expiresIn");
            localStorage.removeItem("userId");
            sessionStorage.removeItem("userId");
        }
    }
})

export const { loginSuccess, logout } = authSlice.actions;
export default authSlice.reducer;