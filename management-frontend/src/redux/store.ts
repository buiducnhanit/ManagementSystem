import { configureStore } from "@reduxjs/toolkit";
import authReducer from '../redux/slices/authSlice'

export const store = configureStore({
    reducer: {
        auht: authReducer,
    },
    devTools: true,
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;