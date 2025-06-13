import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
    isAuthenticated: boolean;
    token: string | null;
    refreshToken: string | null;
}

const initialState: AuthState = {
    isAuthenticated: false,
    token: null,
    refreshToken: null,
}

const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        loginSuccess: (state, action: PayloadAction<{ token: string, refreshToken: string }>) => {
            state.isAuthenticated = true;
            state.token = action.payload.token;
            state.refreshToken = action.payload.refreshToken;

            localStorage.setItem("token", JSON.stringify(state.token));
            localStorage.setItem("refreshToken", JSON.stringify(state.refreshToken));
        },
        logout: (state) => {
            state.isAuthenticated = false;
            state.token = null;
            state.refreshToken = null;

            localStorage.removeItem("token");
            localStorage.removeItem("refreshToken");
        }
    }
})

export const { loginSuccess, logout } = authSlice.actions;
export default authSlice.reducer;