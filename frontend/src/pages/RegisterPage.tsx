import { useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../state/AuthContext';
import { useCart } from '../state/CartContext';
import { ApiError } from '../api/client';
import Field from '../components/Field';

/** Modern equivalent of Account/Register.cshtml. */
export default function RegisterPage() {
  const { register } = useAuth();
  const { refresh: refreshCart } = useCart();
  const navigate = useNavigate();

  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
  });
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const set = (k: keyof typeof form, v: string) => setForm((f) => ({ ...f, [k]: v }));

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setBusy(true);
    setError(null);
    setFieldErrors({});
    try {
      await register(form);
      await refreshCart();
      navigate('/', { replace: true });
    } catch (err) {
      if (err instanceof ApiError && err.errors) {
        const flat: Record<string, string> = {};
        for (const [f, msgs] of Object.entries(err.errors)) {
          flat[f.charAt(0).toLowerCase() + f.slice(1)] = msgs.join(' ');
        }
        setFieldErrors(flat);
      }
      setError(err instanceof ApiError ? err.readableMessage() : 'Registration failed.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="mx-auto max-w-md px-4 py-12 sm:px-6">
      <div className="rounded-xl border border-stone-200 bg-white p-8">
        <h1 className="text-2xl font-extrabold text-stone-900">Create an account</h1>
        <p className="mt-1 text-sm text-stone-500">
          Already have one?{' '}
          <Link to="/login" className="font-semibold text-emerald-700 hover:underline">
            Login
          </Link>
        </p>

        {error && (
          <div className="mt-4 rounded-lg border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700" role="alert">
            {error}
          </div>
        )}

        <form onSubmit={onSubmit} className="mt-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Field label="First name" required autoComplete="given-name" value={form.firstName} onChange={(e) => set('firstName', e.target.value)} error={fieldErrors.firstName} />
            <Field label="Last name" required autoComplete="family-name" value={form.lastName} onChange={(e) => set('lastName', e.target.value)} error={fieldErrors.lastName} />
          </div>
          <Field label="Email" type="email" required autoComplete="email" value={form.email} onChange={(e) => set('email', e.target.value)} error={fieldErrors.email} />
          <Field
            label="Password"
            type="password"
            required
            autoComplete="new-password"
            value={form.password}
            onChange={(e) => set('password', e.target.value)}
            error={fieldErrors.password}
            hint="At least 6 characters."
          />
          <Field
            label="Confirm password"
            type="password"
            required
            autoComplete="new-password"
            value={form.confirmPassword}
            onChange={(e) => set('confirmPassword', e.target.value)}
            error={fieldErrors.confirmPassword}
          />
          <button
            type="submit"
            disabled={busy}
            className="w-full rounded-lg bg-stone-900 px-6 py-3 text-sm font-bold text-white hover:bg-emerald-700 disabled:opacity-60"
          >
            {busy ? 'Creating account…' : 'Register'}
          </button>
        </form>
      </div>
    </div>
  );
}
