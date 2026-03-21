import { http } from '@taxi/shared/api/http';
import type { User } from '@taxi/shared/types';

export const userApi = {
  getById: (id: string) => {
    return http.get<User>(`/users/${id}`);
  },

  update: (id: string, data: Partial<User>) => {
    return http.put<User>(`/users/${id}`, data);
  },

  delete: (id: string) => {
    return http.delete(`/users/${id}`);
  },
};

export default userApi;
