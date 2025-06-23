import React from 'react'

const UnauthorizedPage: React.FC = () => {
    return (
        <div className="flex flex-col items-center justify-center h-screen">
            <h1 className="text-5xl font-bold mb-4">401</h1>
            <p className="text-xl mb-2">Bạn chưa đăng nhập.</p>
            <a href="/login" className="text-blue-600 underline">Đăng nhập</a>
        </div>
    )
}

export default UnauthorizedPage