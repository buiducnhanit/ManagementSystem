import React from 'react'
import { Link, Outlet } from 'react-router-dom'
import Header from '../components/Header'
import Footer from '../components/Footer'

const UserLayout: React.FC = () => {
    const userName = "Duc Nhan"

    return (
        <div className="min-h-screen flex flex-col bg-gray-100">
            <Header userName={userName} />

            <div className="flex flex-1">
                <aside className="w-64 bg-white border-r p-6 hidden md:block">
                    <nav className="space-y-4">
                        <Link to="/users" className="block text-gray-700 hover:text-indigo-600">Danh sách người dùng</Link>
                        <Link to="/users/create" className="block text-gray-700 hover:text-indigo-600">Thêm người dùng</Link>
                    </nav>
                </aside>

                <main className="flex-1 p-6">
                    <Outlet />
                </main>
            </div>

            <Footer />
        </div>
    )
}

export default UserLayout