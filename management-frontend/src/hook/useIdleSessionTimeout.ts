import { useEffect, useRef } from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { logout } from '../redux/slices/authSlice';

const IDLE_TIMEOUT = 5 * 60 * 1000;

const useIdleSessionTimeout = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    useEffect(() => {
        const resetTimer = () => {
            if (timerRef.current) clearTimeout(timerRef.current);
            timerRef.current = setTimeout(() => {
                dispatch(logout());
                navigate('/login');
            }, IDLE_TIMEOUT);
        };

        // Các sự kiện hoạt động của user
        const events = ['mousemove', 'keydown', 'mousedown', 'scroll', 'touchstart'];
        events.forEach(event =>
            window.addEventListener(event, resetTimer)
        );

        resetTimer(); // Khởi tạo timer lần đầu

        return () => {
            if (timerRef.current) clearTimeout(timerRef.current);
            events.forEach(event =>
                window.removeEventListener(event, resetTimer)
            );
        };
    }, [dispatch, navigate]);
};

export default useIdleSessionTimeout;