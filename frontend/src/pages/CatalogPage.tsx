import { useCallback, useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { useSearchParams } from 'react-router-dom';
import { catalogApi } from '../api/catalog';
import type { CategoryDto, ProductListDto } from '../types';
import ProductCard from '../components/ProductCard';
import Pagination from '../components/Pagination';

function CategoryNode({
  category,
  selectedId,
  onSelect,
}: {
  category: CategoryDto;
  selectedId: number | null;
  onSelect: (id: number | null) => void;
}) {
  const [children, setChildren] = useState<CategoryDto[] | null>(null);
  const [expanded, setExpanded] = useState(false);

  const toggle = async () => {
    if (!category.hasChildren) return;
    if (children === null) {
      try {
        setChildren(await catalogApi.categoryTree(category.id));
      } catch (err) {
        console.error('Category children load failed', err);
        return;
      }
    }
    setExpanded((e) => !e);
  };

  return (
    <li>
      <div className="flex items-center">
        <button
          type="button"
          onClick={() => onSelect(category.id)}
          className={`flex-1 rounded-lg px-3 py-2 text-left text-sm ${
            selectedId === category.id
              ? 'bg-emerald-50 font-semibold text-emerald-800'
              : 'text-stone-700 hover:bg-stone-100'
          }`}
        >
          {category.name}
        </button>
        {category.hasChildren && (
          <button
            type="button"
            onClick={toggle}
            aria-label={expanded ? `Collapse ${category.name}` : `Expand ${category.name}`}
            className="rounded p-1 text-stone-400 hover:bg-stone-100 hover:text-stone-700"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className={`h-4 w-4 transition-transform ${expanded ? 'rotate-90' : ''}`} aria-hidden="true">
              <path d="M9 6l6 6-6 6" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </button>
        )}
      </div>
      {expanded && children && (
        <ul className="ml-4 mt-1 space-y-0.5 border-l border-stone-200 pl-2">
          {children.map((child) => (
            <CategoryNode key={child.id} category={child} selectedId={selectedId} onSelect={onSelect} />
          ))}
        </ul>
      )}
    </li>
  );
}

/** Modern equivalent of Product/Index.cshtml with AJAX-style filtering. */
export default function CatalogPage() {
  const [params, setParams] = useSearchParams();
  const [listing, setListing] = useState<ProductListDto | null>(null);
  const [roots, setRoots] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchInput, setSearchInput] = useState(params.get('q') ?? '');

  const categoryId = params.get('categoryId') ? Number(params.get('categoryId')) : null;
  const q = params.get('q') ?? '';
  const page = params.get('page') ? Number(params.get('page')) : 1;

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [list, tree] = await Promise.all([
        catalogApi.list({
          categoryId: categoryId ?? undefined,
          q: q || undefined,
          page,
          pageSize: 12,
        }),
        catalogApi.categoryTree(),
      ]);
      setListing(list);
      setRoots(tree);
    } catch (err) {
      console.error('Catalog load failed', err);
    } finally {
      setLoading(false);
    }
  }, [categoryId, q, page]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    setSearchInput(q);
  }, [q]);

  const updateParams = (updates: Record<string, string | null>) => {
    const next = new URLSearchParams(params);
    for (const [k, v] of Object.entries(updates)) {
      if (v === null || v === '') next.delete(k);
      else next.set(k, v);
    }
    next.delete('page'); // reset pagination on filter change
    setParams(next);
  };

  const onSearch = (e: FormEvent) => {
    e.preventDefault();
    updateParams({ q: searchInput.trim() || null });
  };

  return (
    <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6">
      <h1 className="text-3xl font-extrabold tracking-tight text-stone-900">Shop</h1>

      <div className="mt-6 flex flex-col gap-8 lg:flex-row">
        {/* Sidebar — legacy _Sidebar.cshtml category tree */}
        <aside className="w-full shrink-0 lg:w-64">
          <div className="rounded-xl border border-stone-200 bg-white p-4">
            <h2 className="mb-3 text-sm font-bold uppercase tracking-wide text-stone-500">Categories</h2>
            <ul className="space-y-0.5">
              <li>
                <button
                  type="button"
                  onClick={() => updateParams({ categoryId: null })}
                  className={`w-full rounded-lg px-3 py-2 text-left text-sm ${
                    categoryId === null
                      ? 'bg-emerald-50 font-semibold text-emerald-800'
                      : 'text-stone-700 hover:bg-stone-100'
                  }`}
                >
                  All products
                </button>
              </li>
              {roots.map((c) => (
                <CategoryNode
                  key={c.id}
                  category={c}
                  selectedId={categoryId}
                  onSelect={(id) => updateParams({ categoryId: id === null ? null : String(id) })}
                />
              ))}
            </ul>
          </div>
        </aside>

        {/* Product grid — legacy _ProductList.cshtml partial */}
        <div className="min-w-0 flex-1">
          <form onSubmit={onSearch} className="mb-5 flex gap-2" role="search">
            <input
              type="search"
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              placeholder="Search products…"
              aria-label="Search products"
              className="w-full rounded-lg border border-stone-300 bg-white px-3 py-2 text-sm focus:border-emerald-600 focus:outline-none"
            />
            <button
              type="submit"
              className="rounded-lg bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700"
            >
              Search
            </button>
            {q && (
              <button
                type="button"
                onClick={() => updateParams({ q: null })}
                className="rounded-lg border border-stone-300 px-4 py-2 text-sm font-medium text-stone-600 hover:bg-stone-100"
              >
                Clear
              </button>
            )}
          </form>

          {loading ? (
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
              {Array.from({ length: 6 }).map((_, i) => (
                <div key={i} className="aspect-square animate-pulse rounded-xl bg-stone-200" />
              ))}
            </div>
          ) : !listing || listing.products.length === 0 ? (
            <div className="rounded-xl border border-dashed border-stone-300 bg-white p-12 text-center">
              <p className="font-semibold text-stone-700">No products found</p>
              <p className="mt-1 text-sm text-stone-500">
                Try a different search term or category.
              </p>
            </div>
          ) : (
            <>
              <p className="mb-4 text-sm text-stone-500" aria-live="polite">
                {listing.totalCount} product{listing.totalCount === 1 ? '' : 's'}
                {q && <> for “{q}”</>}
              </p>
              <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
                {listing.products.map((p) => (
                  <ProductCard key={p.id} product={p} />
                ))}
              </div>
              <Pagination
                page={listing.page}
                totalPages={listing.totalPages}
                onPage={(p) => {
                  const next = new URLSearchParams(params);
                  next.set('page', String(p));
                  setParams(next);
                  window.scrollTo({ top: 0, behavior: 'smooth' });
                }}
              />
            </>
          )}
        </div>
      </div>
    </div>
  );
}
