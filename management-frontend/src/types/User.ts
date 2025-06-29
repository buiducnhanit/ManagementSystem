export interface User {
    id: string
    userName: string;
    email: string;
    firstName: string;
    lastName: string;
    avatarUrl: string;
    address: string;
    phoneNumber: string;
    dateOfBirth: string;
    roles?: string[];
    isDeleted: boolean;
    createdAt: string;
    updatedAt: string;
}