import { get, post, refreshXsrfToken } from './client';
import type { UserDto } from '../types';

export interface RegisterInput {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface LoginInput {
  email: string;
  password: string;
  rememberMe: boolean;
}

export const authApi = {
  me: () => get<UserDto>('/api/auth/me'),
  register: (input: RegisterInput) => post<UserDto>('/api/auth/register', input),
  login: (input: LoginInput) => post<UserDto>('/api/auth/login', input),
  logout: () => post<undefined>('/api/auth/logout'),
  /** XSRF tokens are identity-bound: refresh after any auth state change. */
  refreshXsrf: () => refreshXsrfToken(),
};
