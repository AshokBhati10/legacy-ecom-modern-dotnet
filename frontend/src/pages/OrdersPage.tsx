import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { ordersApi } from '../api/orders';
import type { OrderSummaryDto } from '../types';
import { formatDate, money } from '../utils/format';

/** Modern equivalent of Account/Orders.cshtml (replaces DataTables). */
export default function OrdersPage() {
  const [orders, setOrders] = useState<OrderSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    ordersApi
      .history()
      .then((o) => {
        if (!cancelled) setOrders(o);
      })
      .catch((err) => {
        if (!cancelled) setError(err instanceof Error ? err.message : 'Could not load orders.');
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6">
      <h1 className="text-3xl font-extrabold tracking-tight text-stone-900">Order history</h1>

      {loading ? (
        <p className="mt-8 text-stone-500">Loading…</p>
      ) : error ? (
        <div className="mt-8 rounded-lg border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700" role="alert">
          {error}
        </div>
      ) : orders.length === 0 ? (
        <div className="mt-8 rounded-xl border border-dashed border-stone-300 bg-white p-12 text-center">
          <p className="text-lg font-semibold text-stone-800">No orders yet</p>
          <p className="mt-1 text-sm text-stone-500">Your placed orders will appear here.</p>
          <Link
            to="/shop"
            className="mt-4 inline-block rounded-lg bg-stone-900 px-6 py-2.5 text-sm font-semibold text-white hover:bg-emerald-700"
          >
            Start shopping
          </Link>
        </div>
      ) : (
        <div className="mt-6 overflow-hidden rounded-xl border border-stone-200 bg-white">
          <table className="w-full text-left text-sm">
            <thead className="bg-stone-50 text-xs uppercase tracking-wide text-stone-500">
              <tr>
                <th className="px-4 py-3">Order</th>
                <th className="hidden px-4 py-3 sm:table-cell">Date</th>
                <th className="px-4 py-3">Status</th>
                <th className="hidden px-4 py-3 text-right sm:table-cell">Items</th>
                <th className="px-4 py-3 text-right">Total</th>
                <th className="px-4 py-3"><span className="sr-only">Details</span></th>
              </tr>
            </thead>
            <tbody className="divide-y divide-stone-100">
              {orders.map((o) => (
                <tr key={o.orderId} className="hover:bg-stone-50">
                  <td className="px-4 py-3 font-semibold text-stone-900">{o.orderNumber}</td>
                  <td className="hidden px-4 py-3 text-stone-600 sm:table-cell">{formatDate(o.orderDate)}</td>
                  <td className="px-4 py-3">
                    <span className="rounded-full bg-emerald-50 px-2.5 py-1 text-xs font-semibold text-emerald-800">
                      {o.status}
                    </span>
                  </td>
                  <td className="hidden px-4 py-3 text-right text-stone-600 sm:table-cell">{o.itemCount}</td>
                  <td className="px-4 py-3 text-right font-semibold">{money(o.total)}</td>
                  <td className="px-4 py-3 text-right">
                    <Link to={`/orders/${o.orderId}`} className="font-semibold text-emerald-700 hover:underline">
                      Details
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
