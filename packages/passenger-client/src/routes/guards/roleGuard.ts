import { useAuth } from '@features/auth/hooks/useAuth';

export const roleGuard = (requiredRole: string) => () => {
  const auth = useAuth();

  if (auth.userRole() !== requiredRole) {
    return '/unauthorized';
  }
  return true;
};
