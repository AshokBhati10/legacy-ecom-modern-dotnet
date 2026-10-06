import { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '../state/CartContext';
import { money } from '../utils/format';
import ProductImage from './ProductImage';

/** Modern equivalent of the legacy _MiniCart.cshtml — slide-over drawer. */
export default function MiniCart() {
  const { cart, drawerOpen, setDrawerOpen, updateItem, removeItem, loading } = useCart();

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') setDrawerOpen(false);
    };
    if (drawerOpen) window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [drawerOpen, setDrawerOpen]);

  if (!drawerOpen) return null;

  const items = cart?.items ?? [];

  return (
    <div className="fixed inset-0 z-50" role="dialog" aria-modal="true" aria-label="Shopping cart">
      <div
        className="absolute inset-0 bg-black/40"
        onClick={() => setDrawerOpen(false)}
        aria-hidden="true"
      />
      <aside className="absolute right-0 top-0 flex h-full w-full max-w-md flex-col bg-white shadow-xl">
        <div className="flex items-center justify-between border-b border-stone-200 px-5 py-4">
          <h2 className="text-lg font-bold text-stone-900">
            Cart {cart && cart.itemCount > 0 && <span className="text-stone-500">({cart.itemCount})</span>}
          </h2>
          <button
            type="button"
            onClick={() => setDrawerOpen(false)}
            className="rounded-lg p-2 text-stone-500 hover:bg-stone-100"
            aria-label="Close cart"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-5 w-5" aria-hidden="true">
              <path d="M6 6l12 12M18 6L6 18" strokeWidth="1.8" strokeLinecap="round" />
            </svg>
          </button>
        </div>

        <div className="flex-1 overflow-y-auto px-5 py-4">
          {loading ? (
            <p className="text-sm text-stone-500">Loading…</p>
          ) : items.length === 0 ? (
            <div className="flex h-full flex-col items-center justify-center gap-3 text-center">
              <p className="text-stone-500">Your cart is empty.</p>
              <Link
                to="/shop"
                onClick={() => setDrawerOpen(false)}
                className="rounded-lg bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700"
              >
                Continue shopping
              </Link>
            </div>
          ) : (
            <ul className="divide-y divide-stone-100">
              {items.map((line) => (
                <li key={`${line.productId}-${line.variantId ?? 0}`} className="flex gap-3 py-3">
                  <ProductImage
                    src={line.thumbnailUrl}
                    alt={line.productName}
                    className="h-16 w-16 shrink-0 rounded-lg"
                  />
                  <div className="min-w-0 flex-1">
                    <p className="truncate text-sm font-medium text-stone-900">{line.productName}</p>
                    {line.variantName && <p className="text-xs text-stone-500">{line.variantName}</p>}
                    <p className="mt-1 text-sm font-semibold">{money(line.lineTotal)}</p>
                    <div className="mt-1 flex items-center gap-2">
                      <button
                        type="button"
                        onClick={() => updateItem(line.productId, line.variantId, line.quantity - 1)}
                        disabled={line.quantity <= 1}
                        className="rounded border border-stone-300 px-2 py-0.5 text-sm disabled:opacity-40"
                        aria-label="Decrease quantity"
                      >
                        −
                      </button>
                      <span className="text-sm">{line.quantity}</span>
                      <button
                        type="button"
                        onClick={() => updateItem(line.productId, line.variantId, line.quantity + 1)}
                        className="rounded border border-stone-300 px-2 py-0.5 text-sm"
                        aria-label="Increase quantity"
                      >
                        +
                      </button>
                      <button
                        type="button"
                        onClick={() => removeItem(line.productId, line.variantId)}
                        className="ml-auto text-xs font-medium text-rose-600 hover:underline"
                      >
                        Remove
                      </button>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>

        {items.length > 0 && cart && (
          <div className="border-t border-stone-200 px-5 py-4">
            <div className="mb-3 flex items-center justify-between text-sm">
              <span className="text-stone-600">Subtotal</span>
              <span className="text-lg font-bold text-stone-900">{money(cart.subTotal)}</span>
            </div>
            <div className="flex gap-2">
              <Link
                to="/cart"
                onClick={() => setDrawerOpen(false)}
                className="flex-1 rounded-lg border border-stone-300 px-4 py-2.5 text-center text-sm font-semibold text-stone-800 hover:bg-stone-100"
              >
                View cart
              </Link>
              <Link
                to="/checkout"
                onClick={() => setDrawerOpen(false)}
                className="flex-1 rounded-lg bg-emerald-700 px-4 py-2.5 text-center text-sm font-semibold text-white hover:bg-emerald-800"
              >
                Checkout
              </Link>
            </div>
          </div>
        )}
      </aside>
    </div>
  );
}
