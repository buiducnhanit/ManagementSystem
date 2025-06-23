import React from 'react'
import { Navigate } from 'react-router-dom';
import { getUserRole } from '../utils/helper';

interface RoleRouteProps {
    children: React.ReactElement;
    allowedRoles: string[];
}

const RoleRoute: React.FC<RoleRouteProps> = ({ children, allowedRoles }) => {
    const role = getUserRole();
    if (!role) {
        return <Navigate to="/unauthorized" replace />;
    }
    return allowedRoles.includes(role) ? children : <Navigate to="/forbidden" replace />
};

export default RoleRoute
