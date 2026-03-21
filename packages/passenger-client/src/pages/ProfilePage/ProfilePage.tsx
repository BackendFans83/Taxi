import type { Component } from 'solid-js';
import { ProfileForm, Stack, Paper, H6, Button, CircularProgress } from '@taxi/shared';
import { useNavigate } from '@solidjs/router';
import { useAuth } from '@features/auth/hooks/useAuth';
import type { User, UserWithAuth } from '@taxi/shared/types';

const ProfilePage: Component = () => {
  const navigate = useNavigate();
  const auth = useAuth();

  const handleSuccess = (data: User) => {
    console.log('Profile updated:', data);
  };

  const handleLogout = async () => {
    await auth.logout();
    navigate('/login');
  };

  // Если загрузка, показываем индикатор
  if (auth.isLoading()) {
    return (
      <Stack alignItems="center" justifyContent="center" style={{ 'min-height': '100vh' }}>
        <CircularProgress />
      </Stack>
    );
  }

  // Если не авторизован, редирект на login
  if (!auth.isAuthenticated()) {
    navigate('/login');
    return null;
  }

  return (
    <Stack direction="column" style={{ 'min-height': '100vh', 'background-color': 'var(--background)' }}>
      {/* Header */}
      <Paper elevation="sm" style={{ padding: '16px 24px', 'margin-bottom': '24px' }}>
        <Stack direction="row" justifyContent="space-between" alignItems="center">
          <H6>Профиль пассажира</H6>
          <Button variant="outlined" onClick={handleLogout}>
            Выйти
          </Button>
        </Stack>
      </Paper>

      {/* Content */}
      <Stack alignItems="center" style={{ padding: '0 16px' }}>
        <Paper elevation="md" style={{ 'max-width': '500px', width: '100%' }}>
          <ProfileForm
            initialData={auth.user() as UserWithAuth | undefined}
            onSuccess={handleSuccess}
          />
        </Paper>
      </Stack>
    </Stack>
  );
};

export default ProfilePage;
