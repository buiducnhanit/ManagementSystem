import React from 'react'

const NotFoundPage: React.FC = () => {
    return (
        <div className="flex flex-col items-center justify-center h-screen bg-gradient-to-br from-indigo-50 to-white">
            <div className="bg-white rounded-2xl shadow-xl p-10 flex flex-col items-center">
                <svg className="w-20 h-20 text-indigo-400 mb-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                    <circle cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="2" />
                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 8v4m0 4h.01" />
                </svg>
                <h1 className="text-5xl font-extrabold text-indigo-500 mb-2">404</h1>
                <p className="text-xl mb-4 text-gray-700">Không tìm thấy trang.</p>
                <a
                    href="/"
                    className="px-6 py-2 bg-indigo-500 text-white rounded-lg font-semibold shadow hover:bg-indigo-600 transition"
                >
                    Về trang chủ
                </a>
            </div>
        </div>
    )
}

export default NotFoundPage