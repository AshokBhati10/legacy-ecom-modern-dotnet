import { useState } from 'react';

interface Props {
  src: string | null | undefined;
  alt: string;
  className?: string;
}

/**
 * Product image with graceful fallback. The backend seed data references
 * /Content/images/... paths that the API does not serve, so missing images
 * render a neutral placeholder instead of a broken icon.
 */
export default function ProductImage({ src, alt, className = '' }: Props) {
  const [failed, setFailed] = useState(false);

  if (!src || failed) {
    return (
      <div
        className={`flex items-center justify-center bg-stone-200 text-stone-400 ${className}`}
        role="img"
        aria-label={alt}
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-1/3 w-1/3" aria-hidden="true">
          <rect x="3" y="3" width="18" height="18" rx="2" strokeWidth="1.5" />
          <circle cx="8.5" cy="8.5" r="1.5" strokeWidth="1.5" />
          <path d="M21 15l-5-5L5 21" strokeWidth="1.5" />
        </svg>
      </div>
    );
  }

  return (
    <img
      src={src}
      alt={alt}
      loading="lazy"
      onError={() => setFailed(true)}
      className={`object-cover ${className}`}
    />
  );
}
