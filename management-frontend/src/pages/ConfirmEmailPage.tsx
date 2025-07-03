/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react';
import { useSearchParams, useNavigate, useLocation } from 'react-router-dom';
import { confirmEmailAsync, resendConfirmEmailAsync } from '../services/authService';

const ConfirmEmailPage: React.FC = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [status, setStatus] = useState<'pending' | 'success' | 'error'>('pending');
    const [resendStatus, setResendStatus] = useState<'idle' | 'sending' | 'sent' | 'error'>('idle');
    const [errorMsg, setErrorMsg] = useState<string>('');
    const location = useLocation();
    const email = (location.state as { email?: string } | null)?.email || '';

    useEffect(() => {
        const confirmEmail = async () => {
            const token = searchParams.get('token');
            const userId = searchParams.get('userId');
            if (!token || !userId) {
                setStatus('error');
                setErrorMsg('Liên kết xác nhận không hợp lệ hoặc đã hết hạn.');
                return;
            }
            try {
                const response = await confirmEmailAsync(userId, token);
                if(response.data.statusCode === 200){
                    setStatus('success');
                    setTimeout(() => {
                        navigate('/login');
                    }, 2000);
                } else {
                    setStatus('error');
                    setErrorMsg('Liên kết xác nhận không hợp lệ hoặc đã hết hạn.');
                }
            } catch (error: any) {
                setStatus('error');
                setErrorMsg('Đã xảy ra lỗi khi xác nhận email.');
                console.log(error);
            }
        };
        confirmEmail();
    }, [searchParams, navigate]);

    const handleResend = async () => {
        setResendStatus('sending');
        setErrorMsg('');
        
        try {
            const res = await resendConfirmEmailAsync(email);
            console.log(res)
            if (res.data.statusCode === 200) {
                setResendStatus('sent');
            } else {
                setResendStatus('error');
                setErrorMsg('Không thể gửi lại email xác nhận. Vui lòng thử lại sau.');
            }
        } catch {
            setResendStatus('error');
            setErrorMsg('Không thể gửi lại email xác nhận. Vui lòng thử lại sau.');
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100">
            <div className="bg-white p-8 rounded-lg shadow-xl w-full max-w-md text-center">
                {status === 'pending' && (
                    <>
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto mb-6"></div>
                        <h2 className="text-xl font-semibold text-gray-700 mb-2">Đang xác nhận email...</h2>
                        <p className="text-gray-500">Vui lòng chờ trong giây lát.</p>
                    </>
                )}
                {status === 'success' && (
                    <>
                        <div className="text-green-500 text-4xl mb-4">✔</div>
                        <h2 className="text-xl font-semibold text-gray-700 mb-2">Xác nhận email thành công!</h2>
                        <p className="text-gray-500">Bạn sẽ được chuyển hướng sang trang đăng nhập.</p>
                    </>
                )}
                {status === 'error' && (
                    <>
                        <div className="text-red-500 text-4xl mb-4">✖</div>
                        <h2 className="text-xl font-semibold text-gray-700 mb-2">Xác nhận email thất bại</h2>
                        <p className="text-gray-500 mb-4">{errorMsg}</p>
                        <button
                            onClick={handleResend}
                            disabled={resendStatus === 'sending' || resendStatus === 'sent'}
                            className={`w-full py-2 px-4 rounded-md text-white font-medium 
                                ${resendStatus === 'sent'
                                    ? 'bg-green-500 cursor-not-allowed'
                                    : 'bg-indigo-600 hover:bg-indigo-700'}
                                focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 mb-2`}
                        >
                            {resendStatus === 'sending'
                                ? 'Đang gửi lại...'
                                : resendStatus === 'sent'
                                    ? 'Đã gửi lại email xác nhận!'
                                    : 'Gửi lại email xác nhận'}
                        </button>
                        {resendStatus === 'error' && (
                            <div className="text-red-500 text-sm mt-2">{errorMsg}</div>
                        )}
                    </>
                )}
            </div>
        </div>
    );
};

export default ConfirmEmailPage;