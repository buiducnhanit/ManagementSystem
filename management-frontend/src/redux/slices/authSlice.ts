import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
    isAuthenticated: boolean;
    token: string | null;
    refreshToken: string | null;
    expiresIn: string | null;
}

const initialState: AuthState = {
    isAuthenticated: !!localStorage.getItem('token'),
    token: localStorage.getItem("token") ? JSON.parse(localStorage.getItem("token") as string) : null,
    refreshToken: localStorage.getItem("refreshToken") ? JSON.parse(localStorage.getItem("refreshToken") as string) : null,
    expiresIn: localStorage.getItem("expiresIn") || null,
}

const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        loginSuccess: (state, action: PayloadAction<{
            expiresIn: string | null; token: string, refreshToken: string
        }>) => {
            state.isAuthenticated = true;
            state.token = action.payload.token;
            state.refreshToken = action.payload.refreshToken;
            state.expiresIn = action.payload.expiresIn;

            localStorage.setItem("token", JSON.stringify(state.token));
            localStorage.setItem("refreshToken", JSON.stringify(state.refreshToken));
            localStorage.setItem("expiresIn", state.expiresIn ?? "")
        },
        logout: (state) => {
            state.isAuthenticated = false;
            state.token = null;
            state.refreshToken = null;
            state.expiresIn = null;

            // localStorage.removeItem("token");
            // localStorage.removeItem("refreshToken");
            // localStorage.removeItem("expiresIn");
        }
    }
})

export const { loginSuccess, logout } = authSlice.actions;
export default authSlice.reducer;