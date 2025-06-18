import React from 'react'
import { Route, Routes } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import UserLayout from './layout/UserLayout';
import UserListPage from './pages/UserListPage';
import CreateUserPage from './pages/CreateUserPage';
import ConfirmEmailPage from './pages/ConfirmEmailPage';
import ForgotPasswordPage from './pages/ForgotPasswordPage';
import ResetPasswordPage from './pages/ResetPasswordPage';
import UserProfilePage from './pages/UserProfilePage';

const AppRouter: React.FC = () => {
    return (
        <Routes>
            <Route path='/' element={<LoginPage />} />
            <Route path='/login' element={<LoginPage />} />
            <Route path='/register' element={<RegisterPage />} />
            <Route path='/confirm-email' element={<ConfirmEmailPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/reset-password" element={<ResetPasswordPage />} />

            <Route element={<UserLayout />}>
                <Route path='users' element={<UserListPage />} />
                <Route path='users/create' element={<CreateUserPage />} />
                <Route path='profile' element={<UserProfilePage />} />
            </Route>
        </Routes>
    )
}

export default AppRouter;