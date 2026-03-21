import { createSignal, createMemo } from 'solid-js';
import type { UserWithAuth, UserRole } from '@taxi/shared/types';

const [user, setUser] = createSignal<UserWithAuth | null>(null);
const [isLoading, setIsLoading] = createSignal(false);
const [error, setError] = createSignal<string | null>(null);

export const createUserEntity = () => {
  const setUserWithLoading = (userData: UserWithAuth | null) => {
    setUser(userData);
  };

  const setLoading = (loading: boolean) => {
    setIsLoading(loading);
  };

  const setErrorWithMessage = (message: string | null) => {
    setError(message);
  };

  const isAuthenticated = createMemo(() => user() !== null);
  const userRole = createMemo<UserRole | null>(() => user()?.role ?? null);
  const isAdmin = createMemo(() => user()?.role === 'admin');
  const isDriver = createMemo(() => user()?.role === 'driver');

  return {
    user,
    isLoading,
    error,
    setUser: setUserWithLoading,
    setLoading,
    setError: setErrorWithMessage,
    clearError: () => setError(null),
    isAuthenticated,
    userRole,
    isAdmin,
    isDriver,
  };
};

export type UserEntity = ReturnType<typeof createUserEntity>;
