import React, { useEffect, useState } from 'react'
import type { User } from '../types/User';
// import { getAllUsersAsync } from '../services/userService';

const mockUsers: User[] = [
    {
        Id: "1",
        UserName: "admin",
        Email: "admin@example.com",
        LastName: "User",
        FirstName: "Admin",
        Role: "Admin",
        AvatarUrl: "",
        Address: "123 Admin St",
        PhoneNumber: "1234567890",
        DateOfBirth: "1990-01-01"
    },
    {
        Id: "2",
        UserName: "john",
        Email: "john@example.com",
        LastName: "Doe",
        FirstName: "John",
        Role: "User",
        AvatarUrl: "",
        Address: "456 John Ave",
        PhoneNumber: "2345678901",
        DateOfBirth: "1992-02-02"
    },
    {
        Id: "3",
        UserName: "jane",
        Email: "jane@example.com",
        LastName: "Smith",
        FirstName: "Jane",
        Role: "User",
        AvatarUrl: "",
        Address: "789 Jane Blvd",
        PhoneNumber: "3456789012",
        DateOfBirth: "1994-03-03"
    },
];

const UserListPage: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);

    useEffect(() => {
        // const fetchUsers = async () => {
        //     const response = await getAllUsersAsync();
        //     setUsers(response.data.data || []);
        // };
        // fetchUsers();
        setUsers(mockUsers);
    }, [])
    
    return (
        <div>
            <h2 className="text-2xl font-bold mb-4">Danh sách người dùng</h2>
            <table className="min-w-full bg-white border rounded shadow">
                <thead>
                    <tr>
                        <th className="py-2 px-4 border-b">Tên đăng nhập</th>
                        <th className="py-2 px-4 border-b">Họ tên</th>
                        <th className="py-2 px-4 border-b">Email</th>
                        <th className="py-2 px-4 border-b">Vai trò</th>
                        <th className="py-2 px-4 border-b">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map((user) => (
                        <tr key={user.Id}>
                            <td className="py-2 px-4 border-b">{user.UserName}</td>
                            <td className="py-2 px-4 border-b">{user.FirstName} {user.LastName}</td>
                            <td className="py-2 px-4 border-b">{user.Email}</td>
                            <td className="py-2 px-4 border-b">{user.Role}</td>
                            <td className="py-2 px-4 border-b">
                                <button className="text-indigo-600 hover:underline mr-2">Sửa</button>
                                <button className="text-red-600 hover:underline">Xóa</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}

export default UserListPage