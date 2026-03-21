import { http } from './http';
import type { User, LoginCredentials, RegisterCredentials } from '../types';

export const authApi = {
  login: (credentials: LoginCredentials) => {
    return http.post<User>('/auth/login', credentials);
  },

  register: (credentials: RegisterCredentials) => {
    return http.post<User>('/auth/register', credentials);
  },

  logout: () => {
    return http.post('/auth/logout');
  },

  getCurrentUser: () => {
    return http.get<User>('/auth/me');
  },

  updateProfile: (data: Partial<User>) => {
    return http.put<User>('/auth/profile', data);
  },
};

export default authApi;
