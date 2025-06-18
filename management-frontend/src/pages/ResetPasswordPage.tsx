/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { resetPasswordAsync } from '../services/authService';

const ResetPasswordPage: React.FC = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [token, setToken] = useState<string | null>(null);
    const [userId, setUserId] = useState<string | null>(null);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const tokenParam = searchParams.get('token');
        const userIdParam = searchParams.get('userId');

        if (!tokenParam || !userIdParam) {
            setError('Invalid reset password link.');
            return;
        }

        setToken(tokenParam);
        setUserId(userIdParam);
    }, [searchParams]);

    const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(e.target.value);
    };

    const handleConfirmPasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setConfirmPassword(e.target.value);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage(null);
        setError(null);

        if (password !== confirmPassword) {
            setError('Mật khẩu và xác nhận mật khẩu không khớp.');
            return;
        }

        if (!token || !userId) {
            setError('Invalid reset password link.');
            return;
        }

        try {
            await resetPasswordAsync(userId, token, password);
            setMessage('Mật khẩu của bạn đã được đặt lại thành công.');
            setTimeout(() => {
                navigate('/login');
            }, 3000);
        } catch (error: any) {
            if (error.errors) {
                setError(error.errors);
            } else {
                setError('Có lỗi xảy ra khi đặt lại mật khẩu.');
            }
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100">
            <div className="bg-white p-8 rounded-lg shadow-xl w-full max-w-md">
                <h2 className="text-3xl font-bold text-center text-gray-800 mb-8">Đặt Lại Mật Khẩu</h2>
                {message && (
                    <div className="text-green-500 text-sm text-center mb-4">{message}</div>
                )}
                {error && (
                    <div className="text-red-500 text-sm text-center mb-4">{error}</div>
                )}
                <form onSubmit={handleSubmit} className="space-y-6">
                    <div>
                        <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                            Mật Khẩu Mới
                        </label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={password}
                            onChange={handlePasswordChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>
                    <div>
                        <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700">
                            Xác Nhận Mật Khẩu
                        </label>
                        <input
                            type="password"
                            id="confirmPassword"
                            name="confirmPassword"
                            value={confirmPassword}
                            onChange={handleConfirmPasswordChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>
                    <div>
                        <button
                            type="submit"
                            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                        >
                            Đặt Lại Mật Khẩu
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default ResetPasswordPage;