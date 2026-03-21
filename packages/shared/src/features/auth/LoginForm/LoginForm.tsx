import type { Component } from 'solid-js';
import { createSignal } from 'solid-js';
import { TextField, Button, Alert, Stack } from '@taxi/shared/ui';
import { authApi } from '@taxi/shared/api';
import type { LoginCredentials } from '@taxi/shared/types';

export interface LoginFormProps {
  onSuccess?: () => void;
  onError?: (error: Error) => void;
}

export const LoginForm: Component<LoginFormProps> = (props) => {
  const [email, setEmail] = createSignal('');
  const [password, setPassword] = createSignal('');
  const [isLoading, setIsLoading] = createSignal(false);
  const [error, setError] = createSignal<string | null>(null);
  const [emailError, setEmailError] = createSignal<string | null>(null);
  const [passwordError, setPasswordError] = createSignal<string | null>(null);

  const validateEmail = (value: string) => {
    if (!value) return 'Введите email';
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(value)) return 'Некорректный email';
    return null;
  };

  const validatePassword = (value: string) => {
    if (!value) return 'Введите пароль';
    if (value.length < 6) return 'Пароль должен быть не менее 6 символов';
    return null;
  };

  const handleEmailBlur = (e: Event) => {
    const value = (e.currentTarget as HTMLInputElement).value;
    setEmailError(validateEmail(value));
  };

  const handlePasswordBlur = (e: Event) => {
    const value = (e.currentTarget as HTMLInputElement).value;
    setPasswordError(validatePassword(value));
  };

  const handleSubmit = async (e: Event) => {
    e.preventDefault();
    
    // Валидация перед отправкой
    const emailErr = validateEmail(email());
    const passwordErr = validatePassword(password());
    
    if (emailErr || passwordErr) {
      setEmailError(emailErr);
      setPasswordError(passwordErr);
      return;
    }
    
    setIsLoading(true);
    setError(null);

    try {
      await authApi.login({ email: email(), password: password() } as LoginCredentials);
      props.onSuccess?.();
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Ошибка при входе';
      setError(errorMessage);
      props.onError?.(err instanceof Error ? err : new Error(errorMessage));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <Stack direction="column" gap="1em">
        {error() && <Alert type="error">{error()}</Alert>}

        <TextField
          label="Email"
          type="email"
          value={email()}
          onInput={(e: Event) => {
            setEmail((e.currentTarget as HTMLInputElement).value);
            setEmailError(null);
          }}
          onBlur={handleEmailBlur}
          error={emailError() ?? undefined}
          placeholder="Введите email"
          required
        />

        <TextField
          label="Пароль"
          type="password"
          value={password()}
          onInput={(e: Event) => {
            setPassword((e.currentTarget as HTMLInputElement).value);
            setPasswordError(null);
          }}
          onBlur={handlePasswordBlur}
          error={passwordError() ?? undefined}
          placeholder="Введите пароль"
          required
        />

        <Button
          type="submit"
          variant="primary"
          disabled={isLoading()}
        >
          {isLoading() ? 'Вход...' : 'Войти'}
        </Button>
      </Stack>
    </form>
  );
};

export default LoginForm;
