import { http } from '@taxi/shared/api/http';
import type { User } from '@taxi/shared/types';

export const userApi = {
  getById: (id: number) => {
    return http.get<User>(`/api/v1/User/${id}`);
  },
};

export default userApi;
