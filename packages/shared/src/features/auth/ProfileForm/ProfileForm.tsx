import type { Component } from 'solid-js';
import { createSignal, createEffect } from 'solid-js';
import { TextField, Button, Alert, Stack, Paper, H5 } from '@taxi/shared/ui';
import { authApi } from '@taxi/shared/api';
import type { User, UpdateProfileData } from '@taxi/shared/types';

export interface ProfileFormProps {
  initialData?: User;
  onSuccess?: (data: User) => void;
  onError?: (error: Error) => void;
}

export const ProfileForm: Component<ProfileFormProps> = (props) => {
  const [email, setEmail] = createSignal('');
  const [name, setName] = createSignal('');
  const [phone, setPhone] = createSignal('');
  const [isLoading, setIsLoading] = createSignal(false);
  const [error, setError] = createSignal<string | null>(null);
  const [success, setSuccess] = createSignal<string | null>(null);

  createEffect(() => {
    if (props.initialData) {
      setEmail(props.initialData.email);
      setName(props.initialData.name ?? '');
      setPhone(props.initialData.phone ?? '');
    }
  });

  const handleSubmit = async (e: Event) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const updateData: UpdateProfileData = {
        name: name() || undefined,
        phone: phone() || undefined,
      };
      
      const updatedUser = await authApi.updateProfile(updateData as Partial<User>);
      setSuccess('Профиль успешно обновлен');
      props.onSuccess?.(updatedUser);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Ошибка при обновлении профиля';
      setError(errorMessage);
      props.onError?.(err instanceof Error ? err : new Error(errorMessage));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Paper elevation="md" class="profile-form">
      <form onSubmit={handleSubmit}>
        <Stack direction="column" gap="1em">
          <H5>Профиль</H5>
          
          {error() && <Alert type="error">{error()}</Alert>}
          {success() && <Alert type="success">{success()}</Alert>}
          
          <TextField
            label="Email"
            type="email"
            value={email()}
            disabled
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
          
          <Button
            type="submit"
            variant="primary"
            disabled={isLoading()}
          >
            {isLoading() ? 'Сохранение...' : 'Сохранить'}
          </Button>
        </Stack>
      </form>
    </Paper>
  );
};

export default ProfileForm;
