/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState } from 'react'
import type { registerForm } from '../types/registerForm';
import { registerAsync } from '../services/authService';
import { useNavigate } from 'react-router-dom';

const RegisterPage: React.FC = () => {
    const [registerForm, setRegisterForm] = useState<registerForm>({
        "userName": "",
        "firstName": "",
        "lastName": "",
        "password": "",
        "email": "",
        "phoneNumber": "",
        "address": "",
        "dateOfBirth": "",
        "gender": true
    })
    const navigate = useNavigate();

    const handleChange = <K extends keyof registerForm>(e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;

        setRegisterForm((prevData) => {
            const updatedValue: registerForm[K] = (() => {
                if (e.target instanceof HTMLInputElement) {
                    if (e.target.type === 'checkbox') {
                        return e.target.checked as registerForm[K];
                    } else if (e.target.type === 'radio') {
                        return (value === 'true') as registerForm[K];
                    }
                }
                return value as registerForm[K];
            })();

            return {
                ...prevData,
                [name as K]: updatedValue,
            };
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        console.log("Form data:", registerForm);
        try {
            const data = await registerAsync(registerForm);
            console.log("Register successfully.", data);
            navigate("/login");
        } catch (error: any) {
            throw new Error(error.response?.data?.message);
        }
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100 p-4">
            <div className="bg-white p-8 rounded-lg shadow-xl w-full max-w-md">
                <h2 className="text-3xl font-bold text-center text-gray-800 mb-8">Đăng Ký Tài Khoản</h2>
                <form onSubmit={handleSubmit} className="space-y-6">
                    {/* UserName */}
                    <div>
                        <label htmlFor="userName" className="block text-sm font-medium text-gray-700">
                            Tên đăng nhập
                        </label>
                        <input
                            type="text"
                            id="userName"
                            name="userName"
                            value={registerForm.userName}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* First Name */}
                    <div>
                        <label htmlFor="firstName" className="block text-sm font-medium text-gray-700">
                            Tên
                        </label>
                        <input
                            type="text"
                            id="firstName"
                            name="firstName"
                            value={registerForm.firstName}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Last Name */}
                    <div>
                        <label htmlFor="lastName" className="block text-sm font-medium text-gray-700">
                            Họ
                        </label>
                        <input
                            type="text"
                            id="lastName"
                            name="lastName"
                            value={registerForm.lastName}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Password */}
                    <div>
                        <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                            Mật khẩu
                        </label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={registerForm.password}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Email */}
                    <div>
                        <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                            Email
                        </label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={registerForm.email}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Phone Number */}
                    <div>
                        <label htmlFor="phoneNumber" className="block text-sm font-medium text-gray-700">
                            Số điện thoại
                        </label>
                        <input
                            type="tel" // Use type="tel" for phone numbers
                            id="phoneNumber"
                            name="phoneNumber"
                            value={registerForm.phoneNumber}
                            onChange={handleChange}
                            required
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Address */}
                    <div>
                        <label htmlFor="address" className="block text-sm font-medium text-gray-700">
                            Địa chỉ
                        </label>
                        <input
                            type="text"
                            id="address"
                            name="address"
                            value={registerForm.address}
                            onChange={handleChange}
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Date of Birth */}
                    <div>
                        <label htmlFor="dateOfBirth" className="block text-sm font-medium text-gray-700">
                            Ngày sinh
                        </label>
                        <input
                            type="date"
                            id="dateOfBirth"
                            name="dateOfBirth"
                            value={registerForm.dateOfBirth}
                            onChange={handleChange}
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        />
                    </div>

                    {/* Gender */}
                    <div className="flex items-center space-x-4">
                        <span className="text-sm font-medium text-gray-700">Giới tính:</span>
                        <div className="flex items-center">
                            <input
                                type="radio"
                                id="genderMale"
                                name="gender"
                                value="true"
                                checked={registerForm.gender === true}
                                onChange={handleChange}
                                className="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300"
                            />
                            <label htmlFor="genderMale" className="ml-2 block text-sm text-gray-900">
                                Nam
                            </label>
                        </div>
                        <div className="flex items-center">
                            <input
                                type="radio"
                                id="genderFemale"
                                name="gender"
                                value="false"
                                checked={registerForm.gender === false}
                                onChange={handleChange}
                                className="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300"
                            />
                            <label htmlFor="genderFemale" className="ml-2 block text-sm text-gray-900">
                                Nữ
                            </label>
                        </div>
                    </div>

                    {/* Submit Button */}
                    <button
                        type="submit"
                        className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                    >
                        Đăng Ký
                    </button>
                </form>
            </div>
        </div>
    )
}

export default RegisterPage