import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '../state/CartContext';
import { money } from '../utils/format';
import ProductImage from '../components/ProductImage';

/** Modern equivalent of Cart/Index.cshtml (full cart). */
export default function CartPage() {
  const { cart, loading, updateItem, removeItem, clear, lastError, clearError } = useCart();
  const [confirmingClear, setConfirmingClear] = useState(false);

  const items = cart?.items ?? [];

  return (
    <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6">
      <h1 className="text-3xl font-extrabold tracking-tight text-stone-900">Shopping cart</h1>

      {lastError && (
        <div className="mt-4 flex items-center justify-between rounded-lg border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700" role="alert">
          <span>{lastError}</span>
          <button type="button" onClick={clearError} className="font-semibold hover:underline">Dismiss</button>
        </div>
      )}

      {loading ? (
        <p className="mt-8 text-stone-500">Loading…</p>
      ) : items.length === 0 ? (
        <div className="mt-8 rounded-xl border border-dashed border-stone-300 bg-white p-12 text-center">
          <p className="text-lg font-semibold text-stone-800">Your cart is empty</p>
          <p className="mt-1 text-sm text-stone-500">Add some products to get started.</p>
          <Link
            to="/shop"
            className="mt-4 inline-block rounded-lg bg-stone-900 px-6 py-2.5 text-sm font-semibold text-white hover:bg-emerald-700"
          >
            Continue shopping
          </Link>
        </div>
      ) : (
        <div className="mt-6 grid gap-8 lg:grid-cols-3">
          <ul className="divide-y divide-stone-200 rounded-xl border border-stone-200 bg-white lg:col-span-2">
            {items.map((line) => (
              <li key={`${line.productId}-${line.variantId ?? 0}`} className="flex gap-4 p-4 sm:p-5">
                <ProductImage
                  src={line.thumbnailUrl}
                  alt={line.productName}
                  className="h-20 w-20 shrink-0 rounded-lg sm:h-24 sm:w-24"
                />
                <div className="min-w-0 flex-1">
                  <p className="font-semibold text-stone-900">{line.productName}</p>
                  {line.variantName && <p className="text-sm text-stone-500">{line.variantName}</p>}
                  <p className="mt-1 text-sm text-stone-500">{money(line.unitPrice)} each</p>
                  <div className="mt-2 flex items-center gap-2">
                    <div className="flex items-center rounded-lg border border-stone-300">
                      <button
                        type="button"
                        onClick={() => updateItem(line.productId, line.variantId, line.quantity - 1)}
                        disabled={line.quantity <= 1}
                        className="px-2.5 py-1 text-stone-600 hover:bg-stone-100 disabled:opacity-40"
                        aria-label="Decrease quantity"
                      >
                        −
                      </button>
                      <span className="w-8 text-center text-sm font-semibold" aria-live="polite">{line.quantity}</span>
                      <button
                        type="button"
                        onClick={() => updateItem(line.productId, line.variantId, line.quantity + 1)}
                        className="px-2.5 py-1 text-stone-600 hover:bg-stone-100"
                        aria-label="Increase quantity"
                      >
                        +
                      </button>
                    </div>
                    <button
                      type="button"
                      onClick={() => removeItem(line.productId, line.variantId)}
                      className="text-sm font-medium text-rose-600 hover:underline"
                    >
                      Remove
                    </button>
                  </div>
                </div>
                <p className="shrink-0 font-bold text-stone-900">{money(line.lineTotal)}</p>
              </li>
            ))}
          </ul>

          <div className="h-fit rounded-xl border border-stone-200 bg-white p-5">
            <h2 className="text-lg font-bold text-stone-900">Summary</h2>
            <dl className="mt-4 space-y-2 text-sm">
              <div className="flex justify-between">
                <dt className="text-stone-600">Items</dt>
                <dd className="font-medium">{cart!.itemCount}</dd>
              </div>
              <div className="flex justify-between border-t border-stone-100 pt-2">
                <dt className="text-stone-600">Subtotal</dt>
                <dd className="text-lg font-bold text-stone-900">{money(cart!.subTotal)}</dd>
              </div>
            </dl>
            <p className="mt-2 text-xs text-stone-500">
              Shipping and tax are calculated at checkout.
            </p>
            <Link
              to="/checkout"
              className="mt-4 block rounded-lg bg-emerald-700 px-4 py-3 text-center text-sm font-bold text-white hover:bg-emerald-800"
            >
              Proceed to checkout
            </Link>
            <Link
              to="/shop"
              className="mt-2 block rounded-lg border border-stone-300 px-4 py-2.5 text-center text-sm font-semibold text-stone-700 hover:bg-stone-100"
            >
              Continue shopping
            </Link>
            {confirmingClear ? (
              <div className="mt-4 rounded-lg bg-stone-50 p-3 text-center">
                <p className="text-sm text-stone-600">Remove all items?</p>
                <div className="mt-2 flex justify-center gap-2">
                  <button
                    type="button"
                    onClick={() => { clear(); setConfirmingClear(false); }}
                    className="rounded-lg bg-rose-600 px-3 py-1.5 text-sm font-semibold text-white hover:bg-rose-700"
                  >
                    Yes, clear
                  </button>
                  <button
                    type="button"
                    onClick={() => setConfirmingClear(false)}
                    className="rounded-lg border border-stone-300 px-3 py-1.5 text-sm font-medium hover:bg-stone-100"
                  >
                    Cancel
                  </button>
                </div>
              </div>
            ) : (
              <button
                type="button"
                onClick={() => setConfirmingClear(true)}
                className="mt-3 w-full text-center text-sm font-medium text-stone-500 hover:text-rose-600"
              >
                Clear cart
              </button>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
