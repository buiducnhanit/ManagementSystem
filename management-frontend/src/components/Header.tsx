import React, { useEffect } from 'react'
import { logoutAsync } from '../services/authService'
import { Link, useNavigate } from 'react-router';
import { useDispatch } from 'react-redux';
import { logout } from '../redux/slices/authSlice';

interface HeaderProps {
    userName?: string;
}

const Header: React.FC<HeaderProps> = ({ userName }) => {
    const [open, setOpen] = React.useState(false);
    const dropdownRef = React.useRef<HTMLDivElement>(null);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const handleLogout = async () => {
        setOpen(false);
        const response = await logoutAsync();
        if(response.status === 200){
            dispatch(logout());
            navigate("/login");
        }
    }

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setOpen(false);
            }
        }
        if (open) {
            document.addEventListener("mousedown", handleClickOutside);
        }
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [open]);

    return (
        <header className="bg-white shadow px-6 py-4 flex items-center justify-between">
            <div className="text-xl font-bold text-indigo-700">Quản lý người dùng</div>
            <div className="flex items-center space-x-4">
                {userName && (
                    <div className="relative" ref={dropdownRef}>
                        <button
                            className="text-sm text-gray-700 hover:underline focus:outline-none"
                            onClick={() => setOpen((prev) => !prev)}
                        >
                            {userName}
                        </button>
                        {open && (
                            <div className="absolute right-0 mt-2 w-40 bg-white border rounded shadow z-10">
                                <Link
                                    to="/forgot-password"
                                    className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                                    onClick={() => setOpen(false)}
                                >
                                    Quên mật khẩu
                                </Link>
                                <Link
                                    to="/change-password"
                                    className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                                    onClick={() => setOpen(false)}
                                >
                                    Đổi mật khẩu
                                </Link>
                                <button
                                    onClick={handleLogout}
                                    className="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-gray-100"
                                >
                                    Đăng xuất
                                </button>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </header>
    )
}

export default Header