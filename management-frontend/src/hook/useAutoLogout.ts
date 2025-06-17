import { useEffect } from 'react'
import { useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom';
import { logout } from '../redux/slices/authSlice';
import { refreshTokenAsync } from '../services/authService';
import { store } from '../redux/store';

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
            const oldRefreshToken = localStorage.getItem('refreshToken');
            console.log(oldRefreshToken)
            if (oldRefreshToken) {
                refreshTokenAsync({ refreshToken: oldRefreshToken })
                    .then(refreshTokenResponse => {
                        const newAccessToken = refreshTokenResponse.data.data.accessToken;
                        if (newAccessToken) {
                            localStorage.setItem("token", newAccessToken);
                            window.location.reload();
                        }
                        else {
                            store.dispatch(logout());
                            navigate('/login')
                        }
                    })
                    .catch(() => {
                        store.dispatch(logout());
                        navigate('/login')
                    })
            }
            dispatch(logout());
            // localStorage.clear();
            navigate("/login");
        }
    }, [dispatch, navigate]);
}

export default useAutoLogout