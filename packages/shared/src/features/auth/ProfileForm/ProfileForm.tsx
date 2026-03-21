import type { Component } from 'solid-js';
import { createSignal, createEffect } from 'solid-js';
import { TextField, Button, Alert, Stack, Paper, H5 } from '@taxi/shared/ui';
import { authApi } from '@taxi/shared/api';
import type { User, UserWithAuth, UpdatePassengerProfileRequest } from '@taxi/shared/types';

export interface ProfileFormProps {
  initialData?: UserWithAuth;
  onSuccess?: (data: User) => void;
  onError?: (error: Error) => void;
}

export const ProfileForm: Component<ProfileFormProps> = (props) => {
  const [name, setName] = createSignal('');
  const [isLoading, setIsLoading] = createSignal(false);
  const [error, setError] = createSignal<string | null>(null);
  const [success, setSuccess] = createSignal<string | null>(null);

  createEffect(() => {
    if (props.initialData) {
      setName(props.initialData.name ?? '');
    }
  });

  const handleSubmit = async (e: Event) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const updateData: UpdatePassengerProfileRequest = {
        name: name() || null,
      };

      const updatedUser = await authApi.updateProfile(updateData);
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
            label="Имя"
            type="text"
            value={name()}
            onInput={(e: Event) => setName((e.currentTarget as HTMLInputElement).value)}
            placeholder="Введите имя"
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
