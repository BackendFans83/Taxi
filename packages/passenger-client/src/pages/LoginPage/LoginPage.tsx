import type { Component } from 'solid-js';
import { LoginForm, Stack, Paper, H4, Caption } from '@taxi/shared';
import { useNavigate } from '@solidjs/router';

const LoginPage: Component = () => {
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
            Вход
          </H4>
          <Caption style={{ 'text-align': 'center', color: 'var(--text-secondary)' }}>
            Войдите в свой аккаунт
          </Caption>
        </Stack>
        
        <LoginForm onSuccess={handleSuccess} />
        
        <Stack direction="row" justifyContent="center" style={{ 'margin-top': '16px' }}>
          <Caption style={{ color: 'var(--text-secondary)' }}>
            Нет аккаунта?{' '}
          </Caption>
          <Caption>
            <a
              href="/register"
              style={{ color: 'var(--primary)', 'text-decoration': 'none', cursor: 'pointer' }}
              onClick={(e: Event) => {
                e.preventDefault();
                navigate('/register');
              }}
            >
              Зарегистрироваться
            </a>
          </Caption>
        </Stack>
      </Paper>
    </Stack>
  );
};

export default LoginPage;
