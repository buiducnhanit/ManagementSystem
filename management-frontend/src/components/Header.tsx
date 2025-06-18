import React from 'react'
import { logoutAsync } from '../services/authService'
import { Link } from 'react-router';

interface HeaderProps {
    userName?: string;
}

const Header: React.FC<HeaderProps> = ({ userName }) => {
    const handleLogout = async () => {
        await logoutAsync();
    }

    return (
        <header className="bg-white shadow px-6 py-4 flex items-center justify-between">
            <div className="text-xl font-bold text-indigo-700">Quản lý người dùng</div>
            <div className="flex items-center space-x-4">
                {userName && (
                    <div>
                        <Link to="/profile" className="text-sm text-gray-700 hover:underline">
                            {userName}
                        </Link>
                    </div>
                )}
                <div>
                    <button onClick={handleLogout} className="text-sm text-indigo-600 hover:underline">Đăng xuất</button>
                </div>
            </div>
        </header>
    )
}

export default Header