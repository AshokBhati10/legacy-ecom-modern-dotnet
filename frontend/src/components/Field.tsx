import type { InputHTMLAttributes } from 'react';

interface Props extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string | null;
  hint?: string;
}

/** Labeled form field with inline validation message. */
export default function Field({ label, error, hint, id, ...rest }: Props) {
  const fieldId = id ?? `field-${label.replace(/\s+/g, '-').toLowerCase()}`;
  return (
    <div>
      <label htmlFor={fieldId} className="mb-1 block text-sm font-medium text-stone-700">
        {label}
      </label>
      <input
        id={fieldId}
        {...rest}
        aria-invalid={!!error}
        aria-describedby={error ? `${fieldId}-error` : undefined}
        className={`w-full rounded-lg border bg-white px-3 py-2 text-sm focus:outline-none ${
          error
            ? 'border-rose-500 focus:border-rose-600'
            : 'border-stone-300 focus:border-emerald-600'
        }`}
      />
      {error ? (
        <p id={`${fieldId}-error`} className="mt-1 text-xs text-rose-600">
          {error}
        </p>
      ) : hint ? (
        <p className="mt-1 text-xs text-stone-500">{hint}</p>
      ) : null}
    </div>
  );
}
