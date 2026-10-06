import { useState } from 'react';
import type { FormEvent } from 'react';
import { Link, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../state/AuthContext';
import { useCart } from '../state/CartContext';

function linkClass({ isActive }: { isActive: boolean }): string {
  return `px-3 py-2 text-sm font-medium rounded-lg transition-colors ${
    isActive ? 'text-emerald-700 bg-emerald-50' : 'text-stone-600 hover:text-stone-900 hover:bg-stone-100'
  }`;
}

/** Modern equivalent of the legacy _Header.cshtml partial. */
export default function Header() {
  const { user, logout } = useAuth();
  const { mini, setDrawerOpen } = useCart();
  const [search, setSearch] = useState('');
  const [menuOpen, setMenuOpen] = useState(false);
  const navigate = useNavigate();

  const onSearch = (e: FormEvent) => {
    e.preventDefault();
    navigate(`/shop${search.trim() ? `?q=${encodeURIComponent(search.trim())}` : ''}`);
    setMenuOpen(false);
  };

  const onLogout = async () => {
    await logout();
    navigate('/');
  };

  return (
    <header className="sticky top-0 z-40 border-b border-stone-200 bg-white/95 backdrop-blur">
      <div className="mx-auto flex max-w-7xl items-center gap-4 px-4 py-3 sm:px-6">
        <Link to="/" className="flex items-center gap-2 text-xl font-extrabold tracking-tight text-stone-900">
          <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-emerald-700 text-white">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-5 w-5" aria-hidden="true">
              <path d="M6 6h15l-1.5 9h-12z" strokeWidth="1.8" strokeLinejoin="round" />
              <path d="M6 6L5 3H2" strokeWidth="1.8" strokeLinecap="round" />
              <circle cx="9" cy="20" r="1.6" fill="currentColor" stroke="none" />
              <circle cx="17" cy="20" r="1.6" fill="currentColor" stroke="none" />
            </svg>
          </span>
          <span className="hidden sm:inline">Northwind&nbsp;Market</span>
          <span className="sm:hidden">NM</span>
        </Link>

        <nav className="hidden items-center gap-1 md:flex" aria-label="Primary">
          <NavLink to="/" className={linkClass} end>Home</NavLink>
          <NavLink to="/shop" className={linkClass}>Shop</NavLink>
          {user && <NavLink to="/orders" className={linkClass}>Orders</NavLink>}
        </nav>

        <form onSubmit={onSearch} className="ml-auto hidden min-w-0 flex-1 max-w-xs items-center sm:flex" role="search">
          <input
            type="search"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search products…"
            aria-label="Search products"
            className="w-full rounded-lg border border-stone-300 bg-stone-50 px-3 py-2 text-sm focus:border-emerald-600 focus:outline-none"
          />
        </form>

        <div className="ml-auto flex items-center gap-1 sm:ml-0">
          {user ? (
            <div className="hidden items-center gap-1 md:flex">
              <span className="px-2 text-sm text-stone-500">
                Hi, {user.firstName || user.email}
              </span>
              <button
                type="button"
                onClick={onLogout}
                className="rounded-lg px-3 py-2 text-sm font-medium text-stone-600 hover:bg-stone-100 hover:text-stone-900"
              >
                Logout
              </button>
            </div>
          ) : (
            <div className="hidden items-center gap-1 md:flex">
              <NavLink to="/login" className={linkClass}>Login</NavLink>
              <NavLink
                to="/register"
                className="rounded-lg bg-stone-900 px-3 py-2 text-sm font-medium text-white hover:bg-emerald-700"
              >
                Register
              </NavLink>
            </div>
          )}

          <button
            type="button"
            onClick={() => setDrawerOpen(true)}
            className="relative rounded-lg p-2 text-stone-700 hover:bg-stone-100"
            aria-label={`Open cart, ${mini?.itemCount ?? 0} items`}
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-6 w-6" aria-hidden="true">
              <path d="M6 6h15l-1.5 9h-12z" strokeWidth="1.8" strokeLinejoin="round" />
              <path d="M6 6L5 3H2" strokeWidth="1.8" strokeLinecap="round" />
              <circle cx="9" cy="20" r="1.6" fill="currentColor" stroke="none" />
              <circle cx="17" cy="20" r="1.6" fill="currentColor" stroke="none" />
            </svg>
            {(mini?.itemCount ?? 0) > 0 && (
              <span className="absolute -right-0.5 -top-0.5 flex h-5 min-w-5 items-center justify-center rounded-full bg-emerald-700 px-1 text-[11px] font-bold text-white">
                {mini!.itemCount}
              </span>
            )}
          </button>

          <button
            type="button"
            onClick={() => setMenuOpen((o) => !o)}
            className="rounded-lg p-2 text-stone-700 hover:bg-stone-100 md:hidden"
            aria-label="Toggle menu"
            aria-expanded={menuOpen}
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-6 w-6" aria-hidden="true">
              <path d="M4 7h16M4 12h16M4 17h16" strokeWidth="1.8" strokeLinecap="round" />
            </svg>
          </button>
        </div>
      </div>

      {menuOpen && (
        <div className="border-t border-stone-200 bg-white px-4 py-3 md:hidden">
          <form onSubmit={onSearch} className="mb-3 flex" role="search">
            <input
              type="search"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search products…"
              aria-label="Search products"
              className="w-full rounded-lg border border-stone-300 bg-stone-50 px-3 py-2 text-sm focus:border-emerald-600 focus:outline-none"
            />
          </form>
          <nav className="flex flex-col gap-1" aria-label="Mobile">
            <NavLink to="/" className={linkClass} end onClick={() => setMenuOpen(false)}>Home</NavLink>
            <NavLink to="/shop" className={linkClass} onClick={() => setMenuOpen(false)}>Shop</NavLink>
            {user ? (
              <>
                <NavLink to="/orders" className={linkClass} onClick={() => setMenuOpen(false)}>Orders</NavLink>
                <button
                  type="button"
                  onClick={() => { setMenuOpen(false); onLogout(); }}
                  className="rounded-lg px-3 py-2 text-left text-sm font-medium text-stone-600 hover:bg-stone-100"
                >
                  Logout ({user.email})
                </button>
              </>
            ) : (
              <>
                <NavLink to="/login" className={linkClass} onClick={() => setMenuOpen(false)}>Login</NavLink>
                <NavLink to="/register" className={linkClass} onClick={() => setMenuOpen(false)}>Register</NavLink>
              </>
            )}
          </nav>
        </div>
      )}
    </header>
  );
}
