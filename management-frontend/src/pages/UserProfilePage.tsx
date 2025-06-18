/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react'
import type { User } from '../types/User';
import { getUserProfileAsync } from '../services/userService';

const UserProfilePage: React.FC = () => {
    const [profile, setProfile] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);
    const [errors, setErrors] = useState<string | null>(null);

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                setLoading(true);
                setErrors(null);
                const response = await getUserProfileAsync();
                setProfile(response.data);
            } catch (error: any) {
                console.log('Errors: ', error);
                setErrors('Error fetching user profile.');
            } finally {
                setLoading(false)
            }
        }

        fetchProfile();
    }, []);

    if (loading) return <div>Đang tải thông tin hồ sơ...</div>;
    if (errors) return <div className="text-red-500">{errors}</div>;

    return (
        <div>
            <h2 className="text-2xl font-bold mb-4">Hồ sơ người dùng</h2>
            {profile && (
                <div className="max-w-md">
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Tên đăng nhập:
                        </label>
                        <p className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline">
                            {profile.UserName}
                        </p>
                    </div>
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Email:
                        </label>
                        <p className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline">
                            {profile.Email}
                        </p>
                    </div>
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Họ và tên:
                        </label>
                        <p className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline">
                            {profile.FirstName} {profile.LastName}
                        </p>
                    </div>
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Địa chỉ:
                        </label>
                        <p className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline">
                            {profile.Address}
                        </p>
                    </div>
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Số điện thoại:
                        </label>
                        <p className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline">
                            {profile.PhoneNumber}
                        </p>
                    </div>
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Ngày sinh:
                        </label>
                        <p className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline">
                            {profile.DateOfBirth}
                        </p>
                    </div>
                    {/* <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Giới tính:
                        </label>
                        <p className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline">
                            {profile. ? 'Nam' : 'Nữ'}
                        </p>
                    </div> */}
                </div>
            )}
        </div>
    )
}

export default UserProfilePage