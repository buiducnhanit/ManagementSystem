/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState } from 'react'
import type { registerForm } from '../types/registerForm';
import { registerAsync } from '../services/authService';
import { useNavigate, Link } from 'react-router-dom';

const RegisterPage: React.FC = () => {
    const [registerForm, setRegisterForm] = useState<registerForm>({
        "userName": "",
        "firstName": "",
        "lastName": "",
        "password": "",
        "email": "",
        "phoneNumber": "",
        "address": "",
        "dateOfBirth": new Date().toISOString().split('T')[0],
        "gender": true
    })
    const [fieldErrors, setFieldErrors] = useState<{ [key: string]: string[] }>({});
    const [errors, setErrors] = useState<string | null>(null);
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
        setFieldErrors({});
        setErrors(null);
        try {
            const response = await registerAsync(registerForm);
            console.log(response)
            if (response.status === 201) {
                alert("Đăng ký thành công! Vui lòng đăng nhập.");
            }
            navigate("/login");
        } catch (error: any) {
            if (Array.isArray(error.errors)) {
                setErrors(error.errors.join(', '));
            } else if (error.errors) {
                setFieldErrors(error.errors);
            } else {
                alert(error?.title || "Đăng ký không thành công. Vui lòng thử lại.");
                console.error("Registration error:", error);
                setErrors(error?.title || error?.errors)
            }
        }
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100 p-4">
            <div className="bg-white p-8 rounded-lg shadow-xl w-full max-w-3xl">
                <h2 className="text-3xl font-bold text-center text-gray-800 mb-8">Đăng Ký Tài Khoản</h2>
                {errors && (
                    <div className="text-red-500 text-sm text-center mb-4">{errors}</div>
                )}
                <form onSubmit={handleSubmit}>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="space-y-4">
                            {/* UserName */}
                            {/* <div>
                                <label htmlFor="userName" className="block text-sm font-medium text-gray-700">
                                    Tên đăng nhập
                                </label>
                                <input
                                    type="text"
                                    id="userName"
                                    name="userName"
                                    value={registerForm.userName}
                                    onChange={handleChange}
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                                />
                                {fieldErrors.UserName && fieldErrors.UserName.map((msg, idx) => (
                                    <p key={idx} className="text-red-500 text-xs mt-1">{msg}</p>
                                ))}
                            </div> */}
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
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                                />
                                {fieldErrors.FirstName && fieldErrors.FirstName.map((msg, idx) => (
                                    <p key={idx} className="text-red-500 text-xs mt-1">{msg}</p>
                                ))}
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
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                                />
                                {fieldErrors.LastName && fieldErrors.LastName.map((msg, idx) => (
                                    <p key={idx} className="text-red-500 text-xs mt-1">{msg}</p>
                                ))}
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
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                                />
                                {fieldErrors.Password && fieldErrors.Password.map((msg, idx) => (
                                    <p key={idx} className="text-red-500 text-xs mt-1">{msg}</p>
                                ))}
                            </div>
                            <div className="flex items-center space-x-4 mt-2">
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
                        </div>

                        <div className="space-y-4">
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
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                                />
                                {fieldErrors.Email && fieldErrors.Email.map((msg, idx) => (
                                    <p key={idx} className="text-red-500 text-xs mt-1">{msg}</p>
                                ))}
                            </div>
                            {/* Phone Number */}
                            <div>
                                <label htmlFor="phoneNumber" className="block text-sm font-medium text-gray-700">
                                    Số điện thoại
                                </label>
                                <input
                                    type="tel"
                                    id="phoneNumber"
                                    name="phoneNumber"
                                    value={registerForm.phoneNumber}
                                    onChange={handleChange}
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                                />
                                {fieldErrors.PhoneNumber && fieldErrors.PhoneNumber.map((msg, idx) => (
                                    <p key={idx} className="text-red-500 text-xs mt-1">{msg}</p>
                                ))}
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
                                {fieldErrors.Address && fieldErrors.Address.map((msg, idx) => (
                                    <p key={idx} className="text-red-500 text-xs mt-1">{msg}</p>
                                ))}
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
                                {fieldErrors.DateOfBirth && fieldErrors.DateOfBirth.map((msg, idx) => (
                                    <p key={idx} className="text-red-500 text-xs mt-1">{msg}</p>
                                ))}
                            </div>
                            {/* Gender */}
                            
                        </div>
                    </div>
                    {/* Submit & Link */}
                    <div className="mt-8 flex flex-col md:flex-row items-center justify-between gap-4">
                        <button
                            type="submit"
                            className="w-full md:w-auto flex justify-center py-2 px-8 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                        >
                            Đăng Ký
                        </button>
                        <Link
                            to="/login"
                            className="text-indigo-600 hover:underline text-sm"
                        >
                            Đã có tài khoản? Đăng nhập
                        </Link>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default RegisterPage