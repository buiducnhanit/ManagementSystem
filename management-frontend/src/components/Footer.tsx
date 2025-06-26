import React from 'react';

const Footer: React.FC = () => {
    return (
        <footer className="bg-white border-t py-4 text-center mt-8">
            <p className="text-gray-500 text-sm">
                &copy; {new Date().getFullYear()} Bui Duc Nhan. All rights reserved.
            </p>
        </footer>
    );
};

export default Footer;