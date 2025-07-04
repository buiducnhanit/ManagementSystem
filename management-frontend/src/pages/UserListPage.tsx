/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react'
import type { User } from '../types/User';
import { useNavigate } from 'react-router-dom';
import { deleteUserAsync, getAllRolesAsync, getAllUsersAsync } from '../services/userService';
import { getUserRole } from '../utils/helper';
import { addUserRoleAsync, removeUserRoleAsync, unlockUserAsync } from '../services/authService';
import type { Role } from '../types/Role';

const PAGE_SIZE = 5;
// const ALL_ROLES = ['Admin', 'Manager', 'User'];

const UserListPage: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [search, setSearch] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const [roles, setRoles] = useState<Role[]>([]);
    const [openRoleDropdown, setOpenRoleDropdown] = useState<string | null>(null);
    const navigate = useNavigate();
    const userRole = getUserRole();

    useEffect(() => {
        const fetchUsers = async () => {
            const response = await getAllUsersAsync();
            if (response.data && response.data.data) {
                setUsers(response.data.data);
            }
        };

        const fetchRoles = async () => {
            const response = await getAllRolesAsync();
            // console.log(response)
            if (response.data && response.data.data) {
                setRoles(response.data.data)
            }
        }

        fetchUsers();
        fetchRoles();
    }, []);

    const handleEdit = (id: string) => {
        navigate(`/users/edit/${id}`);
    };

    const handleDelete = async (id: string) => {
        if (window.confirm('Bạn có chắc muốn xóa người dùng này?')) {
            try {
                await deleteUserAsync(id);
                // setUsers(prev => prev.filter(user => user.id !== id));
                window.location.reload(); // Reload the page to reflect changes
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
    
    const handleUnlock = async (id: string) => {
        if (window.confirm('Bạn có chắc muốn mở khóa người dùng này?')) {
            try {
                await unlockUserAsync(id);
                setUsers(prev =>
                    prev.map(user =>
                        user.id === id ? { ...user, isDeleted: false } : user
                    )
                );
            } catch (error: any) {
                alert('Mở khóa người dùng thất bại!');
                console.log(error);
            }
        }
    };

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
                                <td className="py-2 px-4 border-b">
                                    <div className="flex flex-wrap items-center gap-2">
                                        {Array.isArray(user.roles) && user.roles.length > 0 ? (
                                            user.roles.map(role => (
                                                <span
                                                    key={role}
                                                    className="inline-flex items-center bg-indigo-100 text-indigo-700 px-2 py-0.5 rounded-full text-xs font-medium"
                                                >
                                                    {role}
                                                    {userRole === 'Admin' && !user.isDeleted && (
                                                        <button
                                                            className="ml-1 w-5 h-5 flex items-center justify-center rounded-full bg-red-100 text-red-500 hover:bg-red-500 hover:text-white transition"
                                                            onClick={e => { e.stopPropagation(); handleRemoveRole(user.id, role) }}
                                                            title="Xóa vai trò"
                                                            style={{ lineHeight: 1 }}
                                                        >
                                                            <svg className="w-3 h-3" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                                                                <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                                                            </svg>
                                                        </button>
                                                    )}
                                                </span>
                                            ))
                                        ) : (
                                            <span className="text-gray-400 italic">Chưa có</span>
                                        )}
                                        {userRole === 'Admin' && !user.isDeleted && roles
                                            .map(r => r.name)
                                            .filter(roleName => !user.roles?.includes(roleName)).length > 0 && (
                                                <div className="relative">
                                                    <button
                                                        className="ml-1 w-6 h-6 flex items-center justify-center rounded-full bg-green-100 text-green-700 hover:bg-green-600 hover:text-white transition"
                                                        onClick={e => {
                                                            e.stopPropagation();
                                                            setOpenRoleDropdown(openRoleDropdown === user.id ? null : user.id);
                                                        }}
                                                        title="Thêm vai trò"
                                                    >
                                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                                                            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4v16m8-8H4" />
                                                        </svg>
                                                    </button>
                                                    {openRoleDropdown === user.id && (
                                                        <div
                                                            className="absolute left-0 mt-2 z-10 bg-white border rounded shadow-lg min-w-[120px]"
                                                            onClick={e => e.stopPropagation()}
                                                        >
                                                            {roles
                                                                .map(r => r.name)
                                                                .filter(roleName => !user.roles?.includes(roleName))
                                                                .map(roleName => (
                                                                    <button
                                                                        key={roleName}
                                                                        className="block w-full text-left px-4 py-2 text-xs hover:bg-green-100 transition"
                                                                        onClick={e => {
                                                                            e.stopPropagation();
                                                                            handleAddRole(user.id, roleName);
                                                                            setOpenRoleDropdown(null);
                                                                        }}
                                                                    >
                                                                        {roleName}
                                                                    </button>
                                                                ))}
                                                        </div>
                                                    )}
                                                </div>
                                            )}
                                    </div>
                                </td>
                                {(userRole !== 'User') && (
                                    <td className="py-2 px-4 border-b">
                                        {user.isDeleted ? (
                                            <button
                                                onClick={e => {
                                                    e.stopPropagation();
                                                    handleUnlock(user.id);
                                                }}
                                                className="inline-flex items-center px-3 py-1 bg-green-100 text-green-700 rounded-lg hover:bg-green-600 hover:text-white transition font-medium"
                                            >
                                                <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 17v-6m0 0V7m0 4h4m-4 0H8m8 4a4 4 0 11-8 0 4 4 0 018 0z" />
                                                </svg>
                                                Mở khóa
                                            </button>
                                        ) : (
                                            <>
                                                {(userRole === 'Admin' || userRole === 'Manager') && (
                                                    <button
                                                        onClick={e => { e.stopPropagation(); handleEdit(user.id); }}
                                                        className="inline-flex items-center px-3 py-1 bg-indigo-100 text-indigo-700 rounded-lg hover:bg-indigo-600 hover:text-white transition font-medium mr-2"
                                                    >
                                                        <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                                                            <path strokeLinecap="round" strokeLinejoin="round" d="M15.232 5.232l3.536 3.536M9 11l6 6M3 17v4h4l10.293-10.293a1 1 0 000-1.414l-3.586-3.586a1 1 0 00-1.414 0L3 17z" />
                                                        </svg>
                                                        Sửa
                                                    </button>
                                                )}
                                                {(userRole === 'Admin') && (
                                                    <button
                                                        onClick={e => { e.stopPropagation(); handleDelete(user.id); }}
                                                        className="inline-flex items-center px-3 py-1 bg-red-100 text-red-600 rounded-lg hover:bg-red-600 hover:text-white transition font-medium"
                                                    >
                                                        <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                                                            <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                                                        </svg>
                                                        Xóa
                                                    </button>
                                                )}
                                            </>
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