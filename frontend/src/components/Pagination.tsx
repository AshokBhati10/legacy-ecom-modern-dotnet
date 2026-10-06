interface Props {
  page: number;
  totalPages: number;
  onPage: (page: number) => void;
}

export default function Pagination({ page, totalPages, onPage }: Props) {
  if (totalPages <= 1) return null;

  const pages: number[] = [];
  for (let p = Math.max(1, page - 2); p <= Math.min(totalPages, page + 2); p++) {
    pages.push(p);
  }

  const btn = (active: boolean) =>
    `rounded-lg px-3 py-1.5 text-sm font-medium ${
      active ? 'bg-stone-900 text-white' : 'border border-stone-300 bg-white text-stone-700 hover:bg-stone-100'
    }`;

  return (
    <nav className="mt-8 flex items-center justify-center gap-1.5" aria-label="Pagination">
      <button
        type="button"
        disabled={page <= 1}
        onClick={() => onPage(page - 1)}
        className={`${btn(false)} disabled:opacity-40`}
      >
        ‹ Prev
      </button>
      {pages[0] > 1 && <span className="px-1 text-stone-400">…</span>}
      {pages.map((p) => (
        <button key={p} type="button" onClick={() => onPage(p)} className={btn(p === page)} aria-current={p === page ? 'page' : undefined}>
          {p}
        </button>
      ))}
      {pages[pages.length - 1] < totalPages && <span className="px-1 text-stone-400">…</span>}
      <button
        type="button"
        disabled={page >= totalPages}
        onClick={() => onPage(page + 1)}
        className={`${btn(false)} disabled:opacity-40`}
      >
        Next ›
      </button>
    </nav>
  );
}
