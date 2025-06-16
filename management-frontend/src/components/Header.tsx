import React from 'react'
import { logoutAsync } from '../services/authService'

const Header: React.FC = () => {
    const handleLogout = async () => {
        await logoutAsync();
    }

    return (
        <header className="bg-white shadow px-6 py-4 flex items-center justify-between">
            <div className="text-xl font-bold text-indigo-700">Quản lý người dùng</div>
            <div>
                <button onClick={handleLogout} className="text-sm text-indigo-600 hover:underline">Đăng xuất</button>
            </div>
        </header>
    )
}

export default Header