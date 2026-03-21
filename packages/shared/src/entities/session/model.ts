import { createSignal, createMemo } from 'solid-js';

const [accessToken, setAccessToken] = createSignal<string | null>(null);
const [refreshToken, setRefreshToken] = createSignal<string | null>(null);
const [isLoading, setIsLoading] = createSignal(false);

export const createSessionStore = () => {
  const setTokens = (access: string, refresh?: string) => {
    setAccessToken(access);
    if (refresh) {
      setRefreshToken(refresh);
    }
  };

  const clearTokens = () => {
    setAccessToken(null);
    setRefreshToken(null);
  };

  const setLoading = (loading: boolean) => {
    setIsLoading(loading);
  };

  const isAuthenticated = createMemo(() => accessToken() !== null);

  return {
    accessToken,
    refreshToken,
    isLoading,
    setTokens,
    clearTokens,
    setLoading,
    isAuthenticated,
  };
};

export type SessionStore = ReturnType<typeof createSessionStore>;
