import React, { useEffect, useState } from 'react'
import type { User } from '../types/User';
import { useNavigate } from 'react-router-dom';
import { getAllUsersAsync } from '../services/userService';

// const mockUsers: User[] = [
//     {
//         id: "1",
//         userName: "admin",
//         email: "admin@example.com",
//         lastName: "User",
//         firstName: "Admin",
//         role: "Admin",
//         avatarUrl: "",
//         address: "123 Admin St",
//         phoneNumber: "1234567890",
//         dateOfBirth: "1990-01-01"
//     },
//     {
//         id: "2",
//         userName: "john",
//         email: "john@example.com",
//         lastName: "Doe",
//         firstName: "John",
//         role: "User",
//         avatarUrl: "",
//         address: "456 John Ave",
//         phoneNumber: "2345678901",
//         dateOfBirth: "1992-02-02"
//     },
//     {
//         id: "3",
//         userName: "jane",
//         email: "jane@example.com",
//         lastName: "Smith",
//         firstName: "Jane",
//         role: "User",
//         avatarUrl: "",
//         address: "789 Jane Blvd",
//         phoneNumber: "3456789012",
//         dateOfBirth: "1994-03-03"
//     },
//     {
//         id: "4",
//         userName: "alice",
//         email: "alice@example.com",
//         lastName: "Wonderland",
//         firstName: "Alice",
//         role: "User",
//         avatarUrl: "",
//         address: "123 Fairy St",
//         phoneNumber: "4567890123",
//         dateOfBirth: "1995-04-04"
//     },
//     {
//         id: "5",
//         userName: "bob",
//         email: "bob@example.com",
//         lastName: "Builder",
//         firstName: "Bob",
//         role: "User",
//         avatarUrl: "",
//         address: "321 Build Ave",
//         phoneNumber: "5678901234",
//         dateOfBirth: "1996-05-05"
//     },
//     {
//         id: "5",
//         userName: "bob",
//         email: "bob@example.com",
//         lastName: "Builder",
//         firstName: "Bob",
//         role: "User",
//         avatarUrl: "",
//         address: "321 Build Ave",
//         phoneNumber: "5678901234",
//         dateOfBirth: "1996-05-05"
//     },
//     {
//         id: "5",
//         userName: "bob",
//         email: "bob@example.com",
//         lastName: "Builder",
//         firstName: "Bob",
//         role: "User",
//         avatarUrl: "",
//         address: "321 Build Ave",
//         phoneNumber: "5678901234",
//         dateOfBirth: "1996-05-05"
//     },
// ];

const PAGE_SIZE = 5;

const UserListPage: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [search, setSearch] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUsers = async () => {
            const response = await getAllUsersAsync();
            if (response.data && response.data.data) {
                setUsers(response.data.data);
            }
            // setUsers(mockUsers);
        };
        fetchUsers();
    }, []);

    const handleEdit = (id: string) => {
        navigate(`/users/edit/${id}`);
    };

    const handleDelete = (id: string) => {
        if (window.confirm('Bạn có chắc muốn xóa người dùng này?')) {
            setUsers(prev => prev.filter(user => user.id !== id));
        }
    };

    // Lọc theo search
    const filteredUsers = users.filter(
        user =>
            user.userName.toLowerCase().includes(search.toLowerCase()) ||
            user.email.toLowerCase().includes(search.toLowerCase()) ||
            `${user.firstName} ${user.lastName}`.toLowerCase().includes(search.toLowerCase())
    );

    // Phân trang
    const totalPages = Math.ceil(filteredUsers.length / PAGE_SIZE);
    const paginatedUsers = filteredUsers.slice(
        (currentPage - 1) * PAGE_SIZE,
        currentPage * PAGE_SIZE
    );

    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearch(e.target.value);
        setCurrentPage(1);
    };

    return (
        <div>
            <h2 className="text-2xl font-bold mb-4">Danh sách người dùng</h2>
            <div className="mb-4 flex justify-between">
                <input
                    type="text"
                    placeholder="Tìm kiếm tên, email, họ tên..."
                    value={search}
                    onChange={handleSearchChange}
                    className="border px-3 py-2 rounded w-1/3"
                />
            </div>
            <table className="min-w-full bg-white border rounded shadow">
                <thead className='text-left bg-gray-100'>
                    <tr>
                        <th className="py-2 px-4 border-b">Tên đăng nhập</th>
                        <th className="py-2 px-4 border-b">Họ tên</th>
                        <th className="py-2 px-4 border-b">Email</th>
                        <th className="py-2 px-4 border-b">Số điện thoại</th>
                        <th className="py-2 px-4 border-b">Địa chỉ</th>
                        <th className="py-2 px-4 border-b">Ngày sinh</th>
                        <th className="py-2 px-4 border-b">Vai trò</th>
                        <th className="py-2 px-4 border-b">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {paginatedUsers.map((user) => (
                        <tr key={user.id}>
                            <td className="py-2 px-4 border-b">{user.userName}</td>
                            <td className="py-2 px-4 border-b">{user.firstName} {user.lastName}</td>
                            <td className="py-2 px-4 border-b">{user.email}</td>
                            <td className="py-2 px-4 border-b">{user.phoneNumber}</td>
                            <td className="py-2 px-4 border-b">{user.address}</td>
                            <td className="py-2 px-4 border-b">{user.dateOfBirth.split('T')[0]}</td>
                            <td className="py-2 px-4 border-b">{user.role}</td>
                            <td className="py-2 px-4 border-b">
                                <button onClick={() => handleEdit(user.id)} className="text-indigo-600 hover:underline mr-2">Sửa</button>
                                <button onClick={() => handleDelete(user.id)} className="text-red-600 hover:underline">Xóa</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <div className="flex justify-end mt-4 space-x-2">
                <button
                    className="px-3 py-1 border rounded disabled:opacity-50"
                    onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                    disabled={currentPage === 1}
                >
                    Trước
                </button>
                <button
                    className="px-3 py-1 border rounded disabled:opacity-50"
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