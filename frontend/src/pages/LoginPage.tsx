import { useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../state/AuthContext';
import { useCart } from '../state/CartContext';
import { ApiError } from '../api/client';
import Field from '../components/Field';

/** Modern equivalent of Account/Login.cshtml. */
export default function LoginPage() {
  const { login } = useAuth();
  const { refresh: refreshCart } = useCart();
  const navigate = useNavigate();
  const location = useLocation();
  const from = (location.state as { from?: string } | null)?.from ?? '/';

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [rememberMe, setRememberMe] = useState(false);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setBusy(true);
    setError(null);
    setFieldErrors({});
    try {
      await login({ email, password, rememberMe });
      await refreshCart(); // backend merged the session cart on login
      navigate(from, { replace: true });
    } catch (err) {
      if (err instanceof ApiError && err.errors) {
        const flat: Record<string, string> = {};
        for (const [f, msgs] of Object.entries(err.errors)) {
          flat[f.charAt(0).toLowerCase() + f.slice(1)] = msgs.join(' ');
        }
        setFieldErrors(flat);
      }
      setError(err instanceof ApiError ? err.readableMessage() : 'Login failed.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="mx-auto max-w-md px-4 py-12 sm:px-6">
      <div className="rounded-xl border border-stone-200 bg-white p-8">
        <h1 className="text-2xl font-extrabold text-stone-900">Login</h1>
        <p className="mt-1 text-sm text-stone-500">
          New here?{' '}
          <Link to="/register" className="font-semibold text-emerald-700 hover:underline">
            Create an account
          </Link>
        </p>

        {error && (
          <div className="mt-4 rounded-lg border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700" role="alert">
            {error}
          </div>
        )}

        <form onSubmit={onSubmit} className="mt-6 space-y-4">
          <Field
            label="Email"
            type="email"
            required
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            error={fieldErrors.email}
          />
          <Field
            label="Password"
            type="password"
            required
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            error={fieldErrors.password}
          />
          <label className="flex items-center gap-2 text-sm text-stone-600">
            <input
              type="checkbox"
              checked={rememberMe}
              onChange={(e) => setRememberMe(e.target.checked)}
              className="h-4 w-4 accent-emerald-700"
            />
            Remember me
          </label>
          <button
            type="submit"
            disabled={busy}
            className="w-full rounded-lg bg-stone-900 px-6 py-3 text-sm font-bold text-white hover:bg-emerald-700 disabled:opacity-60"
          >
            {busy ? 'Logging in…' : 'Login'}
          </button>
        </form>
      </div>
    </div>
  );
}
