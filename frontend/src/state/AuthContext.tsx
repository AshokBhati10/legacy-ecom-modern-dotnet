import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import { authApi } from '../api/auth';
import type { LoginInput, RegisterInput } from '../api/auth';
import { ApiError } from '../api/client';
import type { UserDto } from '../types';

interface AuthContextValue {
  user: UserDto | null;
  loading: boolean;
  login: (input: LoginInput) => Promise<void>;
  register: (input: RegisterInput) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null);
  const [loading, setLoading] = useState(true);

  // Restore session from the auth cookie on load.
  useEffect(() => {
    let cancelled = false;
    authApi
      .me()
      .then((u) => {
        if (!cancelled) setUser(u);
      })
      .catch((err: unknown) => {
        if (!cancelled && err instanceof ApiError && err.status !== 401) {
          console.error('Session restore failed', err);
        }
        if (!cancelled) setUser(null);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  const login = useCallback(async (input: LoginInput) => {
    const u = await authApi.login(input);
    // Backend merged the session cart server-side; get a fresh identity-bound XSRF token.
    await authApi.refreshXsrf();
    setUser(u);
  }, []);

  const register = useCallback(async (input: RegisterInput) => {
    const u = await authApi.register(input);
    await authApi.refreshXsrf();
    setUser(u);
  }, []);

  const logout = useCallback(async () => {
    await authApi.logout();
    await authApi.refreshXsrf();
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({ user, loading, login, register, logout }),
    [user, loading, login, register, logout],
  );
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
