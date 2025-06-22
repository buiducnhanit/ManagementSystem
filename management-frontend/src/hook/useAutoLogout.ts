import { useEffect, useRef } from 'react';
import { useDispatch } from 'react-redux';
import { useLocation, useNavigate } from 'react-router-dom';
import { logout } from '../redux/slices/authSlice';
import { refreshTokenAsync } from '../services/authService';

const IDLE_TIMEOUT_MINUTES = 5;
const ACCESS_TOKEN_REFRESH_THRESHOLD_SECONDS = 30;

const useAutoLogout = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
     const location = useLocation();

    const publicPaths = ['/login', '/register', '/forgot-password', '/reset-password', '/confirm-email'];

    const accessTokenTimerRef = useRef<number | undefined>(undefined);
    const idleActivityTimerRef = useRef<number | undefined>(undefined);

    const resetIdleTimer = () => {
        clearTimeout(idleActivityTimerRef.current);
        idleActivityTimerRef.current = setTimeout(() => {
            console.log('User idle for too long. Logging out...');
            dispatch(logout());
            navigate("/login");
        }, IDLE_TIMEOUT_MINUTES * 60 * 1000);
    };

    const startAccessTokenRefreshTimer = (expiresInISOString: string | number | Date) => {
        clearTimeout(accessTokenTimerRef.current);

        const expirationTime = new Date(expiresInISOString).getTime();
        const currentTime = Date.now();
        const timeToRefresh = expirationTime - currentTime - (ACCESS_TOKEN_REFRESH_THRESHOLD_SECONDS * 1000);

        console.log(`Access Token expires in: ${Math.floor((expirationTime - currentTime) / 1000)}s`);
        console.log(`Time until next refresh attempt: ${Math.floor(timeToRefresh / 1000)}s`);

        if (timeToRefresh > 0) {
            accessTokenTimerRef.current = setTimeout(async () => {
                const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");
                const oldRefreshToken = localStorage.getItem('refreshToken') || sessionStorage.getItem('refreshToken');

                if (oldRefreshToken && userId) {
                    try {
                        const refreshTokenResponse = await refreshTokenAsync(userId, oldRefreshToken);
                        const newAccessToken = refreshTokenResponse.data?.data?.accessToken;
                        const newRefreshToken = refreshTokenResponse.data?.data?.refreshToken;
                        const newExpiresIn = refreshTokenResponse.data?.data?.expiryTime;

                        if (newAccessToken && newExpiresIn) {
                            if (localStorage.getItem('refreshToken')) {
                                localStorage.setItem("token", newAccessToken);
                                localStorage.setItem("refreshToken", newRefreshToken);
                                localStorage.setItem("expiresIn", newExpiresIn);
                            } else {
                                sessionStorage.setItem("token", newAccessToken);
                                sessionStorage.setItem("refreshToken", newRefreshToken);
                                sessionStorage.setItem("expiresIn", newExpiresIn);
                            }
                            console.log('Access Token refreshed successfully!');
                            startAccessTokenRefreshTimer(newExpiresIn);
                            return;
                        }
                    } catch (error) {
                        console.error("Failed to refresh token:", error);
                        dispatch(logout());
                        navigate("/login");
                        return;
                    }
                }
                dispatch(logout());
                navigate("/login");

            }, timeToRefresh);
        } else {
            console.warn("Access Token already expired or close to expiration. Attempting immediate refresh or logout.");
            dispatch(logout());
            navigate("/login");
        }
    };


    useEffect(() => {
        if (publicPaths.includes(location.pathname)) return;
        const expiresIn = localStorage.getItem("expiresIn") || sessionStorage.getItem("expiresIn");
        if (expiresIn) {
            startAccessTokenRefreshTimer(expiresIn);
        } else {
            dispatch(logout());
            navigate("/login");
        }

        const activityEvents = ['mousemove', 'keydown', 'click', 'scroll'];
        activityEvents.forEach(event => {
            window.addEventListener(event, resetIdleTimer);
        });

        resetIdleTimer();

        return () => {
            clearTimeout(accessTokenTimerRef.current);
            clearTimeout(idleActivityTimerRef.current);
            activityEvents.forEach(event => {
                window.removeEventListener(event, resetIdleTimer);
            });
        };
    }, [dispatch, navigate, location.pathname]);

    return null;
};

export default useAutoLogout;