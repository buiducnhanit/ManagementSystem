import React from 'react'

const ForbiddenPage: React.FC = () => {
    return (
        <div className="flex flex-col items-center justify-center h-screen bg-gradient-to-br from-red-50 to-white">
            <div className="bg-white rounded-2xl shadow-xl p-10 flex flex-col items-center">
                <svg className="w-20 h-20 text-red-400 mb-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                    <circle cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="2" />
                    <path strokeLinecap="round" strokeLinejoin="round" d="M9 9l6 6m0-6l-6 6" />
                </svg>
                <h1 className="text-5xl font-extrabold text-red-500 mb-2">403</h1>
                <p className="text-xl mb-4 text-gray-700">Bạn không có quyền truy cập trang này.</p>
                <a
                    href="/"
                    className="px-6 py-2 bg-red-400 text-white rounded-lg font-semibold shadow hover:bg-red-500 transition"
                >
                    Về trang chủ
                </a>
            </div>
        </div>
    )
}

export default ForbiddenPage