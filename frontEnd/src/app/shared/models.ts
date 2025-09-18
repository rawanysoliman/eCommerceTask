export interface UserModel {
    id: number;
    username: string;
    email: string;
    role?: 'User' | 'Admin' | string;
}

export interface ProductModel {
    id: number;
    productCode?: string;
    name: string;
    price: number;
    category: string;
    minimumQuantity: number;
    discountRate?: number;
    imageUrl: string | null;
    createdAt?: string;
    updatedAt?: string;
}

export interface AuthResponseModel {
    accessToken: string;
    refreshToken: string;
    user?: UserModel;
}


