// Central HTTP layer. All backend communication goes through here — never
// scatter raw fetch() calls in components.
//
// Same-origin: the Vite dev server proxies /api to the backend, so cookies
// (auth + session) flow automatically via credentials: 'include'.
// XSRF: state-changing requests (POST/PUT/DELETE) need the XSRF-TOKEN cookie
// value echoed in the X-XSRF-TOKEN header. Tokens are identity-bound, so
// refresh after login/logout/register.

export class ApiError extends Error {
  status: number;
  title: string;
  detail?: string;
  errors?: Record<string, string[]>;

  constructor(status: number, title: string, detail?: string, errors?: Record<string, string[]>) {
    super(detail || title);
    this.name = 'ApiError';
    this.status = status;
    this.title = title;
    this.detail = detail;
    this.errors = errors;
  }

  /** Flatten server validation errors to a single readable message. */
  readableMessage(): string {
    if (this.errors) {
      const parts = Object.entries(this.errors).flatMap(([field, msgs]) =>
        msgs.map((m) => (field ? `${field}: ${m}` : m)),
      );
      if (parts.length > 0) return parts.join(' ');
    }
    return this.detail || this.title;
  }
}

function parseProblem(status: number, data: unknown): ApiError {
  if (data && typeof data === 'object') {
    const p = data as Record<string, unknown>;
    const title = typeof p.title === 'string' ? p.title : `Request failed (${status})`;
    const detail = typeof p.detail === 'string' ? p.detail : undefined;
    // ValidationProblem shape: { errors: { field: [messages] } }
    const errors = p.errors as Record<string, string[]> | undefined;
    // Some endpoints return { message } instead of ProblemDetails.
    const message = typeof p.message === 'string' ? p.message : undefined;
    return new ApiError(status, title, message ?? detail, errors);
  }
  return new ApiError(status, `Request failed (${status})`);
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  let res: Response;
  try {
    res = await fetch(path, { ...init, credentials: 'include' });
  } catch {
    throw new ApiError(0, 'Network error', 'Could not reach the server. Is the backend running?');
  }

  if (res.status === 204) return undefined as T;

  let data: unknown = null;
  const text = await res.text();
  if (text) {
    try {
      data = JSON.parse(text);
    } catch {
      data = null;
    }
  }

  if (!res.ok) throw parseProblem(res.status, data);
  return data as T;
}

/** Read a cookie value (used for the readable XSRF-TOKEN cookie). */
export function getCookie(name: string): string | null {
  const match = document.cookie
    .split(';')
    .map((c) => c.trim())
    .find((c) => c.startsWith(name + '='));
  return match ? decodeURIComponent(match.slice(name.length + 1)) : null;
}

/** Fetch a fresh XSRF token pair from the backend. */
export async function refreshXsrfToken(): Promise<void> {
  await request<undefined>('/api/auth/xsrf-token');
}

/** Ensure we hold an XSRF token before a state-changing request. */
async function ensureXsrfToken(): Promise<void> {
  if (!getCookie('XSRF-TOKEN')) {
    await refreshXsrfToken();
  }
}

export async function get<T>(path: string): Promise<T> {
  return request<T>(path, { method: 'GET' });
}

/** POST/PUT/DELETE with automatic XSRF handling. */
export async function mutate<T>(path: string, method: 'POST' | 'PUT' | 'DELETE', body?: unknown): Promise<T> {
  await ensureXsrfToken();
  const token = getCookie('XSRF-TOKEN');
  return request<T>(path, {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { 'X-XSRF-TOKEN': token } : {}),
    },
    body: body === undefined ? undefined : JSON.stringify(body),
  });
}

export const post = <T>(path: string, body?: unknown): Promise<T> => mutate<T>(path, 'POST', body);
export const put = <T>(path: string, body?: unknown): Promise<T> => mutate<T>(path, 'PUT', body);
export const del = <T>(path: string): Promise<T> => mutate<T>(path, 'DELETE');
