/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState } from 'react'
import { loginAsync } from '../services/authService';
import { useDispatch } from 'react-redux';
import { loginSuccess } from '../redux/slices/authSlice';

const LoginPage: React.FC = () => {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        rememberMe: false,
    });
    const dispatch = useDispatch();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value, type, checked } = e.target;
        setFormData((prevData) => ({
            ...prevData,
            [name]: type === 'checkbox' ? checked : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        console.log(formData);
        try {
            const response = await loginAsync(formData);
            console.log(response);
            if (response.statusCode === 200) {
                dispatch(loginSuccess({ token: response.data.accessToken, refreshToken: response.data.refreshToken }));
            }
            else {
                alert(response.message);
            }
        } catch (error: any) {
            alert(error.response?.data?.message)
        }
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100 p-4">
            <div className="bg-white p-8 rounded-lg shadow-xl w-full max-w-md">
                <h2 className="text-3xl font-bold text-center text-gray-800 mb-8">Đăng Nhập</h2>
                <form onSubmit={handleSubmit} className="space-y-6">
                    {/* Tên đăng nhập hoặc Email */}
                    <div>
                        <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                            Email
                        </label>
                        <input
                            type="text"
                            id="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Mật khẩu */}
                    <div>
                        <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                            Mật khẩu
                        </label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Remember me */}
                    <div className="flex items-center">
                        <input
                            id="rememberMe"
                            name="rememberMe"
                            type="checkbox"
                            checked={formData.rememberMe}
                            onChange={handleChange}
                            className="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
                        />
                        <label htmlFor="rememberMe" className="ml-2 block text-sm text-gray-900">
                            Ghi nhớ đăng nhập
                        </label>
                    </div>

                    {/* Nút Đăng nhập */}
                    <button
                        type="submit"
                        className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                    >
                        Đăng Nhập
                    </button>

                    {/* Liên kết "Quên mật khẩu?" */}
                    <div className="text-center">
                        <a href="#" className="font-medium text-indigo-600 hover:text-indigo-500 text-sm">
                            Quên mật khẩu?
                        </a>
                    </div>

                    {/* Liên kết "Chưa có tài khoản?" */}
                    <div className="text-center text-sm text-gray-600">
                        Chưa có tài khoản?{' '}
                        <a href="/register" className="font-medium text-indigo-600 hover:text-indigo-500">
                            Đăng ký ngay
                        </a>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default LoginPage