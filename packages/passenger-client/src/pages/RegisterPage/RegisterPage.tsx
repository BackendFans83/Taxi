import type { Component } from 'solid-js';
import { RegisterForm, Stack, Paper, H4, Caption } from '@taxi/shared';
import { useNavigate } from '@solidjs/router';

const RegisterPage: Component = () => {
  const navigate = useNavigate();

  const handleSuccess = () => {
    navigate('/profile');
  };

  return (
    <Stack
      alignItems="center"
      justifyContent="center"
      style={{ 'min-height': '100vh', 'background-color': 'var(--background)' }}
    >
      <Paper elevation="md" style={{ padding: '32px', 'max-width': '400px', width: '100%' }}>
        <Stack direction="column" gap="16px" style={{ 'margin-bottom': '24px' }}>
          <H4 style={{ 'text-align': 'center' }}>
            Регистрация
          </H4>
          <Caption style={{ 'text-align': 'center', color: 'var(--text-secondary)' }}>
            Создайте аккаунт пассажира
          </Caption>
        </Stack>
        
        <RegisterForm onSuccess={handleSuccess} />
        
        <Stack direction="row" justifyContent="center" style={{ 'margin-top': '16px' }}>
          <Caption style={{ color: 'var(--text-secondary)' }}>
            Уже есть аккаунт?{' '}
          </Caption>
          <Caption>
            <a
              href="/login"
              style={{ color: 'var(--primary)', 'text-decoration': 'none', cursor: 'pointer' }}
              onClick={(e: Event) => {
                e.preventDefault();
                navigate('/login');
              }}
            >
              Войти
            </a>
          </Caption>
        </Stack>
      </Paper>
    </Stack>
  );
};

export default RegisterPage;
