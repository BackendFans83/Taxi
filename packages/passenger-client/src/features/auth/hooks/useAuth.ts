import { createEffect } from 'solid-js';
import { createUserEntity } from '@taxi/shared/entities';
import { authApi } from '@taxi/shared/api';
import type { LoginCredentials, RegisterCredentials } from '@taxi/shared/types';

export const useAuth = () => {
  const userEntity = createUserEntity();

  const login = async (credentials: LoginCredentials) => {
    userEntity.setLoading(true);
    userEntity.clearError();
    
    try {
      const user = await authApi.login(credentials);
      userEntity.setUser(user);
      return user;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Ошибка при входе';
      userEntity.setError(errorMessage);
      throw error;
    } finally {
      userEntity.setLoading(false);
    }
  };

  const register = async (credentials: RegisterCredentials) => {
    userEntity.setLoading(true);
    userEntity.clearError();
    
    try {
      const user = await authApi.register(credentials);
      userEntity.setUser(user);
      return user;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Ошибка при регистрации';
      userEntity.setError(errorMessage);
      throw error;
    } finally {
      userEntity.setLoading(false);
    }
  };

  const logout = async () => {
    try {
      await authApi.logout();
      userEntity.setUser(null);
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  const loadCurrentUser = async () => {
    userEntity.setLoading(true);
    userEntity.clearError();
    
    try {
      const user = await authApi.getCurrentUser();
      userEntity.setUser(user);
      return user;
    } catch (error) {
      userEntity.setError('Не удалось загрузить данные пользователя');
      throw error;
    } finally {
      userEntity.setLoading(false);
    }
  };

  // Попытка загрузить текущего пользователя при инициализации
  createEffect(() => {
    loadCurrentUser();
  });

  return {
    ...userEntity,
    login,
    register,
    logout,
    loadCurrentUser,
  };
};

export default useAuth;
