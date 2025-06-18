/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState } from 'react';
import { forgotPasswordAsync } from '../services/authService';

const ForgotPasswordPage: React.FC = () => {
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(e.target.value);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage(null);
        setError(null);

        try {
            await forgotPasswordAsync(email);
            setMessage('Chúng tôi đã gửi một liên kết đặt lại mật khẩu đến địa chỉ email này.');
        } catch (error: any) {
            if (error.errors) {
                setError(error.errors);
            } else {
                setError('Có lỗi xảy ra khi gửi yêu cầu đặt lại mật khẩu.');
            }
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100">
            <div className="bg-white p-8 rounded-lg shadow-xl w-full max-w-md">
                <h2 className="text-3xl font-bold text-center text-gray-800 mb-8">Quên Mật Khẩu</h2>
                {message && (
                    <div className="text-green-500 text-sm text-center mb-4">{message}</div>
                )}
                {error && (
                    <div className="text-red-500 text-sm text-center mb-4">{error}</div>
                )}
                <form onSubmit={handleSubmit} className="space-y-6">
                    <div>
                        <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                            Địa chỉ Email
                        </label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={email}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>
                    <div>
                        <button
                            type="submit"
                            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                        >
                            Gửi Yêu Cầu Đặt Lại Mật Khẩu
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default ForgotPasswordPage;