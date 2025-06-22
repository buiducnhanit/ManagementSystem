/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom';
import { createUserAsync, getUserByIdAsync, updateUserAsync } from '../services/userService';
import type { UserForm } from '../types/UserForm';

const CreateUserPage: React.FC = () => {
    const { id } = useParams<{ id?: string }>();
    const [formData, setFormData] = useState<UserForm>({
        userName: '',
        email: '',
        firstName: '',
        lastName: '',
        address: '',
        phoneNumber: '',
        dateOfBirth: new Date().toISOString().split('T')[0],
        gender: true
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
                        userName: user.userName || '',
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
                            : user.gender === 'true'
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
        console.log(formData)
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
        return <div>Đang tải thông tin người dùng...</div>;
    }

    return (
        <div>
            <h2 className="text-2xl font-bold mb-4">Tạo người dùng mới</h2>
            {errors.general && <div className="text-red-500 mb-4">{errors.general}</div>}
            <form onSubmit={handleSubmit} className="max-w-md">
                <div className="mb-4">
                    <label htmlFor="userName" className="block text-gray-700 text-sm font-bold mb-2">
                        Username:
                    </label>
                    <input
                        type="text"
                        id="userName"
                        name="userName"
                        value={formData.userName}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.userName && <p className="text-red-500 text-xs italic">{errors.userName}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="Email" className="block text-gray-700 text-sm font-bold mb-2">
                        Email:
                    </label>
                    <input
                        type="email"
                        id="email"
                        name="email"
                        value={formData.email}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.email && <p className="text-red-500 text-xs italic">{errors.email}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="firstName" className="block text-gray-700 text-sm font-bold mb-2">
                        First name:
                    </label>
                    <input
                        type="text"
                        id="firstName"
                        name="firstName"
                        value={formData.firstName}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.firstName && <p className="text-red-500 text-xs italic">{errors.firstName}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="lastName" className="block text-gray-700 text-sm font-bold mb-2">
                        Last name:
                    </label>
                    <input
                        type="text"
                        id="lastName"
                        name="lastName"
                        value={formData.lastName}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.lastName && <p className="text-red-500 text-xs italic">{errors.lastName}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="address" className="block text-gray-700 text-sm font-bold mb-2">
                        Address:
                    </label>
                    <input
                        type="text"
                        id="address"
                        name="address"
                        value={formData.address}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.address && <p className="text-red-500 text-xs italic">{errors.address}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="phoneNumber" className="block text-gray-700 text-sm font-bold mb-2">
                        Phone number:
                    </label>
                    <input
                        type="tel"
                        id="phoneNumber"
                        name="phoneNumber"
                        value={formData.phoneNumber}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.phoneNumber && <p className="text-red-500 text-xs italic">{errors.phoneNumber}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="dateOfBirth" className="block text-gray-700 text-sm font-bold mb-2">
                        Date of birth:
                    </label>
                    <input
                        type="date"
                        id="dateOfBirth"
                        name="dateOfBirth"
                        value={formData.dateOfBirth}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.dateOfBirth && <p className="text-red-500 text-xs italic">{errors.dateOfBirth}</p>}
                </div>
                <div className="mb-4">
                    <label className="block text-gray-700 text-sm font-bold mb-2">
                        Gender:
                    </label>
                    <div className="flex items-center space-x-4">
                        <label className="inline-flex items-center">
                            <input
                                type="radio"
                                name="gender"
                                value="true"
                                checked={formData.gender === true}
                                onChange={() => setFormData({ ...formData, gender: true })}
                                className="form-radio"
                            />
                            <span className="ml-2">Male</span>
                        </label>
                        <label className="inline-flex items-center">
                            <input
                                type="radio"
                                name="gender"
                                value="false"
                                checked={formData.gender === false}
                                onChange={() => setFormData({ ...formData, gender: false })}
                                className="form-radio"
                            />
                            <span className="ml-2">Female</span>
                        </label>
                    </div>
                    {errors.gender && <p className="text-red-500 text-xs italic">{errors.gender}</p>}
                </div>
                {/* <div className="mb-4">
                    <label htmlFor="role" className="block text-gray-700 text-sm font-bold mb-2">
                        Vai trò:
                    </label>
                    <select
                        id="role"
                        name="role"
                        value={formData.role}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    >
                        <option value="User">Người dùng</option>
                        <option value="Admin">Quản trị viên</option>
                    </select>
                    {errors.role && <p className="text-red-500 text-xs italic">{errors.role}</p>}
                </div> */}
                <div className="flex items-center justify-between">
                    <button
                        className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                        type="submit"
                    >
                        {isEditMode ? 'Cập nhật người dùng' : 'Tạo người dùng'}
                    </button>
                    <button
                        className="bg-gray-500 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                        type="button"
                        onClick={() => navigate('/users')}
                    >
                        Hủy
                    </button>
                </div>
            </form>
        </div>
    )
}

export default CreateUserPage