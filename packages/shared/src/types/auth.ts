export type UserRole = 'user' | 'admin' | 'driver';

export type User = {
  id: string;
  email: string;
  name?: string;
  phone?: string;
  role: UserRole;
  createdAt?: string;
  updatedAt?: string;
};

export type LoginCredentials = {
  email: string;
  password: string;
};

export type RegisterCredentials = {
  email: string;
  password: string;
  name?: string;
  phone?: string;
};

export type UpdateProfileData = {
  name?: string;
  phone?: string;
  email?: string;
};
