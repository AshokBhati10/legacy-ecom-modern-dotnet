import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { catalogApi } from '../api/catalog';
import type { CategoryDto, ProductCardDto } from '../types';
import ProductCard from '../components/ProductCard';

export default function Home() {
  const [featured, setFeatured] = useState<ProductCardDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    Promise.all([catalogApi.featured(8), catalogApi.categories()])
      .then(([f, c]) => {
        if (!cancelled) {
          setFeatured(f);
          setCategories(c);
        }
      })
      .catch((err) => console.error('Home load failed', err))
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <div>
      {/* Hero */}
      <section className="bg-gradient-to-br from-emerald-800 via-emerald-700 to-teal-700 text-white">
        <div className="mx-auto max-w-7xl px-4 py-16 sm:px-6 sm:py-24">
          <p className="text-sm font-semibold uppercase tracking-widest text-emerald-200">
            The modernized classic store
          </p>
          <h1 className="mt-3 max-w-2xl text-4xl font-extrabold tracking-tight sm:text-5xl">
            Everything you need, delivered to your door.
          </h1>
          <p className="mt-4 max-w-xl text-lg text-emerald-100">
            Browse electronics, home essentials, sports gear and more — with fast
            checkout and order tracking.
          </p>
          <div className="mt-8 flex flex-wrap gap-3">
            <Link
              to="/shop"
              className="rounded-lg bg-white px-6 py-3 text-sm font-bold text-emerald-800 hover:bg-emerald-50"
            >
              Shop all products
            </Link>
            <Link
              to="/shop"
              className="rounded-lg border border-emerald-200 px-6 py-3 text-sm font-bold text-white hover:bg-white/10"
            >
              Browse categories
            </Link>
          </div>
        </div>
      </section>

      {/* Categories */}
      <section className="mx-auto max-w-7xl px-4 py-12 sm:px-6">
        <div className="mb-6 flex items-center justify-between">
          <h2 className="text-2xl font-bold text-stone-900">Popular categories</h2>
          <Link to="/shop" className="text-sm font-semibold text-emerald-700 hover:underline">
            View all →
          </Link>
        </div>
        {loading ? (
          <p className="text-stone-500">Loading…</p>
        ) : (
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-6">
            {categories.map((c) => (
              <Link
                key={c.id}
                to={`/shop?categoryId=${c.id}`}
                className="rounded-xl border border-stone-200 bg-white p-5 text-center transition-shadow hover:shadow-md"
              >
                <p className="font-semibold text-stone-900">{c.name}</p>
                <p className="mt-1 text-xs text-stone-500">Shop now →</p>
              </Link>
            ))}
          </div>
        )}
      </section>

      {/* Featured products */}
      <section className="mx-auto max-w-7xl px-4 pb-16 sm:px-6">
        <div className="mb-6 flex items-center justify-between">
          <h2 className="text-2xl font-bold text-stone-900">Featured products</h2>
          <Link to="/shop" className="text-sm font-semibold text-emerald-700 hover:underline">
            View all →
          </Link>
        </div>
        {loading ? (
          <p className="text-stone-500">Loading…</p>
        ) : featured.length === 0 ? (
          <p className="text-stone-500">No featured products right now.</p>
        ) : (
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
            {featured.map((p) => (
              <ProductCard key={p.id} product={p} />
            ))}
          </div>
        )}
      </section>

      {/* Value props */}
      <section className="border-t border-stone-200 bg-white">
        <div className="mx-auto grid max-w-7xl gap-6 px-4 py-10 sm:grid-cols-3 sm:px-6">
          {[
            { title: 'Secure checkout', text: 'Address, shipping and payment steps with server-side validation.' },
            { title: 'Real-time cart', text: 'Session cart that follows you — merge it when you sign in.' },
            { title: 'Order tracking', text: 'Full order history with line-item detail in your account.' },
          ].map((v) => (
            <div key={v.title} className="rounded-xl bg-stone-50 p-6">
              <p className="font-bold text-stone-900">{v.title}</p>
              <p className="mt-1 text-sm text-stone-600">{v.text}</p>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
}
