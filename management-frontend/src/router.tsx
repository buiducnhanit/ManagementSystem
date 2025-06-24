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
import ChangePassword from './pages/ChangePassword';
import PrivateRoute from './router/PrivateRoute';
import PublicRoute from './router/PublicRoute';
import ForbiddenPage from './pages/errors/ForbiddenPage';
import UnauthorizedPage from './pages/errors/UnauthorizedPage';
import NotFoundPage from './pages/errors/NotFoundPage';
import RoleRoute from './router/RoleRoute';

const AppRouter: React.FC = () => {
    return (
        <Routes>
            <Route path='/' element={
                <PublicRoute>
                    <LoginPage />
                </PublicRoute>
            } />
            <Route path='/login' element={
                <PublicRoute>
                    <LoginPage />
                </PublicRoute>
            } />
            <Route path='/register' element={
                <PublicRoute>
                    <RegisterPage />
                </PublicRoute>
            } />
            <Route path='/confirm-email' element={<ConfirmEmailPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/reset-password" element={<ResetPasswordPage />} />
            <Route path='/change-password' element={<ChangePassword />} />

            {/* Protected routes */}
            <Route element={<PrivateRoute><UserLayout /></PrivateRoute>}>
                <Route path='users' element={<UserListPage />}/>
                <Route path='users/create' element={
                    <RoleRoute allowedRoles={['Admin']}>
                        <CreateUserPage />
                    </RoleRoute>} />
                <Route path='users/edit/:id' element={
                    <RoleRoute allowedRoles={['Admin', 'Manager']}>
                        <CreateUserPage />
                    </RoleRoute>} />
                <Route path='users/:id' element={<UserProfilePage />} />
            </Route>

            <Route path='/forbidden' element={<ForbiddenPage />} />
            <Route path='/unauthorized' element={<UnauthorizedPage />} />
            <Route path='/*' element={<NotFoundPage />} />
        </Routes>
    )
}

export default AppRouter;