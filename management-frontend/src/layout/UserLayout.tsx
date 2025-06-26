import React, { useEffect, useState } from 'react'
import { Link, Outlet, useLocation } from 'react-router-dom'
import Header from '../components/Header'
import Footer from '../components/Footer'
import { getUserProfileAsync } from '../services/userService'

const UserLayout: React.FC = () => {
    const [avatar, setAvatar] = useState<string>('');
    const [userName, setUserName] = useState<string>('');
    const location = useLocation();

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const res = await getUserProfileAsync();
                if (res.data && res.data.data) {
                    setAvatar(res.data.data.avatarUrl || '');
                    setUserName(res.data.data.userName || '');
                } else {
                    setAvatar('');
                    setUserName('');
                }
            } catch {
                setAvatar('');
                setUserName('');
            }
        }
        fetchUserData();
    }, []);

    const navLinks = [
        { to: '/users', label: 'Danh sách người dùng' },
        { to: '/users/create', label: 'Thêm người dùng' }
    ];

    return (
        <div className="min-h-screen flex flex-col bg-gray-100">
            <Header avatar={avatar} userName={userName} />

            <div className="flex flex-1">
                <aside className="w-64 bg-white border-r p-8 hidden md:block shadow-lg rounded-tr-2xl rounded-br-2xl mt-6 ml-4 h-[calc(100vh-120px)]">
                    <nav className="space-y-4">
                        {navLinks.map(link => (
                            <Link
                                key={link.to}
                                to={link.to}
                                className={`block px-4 py-2 rounded-lg font-medium transition ${
                                    location.pathname === link.to
                                        ? 'bg-indigo-100 text-indigo-700'
                                        : 'text-gray-700 hover:bg-indigo-50 hover:text-indigo-600'
                                }`}
                            >
                                {link.label}
                            </Link>
                        ))}
                    </nav>
                </aside>

                <main className="flex-1 p-6 md:p-10">

                        <Outlet />

                </main>
            </div>

            <Footer />
        </div>
    )
}

export default UserLayout