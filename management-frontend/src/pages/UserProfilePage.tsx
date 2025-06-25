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

    const gender = user.gender === true ? 'Nam' : user.gender === false ? 'Nữ' : 'Không xác định';
    const createdAt = user.createdAt ? user.createdAt.split('T')[0] : 'Không rõ';
    const dateOfBirth = user.dateOfBirth ? user.dateOfBirth.split('T')[0] : 'Không rõ';
    const status = user.isDeleted ? 'Đã khóa' : 'Đang hoạt động';
    const avatar = user.avatarUrl;
    const address = user.address || 'Chưa cập nhật';
    const phoneNumber = user.phoneNumber || 'Chưa cập nhật';
    const roles = Array.isArray(user.roles) && user.roles.length > 0 ? user.roles : ['Chưa có'];

    const getInitial = () => {
        if (user.userName && typeof user.userName === 'string') {
            return user.userName.trim().charAt(0).toUpperCase();
        }
        if (user.email && typeof user.email === 'string') {
            return user.email.trim().charAt(0).toUpperCase();
        }
        return '?';
    };

    return (
        <div className="min-h-screen bg-gradient-to-br py-10 px-2">
            <div className="max-w-2xl mx-auto bg-white rounded-2xl shadow-2xl p-8">
                <div className="flex flex-col items-center gap-4 mb-8">
                    <div className="w-28 h-28 rounded-full bg-indigo-100 flex items-center justify-center text-5xl font-bold text-indigo-600 shadow">
                        {avatar
                            ? <img src={avatar} alt="avatar" className="w-full h-full rounded-full object-cover" />
                            : (
                                <span>{getInitial()}</span>
                            )
                        }
                    </div>
                    <h2 className="text-3xl font-extrabold text-indigo-700">{user.firstName} {user.lastName}</h2>
                    <div className="flex gap-2 flex-wrap">
                        {roles.map((role: string) => (
                            <span key={role} className="bg-indigo-100 text-indigo-700 px-3 py-1 rounded-full text-xs font-semibold shadow">{role}</span>
                        ))}
                    </div>
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
                            <span className="ml-1">{phoneNumber}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Giới tính:</span>
                            <span className="ml-1">{gender}</span>
                        </div>
                    </div>
                    <div className="space-y-4">
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Địa chỉ:</span>
                            <span className="ml-1">{address}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Ngày sinh:</span>
                            <span className="ml-1">{dateOfBirth}</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <span className="font-semibold text-gray-600">Ngày tạo:</span>
                            <span className="ml-1">{createdAt}</span>
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