import React from 'react';
import { Navigate } from 'react-router-dom';

const isAuthenticated = () => {
    return !!(localStorage.getItem('token') || sessionStorage.getItem('token'));
};

const PublicRoute = ({ children }: { children: React.ReactElement }) => {
    return isAuthenticated() ? <Navigate to="/users" replace /> : children;
};

export default PublicRoute;