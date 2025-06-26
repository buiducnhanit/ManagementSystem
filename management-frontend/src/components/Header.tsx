import React, { useEffect } from 'react'
import { logoutAsync } from '../services/authService'
import { Link, useNavigate } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { logout } from '../redux/slices/authSlice';

interface HeaderProps {
    avatar?: string;
    userName?: string;
}

const Header: React.FC<HeaderProps> = ({ avatar, userName }) => {
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

    const getInitial = () => {
        if (userName && typeof userName === 'string') {
            return userName.trim().charAt(0).toUpperCase();
        }
        return '?';
    };

    return (
        <header className="bg-white shadow px-8 py-5 flex items-center justify-between">
            <div className="text-2xl font-extrabold text-indigo-700 tracking-wide">Quản lý người dùng</div>
            <div className="flex items-center space-x-4">
                <div className="relative" ref={dropdownRef}>
                    <button
                        className="flex items-center space-x-2 focus:outline-none hover:ring-2 hover:ring-indigo-200 rounded-full transition"
                        onClick={() => setOpen((prev) => !prev)}
                    >
                        {avatar ? (
                            <img
                                src={avatar}
                                alt="avatar"
                                className="w-10 h-10 rounded-full object-cover border-2 border-indigo-200"
                            />
                        ) : (
                            <div className="w-10 h-10 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-600 font-bold text-xl border-2 border-indigo-200">
                                {getInitial()}
                            </div>
                        )}
                    </button>
                    {open && (
                        <div className="absolute right-0 mt-2 w-48 bg-white border rounded-xl shadow-lg z-10 overflow-hidden">
                            <Link
                                to="/forgot-password"
                                className="block px-5 py-3 text-sm text-gray-700 hover:bg-indigo-50"
                                onClick={() => setOpen(false)}
                            >
                                Quên mật khẩu
                            </Link>
                            <Link
                                to="/change-password"
                                className="block px-5 py-3 text-sm text-gray-700 hover:bg-indigo-50"
                                onClick={() => setOpen(false)}
                            >
                                Đổi mật khẩu
                            </Link>
                            <button
                                onClick={handleLogout}
                                className="w-full text-left px-5 py-3 text-sm text-red-600 hover:bg-red-50"
                            >
                                Đăng xuất
                            </button>
                        </div>
                    )}
                </div>
            </div>
        </header>
    )
}

export default Header