/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom';
import { createUserAsync, getUserByIdAsync, updateUserAsync } from '../services/userService';
import type { UserForm } from '../types/UserForm';

const CreateUserPage: React.FC = () => {
    const { id } = useParams<{ id?: string }>();
    const [formData, setFormData] = useState<UserForm>({
        UserName: '',
        Email: '',
        FirstName: '',
        LastName: '',
        Address: '',
        PhoneNumber: '',
        DateOfBirth: new Date().toISOString().split('T')[0],
        Gender: false
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
                    setFormData(response.data);
                } catch (error: any) {
                    console.log('Error fetching user: ', error)
                    setErrors({ general: 'Không thể tải thông tin người dùng.' })
                }
                finally {
                    setLoading(false);
                }
            }
        }

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
                    <label htmlFor="UserName" className="block text-gray-700 text-sm font-bold mb-2">
                        Username:
                    </label>
                    <input
                        type="text"
                        id="UserName"
                        name="UserName"
                        value={formData.UserName}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.UserName && <p className="text-red-500 text-xs italic">{errors.UserName}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="Email" className="block text-gray-700 text-sm font-bold mb-2">
                        Email:
                    </label>
                    <input
                        type="email"
                        id="Email"
                        name="Email"
                        value={formData.Email}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.Email && <p className="text-red-500 text-xs italic">{errors.Email}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="FirstName" className="block text-gray-700 text-sm font-bold mb-2">
                        First name:
                    </label>
                    <input
                        type="text"
                        id="FirstName"
                        name="FirstName"
                        value={formData.FirstName}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.FirstName && <p className="text-red-500 text-xs italic">{errors.FirstName}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="LastName" className="block text-gray-700 text-sm font-bold mb-2">
                        Last name:
                    </label>
                    <input
                        type="text"
                        id="LastName"
                        name="LastName"
                        value={formData.LastName}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.LastName && <p className="text-red-500 text-xs italic">{errors.LastName}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="Address" className="block text-gray-700 text-sm font-bold mb-2">
                        Address:
                    </label>
                    <input
                        type="text"
                        id="Address"
                        name="Address"
                        value={formData.Email}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.Address && <p className="text-red-500 text-xs italic">{errors.Address}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="PhoneNumber" className="block text-gray-700 text-sm font-bold mb-2">
                        Phone number:
                    </label>
                    <input
                        type="tel"
                        id="PhoneNumber"
                        name="PhoneNumber"
                        value={formData.PhoneNumber}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.PhoneNumber && <p className="text-red-500 text-xs italic">{errors.PhoneNumber}</p>}
                </div>
                <div className="mb-4">
                    <label htmlFor="DateOfBirth" className="block text-gray-700 text-sm font-bold mb-2">
                        Date of birth:
                    </label>
                    <input
                        type="date"
                        id="DateOfBirth"
                        name="DateOfBirth"
                        value={formData.DateOfBirth}
                        onChange={handleChange}
                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    />
                    {errors.DateOfBirth && <p className="text-red-500 text-xs italic">{errors.DateOfBirth}</p>}
                </div>
                <div className="mb-4">
                    <label className="block text-gray-700 text-sm font-bold mb-2">
                        Gender:
                    </label>
                    <div className="flex items-center space-x-4">
                        <label className="inline-flex items-center">
                            <input
                                type="radio"
                                name="Gender"
                                value="true"
                                checked={formData.Gender === true}
                                onChange={() => setFormData({ ...formData, Gender: true })}
                                className="form-radio"
                            />
                            <span className="ml-2">Male</span>
                        </label>
                        <label className="inline-flex items-center">
                            <input
                                type="radio"
                                name="Gender"
                                value="false"
                                checked={formData.Gender === false}
                                onChange={() => setFormData({ ...formData, Gender: false })}
                                className="form-radio"
                            />
                            <span className="ml-2">Female</span>
                        </label>
                    </div>
                    {errors.Gender && <p className="text-red-500 text-xs italic">{errors.Gender}</p>}
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