import type { Component } from 'solid-js';
import { createSignal } from 'solid-js';
import { TextField, Button, Alert, Stack } from '@taxi/shared/ui';
import { authApi } from '@taxi/shared/api';
import type { RegisterCredentials } from '@taxi/shared/types';

export interface RegisterFormProps {
  onSuccess?: () => void;
  onError?: (error: Error) => void;
}

export const RegisterForm: Component<RegisterFormProps> = (props) => {
  const [email, setEmail] = createSignal('');
  const [password, setPassword] = createSignal('');
  const [confirmPassword, setConfirmPassword] = createSignal('');
  const [name, setName] = createSignal('');
  const [phone, setPhone] = createSignal('');
  const [isLoading, setIsLoading] = createSignal(false);
  const [error, setError] = createSignal<string | null>(null);
  
  // Ошибки валидации для полей
  const [emailError, setEmailError] = createSignal<string | null>(null);
  const [passwordError, setPasswordError] = createSignal<string | null>(null);
  const [confirmPasswordError, setConfirmPasswordError] = createSignal<string | null>(null);

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

  const validateConfirmPassword = (value: string) => {
    if (!value) return 'Подтвердите пароль';
    if (value !== password()) return 'Пароли не совпадают';
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

  const handleConfirmPasswordBlur = (e: Event) => {
    const value = (e.currentTarget as HTMLInputElement).value;
    setConfirmPasswordError(validateConfirmPassword(value));
  };

  const handleSubmit = async (e: Event) => {
    e.preventDefault();
    
    // Валидация всех полей перед отправкой
    const emailErr = validateEmail(email());
    const passwordErr = validatePassword(password());
    const confirmErr = validateConfirmPassword(confirmPassword());
    
    if (emailErr || passwordErr || confirmErr) {
      setEmailError(emailErr);
      setPasswordError(passwordErr);
      setConfirmPasswordError(confirmErr);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      await authApi.register({
        email: email(),
        password: password(),
        name: name() || undefined,
        phone: phone() || undefined,
        role: 'passenger', // Хардкод роли пассажира
      } as RegisterCredentials);
      props.onSuccess?.();
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Ошибка при регистрации';
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
          label="Имя"
          type="text"
          value={name()}
          onInput={(e: Event) => setName((e.currentTarget as HTMLInputElement).value)}
          placeholder="Введите имя"
        />

        <TextField
          label="Телефон"
          type="tel"
          value={phone()}
          onInput={(e: Event) => setPhone((e.currentTarget as HTMLInputElement).value)}
          placeholder="+7 (___) ___-__-__"
        />

        <TextField
          label="Пароль"
          type="password"
          value={password()}
          onInput={(e: Event) => {
            setPassword((e.currentTarget as HTMLInputElement).value);
            setPasswordError(null);
            // Перепроверяем подтверждение пароля при изменении основного пароля
            if (confirmPassword()) {
              setConfirmPasswordError(validateConfirmPassword(confirmPassword()));
            }
          }}
          onBlur={handlePasswordBlur}
          error={passwordError() ?? undefined}
          placeholder="Придумайте пароль"
          required
        />

        <TextField
          label="Подтвердите пароль"
          type="password"
          value={confirmPassword()}
          onInput={(e: Event) => {
            setConfirmPassword((e.currentTarget as HTMLInputElement).value);
            setConfirmPasswordError(null);
          }}
          onBlur={handleConfirmPasswordBlur}
          error={confirmPasswordError() ?? undefined}
          placeholder="Повторите пароль"
          required
        />

        <Button
          type="submit"
          variant="primary"
          disabled={isLoading()}
        >
          {isLoading() ? 'Регистрация...' : 'Зарегистрироваться'}
        </Button>
      </Stack>
    </form>
  );
};

export default RegisterForm;
