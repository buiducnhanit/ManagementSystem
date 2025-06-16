import { useEffect } from 'react'
import { useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom';
import { logout } from '../redux/slices/authSlice';

const useAutoLogout = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    useEffect(() => {
        const expiresIn = localStorage.getItem("expiresIn");
        if (!expiresIn) return;
        console.log(new Date(expiresIn).getTime() - Date.now());
        const timeout = new Date(expiresIn).getTime() - Date.now();
        if (timeout > 0) {
            const timer = setTimeout(() => {
                dispatch(logout());
                // localStorage.clear();
                navigate("/login");
            }, timeout);
            return () => clearTimeout(timer);
        } else {
            dispatch(logout());
            // localStorage.clear();
            navigate("/login");
        }
    }, [dispatch, navigate]);
}

export default useAutoLogout