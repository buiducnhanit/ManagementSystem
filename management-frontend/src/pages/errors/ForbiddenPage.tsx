import React from 'react'

const ForbiddenPage: React.FC = () => {
    return (
        <div className="flex flex-col items-center justify-center h-screen">
            <h1 className="text-5xl font-bold mb-4">403</h1>
            <p className="text-xl mb-2">Bạn không có quyền truy cập trang này.</p>
            <a href="/" className="text-blue-600 underline">Về trang chủ</a>
        </div>
    )
}

export default ForbiddenPage