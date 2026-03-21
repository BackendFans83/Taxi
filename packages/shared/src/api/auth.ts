import { http } from './http';
import type { User, UserWithAuth, LoginCredentials, RegisterCredentials } from '../types';

export const authApi = {
  login: (credentials: LoginCredentials) => {
    return http.post<User>('/api/v1/auth/login', credentials);
  },

  register: (credentials: RegisterCredentials) => {
    return http.post<User>('/api/v1/auth/register', credentials);
  },

  logout: () => {
    return http.post('/api/v1/auth/logout');
  },

  refresh: () => {
    return http.post('/api/v1/auth/refresh');
  },

  getCurrentUser: () => {
    return http.get<UserWithAuth>('/api/v1/User/me');
  },

  updateProfile: (data: Partial<User>) => {
    return http.put<User>('/api/v1/User/me', data);
  },

  sendEmailVerificationCode: (email: string) => {
    return http.post('/api/v1/auth/email/send-verification-code', { email });
  },

  verifyEmail: (token: string, email: string, code: string) => {
    return http.post('/api/v1/auth/email/verify', { token, email, code });
  },

  changePassword: (oldPassword: string, newPassword: string) => {
    return http.patch('/api/v1/auth/change-password', { oldPassword, newPassword });
  },
};

export default authApi;
