import React from 'react';

const Footer: React.FC = () => {
    return (
        <footer className="bg-white py-2 text-center">
            <p className="text-gray-500">
                &copy; {new Date().getFullYear()} Bui Duc Nhan
            </p>
        </footer>
    );
};

export default Footer;