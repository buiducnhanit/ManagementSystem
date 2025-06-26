/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom';
import { createUserAsync, getUserByIdAsync, updateUserAsync } from '../services/userService';
import type { UserForm } from '../types/UserForm';

const CreateUserPage: React.FC = () => {
    const { id } = useParams<{ id?: string }>();
    const [formData, setFormData] = useState<UserForm>({
        email: '',
        firstName: '',
        lastName: '',
        address: '',
        phoneNumber: '',
        dateOfBirth: new Date().toISOString().split('T')[0],
        gender: true,
        roles: []
    });
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const isEditMode = !!id;

    useEffect(() => {
        const fetchUser = async () => {
            if (isEditMode) {
                try {
                    setLoading(true);
                    const response = await getUserByIdAsync(id);
                    const user = response.data.data;
                    setFormData({
                        email: user.email || '',
                        firstName: user.firstName || '',
                        lastName: user.lastName || '',
                        address: user.address || '',
                        phoneNumber: user.phoneNumber || '',
                        dateOfBirth: user.dateOfBirth
                            ? user.dateOfBirth.split('T')[0]
                            : new Date().toISOString().split('T')[0],
                        gender: typeof user.gender === 'boolean'
                            ? user.gender
                            : user.gender === 'true',
                        roles: user.roles
                    });
                } catch (error: any) {
                    setErrors({ general: 'Không thể tải thông tin người dùng.' })
                    console.log(error);
                } finally {
                    setLoading(false);
                }
            }
        };

        fetchUser();
    }, [id, isEditMode])

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const target = e.target as HTMLInputElement | HTMLSelectElement;
        const { name, value, type } = target;
        setFormData({
            ...formData,
            [name]: type === 'checkbox' ? (target as HTMLInputElement).checked : value,
        });
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setErrors({});
        try {
            if (isEditMode) {
                await updateUserAsync(id, formData);
            }
            else {
                await createUserAsync(formData);
            }
            navigate('/users')
        } catch (error: any) {
            if (error.response?.data?.errors) {
                setErrors(error.response.data.errors);
            }
            else {
                setErrors({ general: 'Có lỗi xảy ra khi tạo người dùng.' })
            }
        }
    }

    if (loading) {
        return <div className="text-center py-10 text-lg">Đang tải thông tin người dùng...</div>;
    }

    return (
        <div className="flex items-center justify-center bg-gray-100 py-8 px-2">
            <div className="bg-white p-8 rounded-xl shadow-xl w-full max-w-2xl">
                <h2 className="text-2xl font-bold mb-6 text-indigo-700 text-center">
                    {isEditMode ? 'Cập nhật người dùng' : 'Tạo người dùng mới'}
                </h2>
                {errors.general && <div className="text-red-500 mb-4 text-center">{errors.general}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="space-y-4">
                            <div>
                                <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                                    Email
                                </label>
                                <input
                                    type="email"
                                    id="email"
                                    name="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-400"
                                />
                                {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email}</p>}
                            </div>
                            <div>
                                <label htmlFor="firstName" className="block text-sm font-medium text-gray-700">
                                    Họ
                                </label>
                                <input
                                    type="text"
                                    id="firstName"
                                    name="firstName"
                                    value={formData.firstName}
                                    onChange={handleChange}
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-400"
                                />
                                {errors.firstName && <p className="text-red-500 text-xs mt-1">{errors.firstName}</p>}
                            </div>
                            <div>
                                <label htmlFor="lastName" className="block text-sm font-medium text-gray-700">
                                    Tên
                                </label>
                                <input
                                    type="text"
                                    id="lastName"
                                    name="lastName"
                                    value={formData.lastName}
                                    onChange={handleChange}
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-400"
                                />
                                {errors.lastName && <p className="text-red-500 text-xs mt-1">{errors.lastName}</p>}
                            </div>
                            <div>
                                <label htmlFor="address" className="block text-sm font-medium text-gray-700">
                                    Địa chỉ
                                </label>
                                <input
                                    type="text"
                                    id="address"
                                    name="address"
                                    value={formData.address}
                                    onChange={handleChange}
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-400"
                                />
                                {errors.address && <p className="text-red-500 text-xs mt-1">{errors.address}</p>}
                            </div>
                        </div>
                        <div className="space-y-4">
                            <div>
                                <label htmlFor="phoneNumber" className="block text-sm font-medium text-gray-700">
                                    Số điện thoại
                                </label>
                                <input
                                    type="tel"
                                    id="phoneNumber"
                                    name="phoneNumber"
                                    value={formData.phoneNumber}
                                    onChange={handleChange}
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-400"
                                />
                                {errors.phoneNumber && <p className="text-red-500 text-xs mt-1">{errors.phoneNumber}</p>}
                            </div>
                            <div>
                                <label htmlFor="dateOfBirth" className="block text-sm font-medium text-gray-700">
                                    Ngày sinh
                                </label>
                                <input
                                    type="date"
                                    id="dateOfBirth"
                                    name="dateOfBirth"
                                    value={formData.dateOfBirth}
                                    onChange={handleChange}
                                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-400"
                                />
                                {errors.dateOfBirth && <p className="text-red-500 text-xs mt-1">{errors.dateOfBirth}</p>}
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Giới tính
                                </label>
                                <div className="flex items-center gap-6">
                                    <label className="inline-flex items-center">
                                        <input
                                            type="radio"
                                            name="gender"
                                            value="true"
                                            checked={formData.gender === true}
                                            onChange={() => setFormData({ ...formData, gender: true })}
                                            className="form-radio text-indigo-600 focus:ring-indigo-500"
                                        />
                                        <span className="ml-2">Nam</span>
                                    </label>
                                    <label className="inline-flex items-center">
                                        <input
                                            type="radio"
                                            name="gender"
                                            value="false"
                                            checked={formData.gender === false}
                                            onChange={() => setFormData({ ...formData, gender: false })}
                                            className="form-radio text-indigo-600 focus:ring-indigo-500"
                                        />
                                        <span className="ml-2">Nữ</span>
                                    </label>
                                </div>
                                {errors.gender && <p className="text-red-500 text-xs mt-1">{errors.gender}</p>}
                            </div>
                        </div>
                    </div>
                    <div className="mt-8 flex flex-col md:flex-row items-center justify-between gap-4">
                        <button
                            className="w-full md:w-auto flex justify-center py-2 px-8 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                            type="submit"
                        >
                            {isEditMode ? 'Cập nhật người dùng' : 'Tạo người dùng'}
                        </button>
                        <button
                            type="button"
                            className="w-full md:w-auto flex justify-center py-2 px-8 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-gray-500 hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-400"
                            onClick={() => navigate('/users')}
                        >
                            Hủy
                        </button>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default CreateUserPage