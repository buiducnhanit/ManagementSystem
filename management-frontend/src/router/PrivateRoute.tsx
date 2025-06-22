import React from 'react';
import { Navigate } from 'react-router-dom';

const isAuthenticated = () => {
    return !!(localStorage.getItem('token') || sessionStorage.getItem('token'));
};

const PrivateRoute = ({ children }: { children: React.ReactElement }) => {
    return isAuthenticated() ? children : <Navigate to="/login" replace />;
};

export default PrivateRoute;