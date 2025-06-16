import React from 'react'
import { Route, Routes } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import UserLayout from './layout/userLayout';
import UserListPage from './pages/UserListPage';

const AppRouter: React.FC = () => {
    return (
        <Routes>
            <Route path='/' element={<LoginPage />} />
            <Route path='/login' element={<LoginPage />} />
            <Route path='/register' element={<RegisterPage />} />

            <Route element={<UserLayout />}>
                <Route path='users' element={<UserListPage />} />
            </Route>
        </Routes>
    )
}

export default AppRouter;