import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ordersApi } from '../api/orders';
import type { OrderDto } from '../types';
import { formatDate, money } from '../utils/format';

/** Order detail view. */
export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    let cancelled = false;
    ordersApi
      .detail(Number(id))
      .then((o) => {
        if (!cancelled) setOrder(o);
      })
      .catch(() => {
        if (!cancelled) setNotFound(true);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [id]);

  if (loading) {
    return <div className="mx-auto max-w-5xl px-4 py-16 text-stone-500 sm:px-6">Loading…</div>;
  }
  if (notFound || !order) {
    return (
      <div className="mx-auto max-w-5xl px-4 py-16 text-center sm:px-6">
        <p className="text-lg font-semibold text-stone-800">Order not found</p>
        <Link to="/orders" className="mt-3 inline-block text-sm font-semibold text-emerald-700 hover:underline">
          Back to orders →
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6">
      <nav className="mb-6 text-sm text-stone-500" aria-label="Breadcrumb">
        <Link to="/orders" className="hover:text-emerald-700">Orders</Link>
        <span className="mx-2">/</span>
        <span className="text-stone-800">{order.orderNumber}</span>
      </nav>

      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-3xl font-extrabold tracking-tight text-stone-900">{order.orderNumber}</h1>
        <span className="rounded-full bg-emerald-50 px-3 py-1 text-sm font-semibold text-emerald-800">
          {order.status}
        </span>
      </div>
      <p className="mt-1 text-sm text-stone-500">Placed on {formatDate(order.orderDate)}</p>

      <div className="mt-6 grid gap-6 lg:grid-cols-3">
        <div className="rounded-xl border border-stone-200 bg-white p-5 lg:col-span-2">
          <h2 className="mb-4 text-lg font-bold text-stone-900">Items</h2>
          <ul className="divide-y divide-stone-100">
            {order.lines.map((l) => (
              <li key={l.id} className="flex items-center justify-between gap-4 py-3">
                <div className="min-w-0">
                  <p className="truncate font-medium text-stone-900">{l.productName}</p>
                  {l.variantName && <p className="text-sm text-stone-500">{l.variantName}</p>}
                  <p className="text-sm text-stone-500">{money(l.unitPrice)} × {l.quantity}</p>
                </div>
                <p className="shrink-0 font-semibold">{money(l.lineTotal)}</p>
              </li>
            ))}
          </ul>
          <dl className="mt-4 space-y-1.5 border-t border-stone-100 pt-4 text-sm">
            <div className="flex justify-between"><dt className="text-stone-600">Subtotal</dt><dd>{money(order.subTotal)}</dd></div>
            <div className="flex justify-between"><dt className="text-stone-600">Shipping ({order.shippingMethod})</dt><dd>{money(order.shippingCost)}</dd></div>
            <div className="flex justify-between"><dt className="text-stone-600">Tax</dt><dd>{money(order.taxAmount)}</dd></div>
            <div className="flex justify-between border-t border-stone-100 pt-2 text-base font-bold"><dt>Total</dt><dd>{money(order.total)}</dd></div>
          </dl>
        </div>

        <div className="space-y-6">
          <div className="rounded-xl border border-stone-200 bg-white p-5">
            <h2 className="mb-3 text-lg font-bold text-stone-900">Shipping address</h2>
            <address className="text-sm not-italic leading-relaxed text-stone-600">
              {order.shipFirstName} {order.shipLastName}<br />
              {order.shipStreet}<br />
              {order.shipCity}, {order.shipState} {order.shipPostalCode}<br />
              {order.shipCountry}<br />
              {order.shipEmail}
              {order.shipPhone && <><br />{order.shipPhone}</>}
            </address>
          </div>
          <div className="rounded-xl border border-stone-200 bg-white p-5">
            <h2 className="mb-3 text-lg font-bold text-stone-900">Payment</h2>
            <p className="text-sm text-stone-600">{order.paymentMethod}</p>
          </div>
        </div>
      </div>
    </div>
  );
}
