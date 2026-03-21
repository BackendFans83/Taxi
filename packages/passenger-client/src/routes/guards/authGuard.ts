import { useAuth } from '@features/auth/hooks/useAuth';

export const authGuard = () => {
  const auth = useAuth();
  if (!auth.isAuthenticated()) {
    return '/login';
  }
  return true;
};
