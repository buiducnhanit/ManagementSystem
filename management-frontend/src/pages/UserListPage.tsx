/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react'
import type { User } from '../types/User';
import { useNavigate } from 'react-router-dom';
import { deleteUserAsync, getAllUsersAsync } from '../services/userService';
import { getUserRole } from '../utils/helper';
import { addUserRoleAsync, removeUserRoleAsync } from '../services/authService';

const PAGE_SIZE = 5;
const ALL_ROLES = ['Admin', 'Manager', 'User'];

const UserListPage: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [search, setSearch] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const navigate = useNavigate();
    const userRole = getUserRole();

    useEffect(() => {
        const fetchUsers = async () => {
            const response = await getAllUsersAsync();
            if (response.data && response.data.data) {
                setUsers(response.data.data);
            }
        };
        fetchUsers();
    }, []);

    const handleEdit = (id: string) => {
        navigate(`/users/edit/${id}`);
    };

    const handleDelete = async (id: string) => {
        if (window.confirm('Bạn có chắc muốn xóa người dùng này?')) {
            try {
                await deleteUserAsync(id);
                setUsers(prev => prev.filter(user => user.id !== id));
            } catch (error: any) {
                alert('Xóa người dùng thất bại!');
                console.log(error)
            }
        }
    };

    const filteredUsers = users.filter(
        user =>
            user.userName.toLowerCase().includes(search.toLowerCase()) ||
            user.email.toLowerCase().includes(search.toLowerCase()) ||
            `${user.firstName} ${user.lastName}`.toLowerCase().includes(search.toLowerCase())
    );

    const totalPages = Math.ceil(filteredUsers.length / PAGE_SIZE);
    const paginatedUsers = filteredUsers.slice(
        (currentPage - 1) * PAGE_SIZE,
        currentPage * PAGE_SIZE
    );

    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearch(e.target.value);
        setCurrentPage(1);
    };

    const handleAddRole = async (userId: string, role: string) => {
        try {
            await addUserRoleAsync(userId, [role]);
            setUsers(prev =>
                prev.map(user =>
                    user.id === userId && !user.roles?.includes(role)
                        ? { ...user, roles: user.roles ? [...user.roles, role] : [role] }
                        : user
                )
            );
        } catch (error: any) {
            alert('Thêm vai trò thất bại!');
            console.log(error);
        }
    }

    const handleRemoveRole = async (userId: string, role: string) => {
        try {
            await removeUserRoleAsync(userId, [role]);
            setUsers(prev =>
                prev.map(user =>
                    user.id === userId
                        ? { ...user, roles: user.roles?.filter(r => r !== role) }
                        : user
                )
            );
        } catch (error: any) {
            alert('Xóa vai trò thất bại!');
            console.log(error);
        }
    }

    return (
        <div className="max-w-7xl mx-auto p-6 bg-white rounded-xl shadow-lg mt-8">
            <h2 className="text-3xl font-extrabold mb-6 text-indigo-700 flex items-center gap-2">
                <svg className="w-8 h-8 text-indigo-500" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" d="M17 20h5v-2a4 4 0 00-3-3.87M9 20H4v-2a4 4 0 013-3.87m13-7V7a4 4 0 00-4-4H7a4 4 0 00-4 4v2m16 0a4 4 0 01-3 3.87M4 10a4 4 0 003 3.87m0 0v2a4 4 0 004 4h4a4 4 0 004-4v-2m-8 0a4 4 0 01-3-3.87"></path></svg>
                Danh sách người dùng
            </h2>
            <div className="mb-6 flex flex-col md:flex-row md:justify-between md:items-center gap-4">
                <input
                    type="text"
                    placeholder="Tìm kiếm tên, email, họ tên..."
                    value={search}
                    onChange={handleSearchChange}
                    className="border border-gray-300 px-4 py-2 rounded-lg w-full md:w-1/3 focus:outline-none focus:ring-2 focus:ring-indigo-400"
                />
            </div>
            <div className="overflow-x-auto rounded-lg shadow">
                <table className="min-w-full bg-white">
                    <thead className='text-left bg-indigo-50'>
                        <tr>
                            <th className="py-3 px-4 border-b font-semibold text-gray-700">Tên đăng nhập</th>
                            <th className="py-3 px-4 border-b font-semibold text-gray-700">Họ tên</th>
                            <th className="py-3 px-4 border-b font-semibold text-gray-700">Email</th>
                            <th className="py-3 px-4 border-b font-semibold text-gray-700">Số điện thoại</th>
                            <th className="py-3 px-4 border-b font-semibold text-gray-700">Địa chỉ</th>
                            <th className="py-3 px-4 border-b font-semibold text-gray-700">Ngày sinh</th>
                            <th className="py-3 px-4 border-b font-semibold text-gray-700">Vai trò</th>
                            {(userRole !== 'User') && (
                                <th className="py-3 px-4 border-b font-semibold text-gray-700">Thao tác</th>
                            )}
                        </tr>
                    </thead>
                    <tbody>
                        {paginatedUsers.map((user) => (
                            <tr key={user.id} onClick={() => navigate(`/users/${user.id}`)} className="hover:bg-indigo-50 transition-colors hover:cursor-pointer">
                                <td className="py-2 px-4 border-b">{user.userName}</td>
                                <td className="py-2 px-4 border-b">{user.firstName} {user.lastName}</td>
                                <td className="py-2 px-4 border-b">{user.email}</td>
                                <td className="py-2 px-4 border-b">{user.phoneNumber}</td>
                                <td className="py-2 px-4 border-b">{user.address}</td>
                                <td className="py-2 px-4 border-b">{user.dateOfBirth?.split('T')[0]}</td>
                                <td className="py-2 px-4 border-b">
                                    <div className="flex flex-wrap items-center gap-2">
                                        {Array.isArray(user.roles) && user.roles.length > 0 ? (
                                            user.roles.map(role => (
                                                <span
                                                    key={role}
                                                    className="inline-flex items-center bg-indigo-100 text-indigo-700 px-2 py-0.5 rounded-full text-xs font-medium"
                                                >
                                                    {role}
                                                    {userRole === 'Admin' && (
                                                        <button
                                                            className="ml-1 text-xs text-red-500 hover:text-white hover:bg-red-500 rounded-full transition-colors duration-150 w-4 h-4 flex items-center justify-center"
                                                            onClick={e => { e.stopPropagation(); handleRemoveRole(user.id, role) }}
                                                            title="Xóa vai trò"
                                                            style={{ lineHeight: 1 }}
                                                        >
                                                            ×
                                                        </button>
                                                    )}
                                                </span>
                                            ))
                                        ) : (
                                            <span className="text-gray-400 italic">Chưa có</span>
                                        )}
                                        {userRole === 'Admin' && (
                                            <select
                                                className="ml-2 border rounded px-1 py-0.5 text-xs bg-white"
                                                defaultValue=""
                                                onClick={e => e.stopPropagation()}
                                                onChange={e => {
                                                    if (e.target.value) {
                                                        handleAddRole(user.id, e.target.value);
                                                        e.target.value = '';
                                                    }
                                                }}
                                            >
                                                <option value="" disabled>+ Thêm vai trò</option>
                                                {ALL_ROLES.filter(r => !user.roles?.includes(r)).map(role => (
                                                    <option key={role} value={role}>{role}</option>
                                                ))}
                                            </select>
                                        )}
                                    </div>
                                </td>
                                {(userRole !== 'User') && (
                                    <td className="py-2 px-4 border-b">
                                        {(userRole === 'Admin' || userRole === 'Manager') && (
                                            <button
                                                onClick={e => { e.stopPropagation(); handleEdit(user.id); }}
                                                className="text-indigo-600 hover:underline mr-2 font-medium"
                                            >
                                                Sửa
                                            </button>
                                        )}
                                        {(userRole === 'Admin') && (
                                            <button
                                                onClick={e => { e.stopPropagation(); handleDelete(user.id); }}
                                                className="text-red-600 hover:underline font-medium"
                                            >
                                                Xóa
                                            </button>
                                        )}
                                    </td>
                                )}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <div className="flex justify-end mt-6 space-x-2">
                <button
                    className="px-4 py-2 border rounded-lg bg-indigo-50 hover:bg-indigo-100 text-indigo-700 font-semibold disabled:opacity-50"
                    onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                    disabled={currentPage === 1}
                >
                    Trước
                </button>
                <span className="px-3 py-2 text-gray-600 font-medium">
                    Trang {currentPage} / {totalPages}
                </span>
                <button
                    className="px-4 py-2 border rounded-lg bg-indigo-50 hover:bg-indigo-100 text-indigo-700 font-semibold disabled:opacity-50"
                    onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
                    disabled={currentPage === totalPages}
                >
                    Sau
                </button>
            </div>
        </div>
    )
}

export default UserListPage