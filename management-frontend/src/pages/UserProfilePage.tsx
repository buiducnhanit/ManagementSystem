/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getUserByIdAsync } from '../services/userService';

const UserProfilePage: React.FC = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [user, setUser] = useState<any>(null);

    useEffect(() => {
        const fetchUser = async () => {
            const res = await getUserByIdAsync(id as string);
            setUser(res.data?.data);
        };
        fetchUser();
    }, [id]);

    if (!user) {
        return <div className="text-center py-10">Đang tải thông tin người dùng...</div>;
    }

    // Giả lập các thuộc tính bổ sung nếu chưa có từ backend
    const gender = user.gender || 'Nam';
    const createdAt = user.createdAt || '2024-01-01T12:00:00';
    const status = user.isActive === false ? 'Đã khóa' : 'Đang hoạt động';
    const bio = user.bio || 'Chưa có mô tả';

    return (
        <div className="min-h-screen bg-gradient-to-br py-10 px-2">
            <div className="max-w-2xl mx-auto bg-white rounded-2xl shadow-2xl p-8">
                <div className="flex flex-col items-center gap-4 mb-8">
                    <div className="w-28 h-28 rounded-full bg-indigo-100 flex items-center justify-center text-5xl font-bold text-indigo-600 shadow">
                        {user.avatarUrl
                            ? <img src={user.avatarUrl} alt="avatar" className="w-full h-full rounded-full object-cover" />
                            : (
                                <svg className="w-16 h-16 text-indigo-300" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 12c2.7 0 4.5-2.2 4.5-4.5S14.7 3 12 3 7.5 5.2 7.5 7.5 9.3 12 12 12zm0 2c-3 0-9 1.5-9 4.5V21h18v-2.5c0-3-6-4.5-9-4.5z" />
                                </svg>
                            )
                        }
                    </div>
                    <h2 className="text-3xl font-extrabold text-indigo-700">{user.firstName} {user.lastName}</h2>
                    <div className="flex gap-2 flex-wrap">
                        {user.roles?.map((role: string) => (
                            <span key={role} className="bg-indigo-100 text-indigo-700 px-3 py-1 rounded-full text-xs font-semibold shadow">{role}</span>
                        ))}
                    </div>
                    <div className="text-gray-500 italic mt-2">{bio}</div>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="space-y-4">
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Tên đăng nhập:</span>
                            <span className="ml-1">{user.userName}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Email:</span>
                            <span className="ml-1">{user.email}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Số điện thoại:</span>
                            <span className="ml-1">{user.phoneNumber}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Giới tính:</span>
                            <span className="ml-1">{gender}</span>
                        </div>
                    </div>
                    <div className="space-y-4">
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Địa chỉ:</span>
                            <span className="ml-1">{user.address}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Ngày sinh:</span>
                            <span className="ml-1">{user.dateOfBirth?.split('T')[0]}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Ngày tạo:</span>
                            <span className="ml-1">{createdAt?.split('T')[0]}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Trạng thái:</span>
                            <span className={`ml-1 font-semibold ${status === 'Đang hoạt động' ? 'text-green-600' : 'text-red-600'}`}>{status}</span>
                        </div>
                    </div>
                </div>
                <div className="mt-10 flex justify-end">
                    <button
                        className="px-5 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 shadow transition"
                        onClick={() => navigate(-1)}
                    >
                        Quay lại
                    </button>
                </div>
            </div>
        </div>
    );
};

export default UserProfilePage;