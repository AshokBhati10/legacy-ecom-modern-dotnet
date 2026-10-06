import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { checkoutApi } from '../api/checkout';
import { ApiError } from '../api/client';
import { useAuth } from '../state/AuthContext';
import { useCart } from '../state/CartContext';
import type {
  AddressRequest,
  CheckoutSummaryDto,
  ShippingOptionDto,
} from '../types';
import { money } from '../utils/format';
import Field from '../components/Field';

type Step = 'address' | 'shipping' | 'payment' | 'confirmation';

const STEPS: { key: Step; label: string }[] = [
  { key: 'address', label: 'Address' },
  { key: 'shipping', label: 'Shipping' },
  { key: 'payment', label: 'Payment' },
  { key: 'confirmation', label: 'Confirmation' },
];

const EMPTY_ADDRESS: AddressRequest = {
  email: '',
  firstName: '',
  lastName: '',
  phone: '',
  street: '',
  city: '',
  state: '',
  postalCode: '',
  country: '',
};

function Stepper({ current }: { current: Step }) {
  const idx = STEPS.findIndex((s) => s.key === current);
  return (
    <ol className="mb-8 flex items-center" aria-label="Checkout steps">
      {STEPS.map((s, i) => (
        <li key={s.key} className={`flex items-center ${i < STEPS.length - 1 ? 'flex-1' : ''}`}>
          <div className="flex items-center gap-2">
            <span
              className={`flex h-8 w-8 items-center justify-center rounded-full text-sm font-bold ${
                i < idx
                  ? 'bg-emerald-700 text-white'
                  : i === idx
                    ? 'bg-stone-900 text-white'
                    : 'bg-stone-200 text-stone-500'
              }`}
              aria-current={i === idx ? 'step' : undefined}
            >
              {i < idx ? '✓' : i + 1}
            </span>
            <span className={`hidden text-sm font-medium sm:inline ${i === idx ? 'text-stone-900' : 'text-stone-500'}`}>
              {s.label}
            </span>
          </div>
          {i < STEPS.length - 1 && (
            <div className={`mx-2 h-0.5 flex-1 sm:mx-4 ${i < idx ? 'bg-emerald-700' : 'bg-stone-200'}`} aria-hidden="true" />
          )}
        </li>
      ))}
    </ol>
  );
}

/** Modern equivalent of the legacy checkout wizard (Address/Shipping/Payment/Confirmation). */
export default function CheckoutPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { cart, refresh: refreshCart } = useCart();

  const [step, setStep] = useState<Step>('address');
  const [address, setAddress] = useState<AddressRequest>(EMPTY_ADDRESS);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [options, setOptions] = useState<ShippingOptionDto[]>([]);
  const [selectedMethod, setSelectedMethod] = useState<string | null>(null);
  const [summary, setSummary] = useState<CheckoutSummaryDto | null>(null);
  const [paymentMethod, setPaymentMethod] = useState('Card');
  const [card, setCard] = useState({ cardholderName: '', cardNumber: '', expiryMonth: '', expiryYear: '', cvv: '' });
  const [orderNumber, setOrderNumber] = useState<string | null>(null);
  const [orderId, setOrderId] = useState<number | null>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loaded, setLoaded] = useState(false);

  // Guard: empty cart -> cart page. Prefill address + user email.
  useEffect(() => {
    if (cart && cart.itemCount === 0 && step !== 'confirmation') {
      navigate('/cart', { replace: true });
      return;
    }
    if (!loaded) {
      checkoutApi
        .getAddress()
        .then((a) => {
          if (a) setAddress({ ...EMPTY_ADDRESS, ...a });
          else if (user) setAddress((prev) => ({ ...prev, email: user.email, firstName: user.firstName ?? '', lastName: user.lastName ?? '' }));
        })
        .catch(() => {})
        .finally(() => setLoaded(true));
    }
  }, [cart, step, navigate, loaded, user]);

  const fail = (err: unknown): boolean => {
    if (err instanceof ApiError) {
      if (err.errors) {
        const flat: Record<string, string> = {};
        for (const [field, msgs] of Object.entries(err.errors)) {
          flat[field.charAt(0).toLowerCase() + field.slice(1)] = msgs.join(' ');
        }
        setFieldErrors(flat);
        setError('Please fix the highlighted fields.');
      } else {
        setError(err.readableMessage());
      }
      // Backend enforces wizard order: a 409 means an earlier step is missing.
      if (err.status === 409) setStep('address');
      return true;
    }
    setError('Something went wrong. Please try again.');
    return true;
  };

  const submitAddress = async (e: FormEvent) => {
    e.preventDefault();
    setBusy(true);
    setError(null);
    setFieldErrors({});
    try {
      await checkoutApi.setAddress(address);
      const opts = await checkoutApi.shippingOptions();
      setOptions(opts.options);
      setSelectedMethod(opts.selectedMethod ?? opts.options[0]?.code ?? null);
      setSummary(opts.summary);
      setStep('shipping');
    } catch (err) {
      fail(err);
    } finally {
      setBusy(false);
    }
  };

  const submitShipping = async (e: FormEvent) => {
    e.preventDefault();
    if (!selectedMethod) {
      setError('Please choose a shipping method.');
      return;
    }
    setBusy(true);
    setError(null);
    try {
      await checkoutApi.setShippingMethod(selectedMethod);
      setSummary(await checkoutApi.summary());
      setStep('payment');
    } catch (err) {
      fail(err);
    } finally {
      setBusy(false);
    }
  };

  const submitPayment = async (e: FormEvent) => {
    e.preventDefault();
    setBusy(true);
    setError(null);
    setFieldErrors({});
    try {
      const body =
        paymentMethod === 'Card'
          ? {
              paymentMethod,
              cardholderName: card.cardholderName,
              cardNumber: card.cardNumber.replace(/\s+/g, ''),
              expiryMonth: card.expiryMonth ? Number(card.expiryMonth) : undefined,
              expiryYear: card.expiryYear ? Number(card.expiryYear) : undefined,
              cvv: card.cvv,
            }
          : { paymentMethod };
      const res = await checkoutApi.placeOrder(body);
      setOrderNumber(res.orderNumber);
      setOrderId(res.orderId);
      await refreshCart();
      setStep('confirmation');
      window.scrollTo({ top: 0 });
    } catch (err) {
      fail(err);
    } finally {
      setBusy(false);
    }
  };

  const setAddr = (k: keyof AddressRequest, v: string) =>
    setAddress((a) => ({ ...a, [k]: v }));

  return (
    <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6">
      <h1 className="text-3xl font-extrabold tracking-tight text-stone-900">Checkout</h1>
      <div className="mt-6">
        <Stepper current={step} />
      </div>

      {error && (
        <div className="mb-6 rounded-lg border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700" role="alert">
          {error}
        </div>
      )}

      {step === 'address' && (
        <form onSubmit={submitAddress} className="rounded-xl border border-stone-200 bg-white p-6">
          <h2 className="mb-5 text-lg font-bold text-stone-900">Shipping address</h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <div className="sm:col-span-2">
              <Field label="Email" type="email" required value={address.email} onChange={(e) => setAddr('email', e.target.value)} error={fieldErrors.email} />
            </div>
            <Field label="First name" required value={address.firstName} onChange={(e) => setAddr('firstName', e.target.value)} error={fieldErrors.firstName} />
            <Field label="Last name" required value={address.lastName} onChange={(e) => setAddr('lastName', e.target.value)} error={fieldErrors.lastName} />
            <div className="sm:col-span-2">
              <Field label="Phone (optional)" type="tel" value={address.phone ?? ''} onChange={(e) => setAddr('phone', e.target.value)} error={fieldErrors.phone} />
            </div>
            <div className="sm:col-span-2">
              <Field label="Street address" required value={address.street} onChange={(e) => setAddr('street', e.target.value)} error={fieldErrors.street} />
            </div>
            <Field label="City" required value={address.city} onChange={(e) => setAddr('city', e.target.value)} error={fieldErrors.city} />
            <Field label="State / Province" required value={address.state} onChange={(e) => setAddr('state', e.target.value)} error={fieldErrors.state} />
            <Field label="Postal code" required value={address.postalCode} onChange={(e) => setAddr('postalCode', e.target.value)} error={fieldErrors.postalCode} />
            <Field label="Country" required value={address.country} onChange={(e) => setAddr('country', e.target.value)} error={fieldErrors.country} />
          </div>
          <button
            type="submit"
            disabled={busy}
            className="mt-6 w-full rounded-lg bg-stone-900 px-6 py-3 text-sm font-bold text-white hover:bg-emerald-700 disabled:opacity-60"
          >
            {busy ? 'Saving…' : 'Continue to shipping'}
          </button>
        </form>
      )}

      {step === 'shipping' && (
        <form onSubmit={submitShipping} className="rounded-xl border border-stone-200 bg-white p-6">
          <h2 className="mb-5 text-lg font-bold text-stone-900">Shipping method</h2>
          <div className="space-y-3" role="radiogroup" aria-label="Shipping method">
            {options.map((o) => (
              <label
                key={o.code}
                className={`flex cursor-pointer items-center gap-4 rounded-lg border p-4 ${
                  selectedMethod === o.code ? 'border-emerald-700 bg-emerald-50' : 'border-stone-300 hover:border-stone-400'
                }`}
              >
                <input
                  type="radio"
                  name="shipping"
                  checked={selectedMethod === o.code}
                  onChange={() => setSelectedMethod(o.code)}
                  className="h-4 w-4 accent-emerald-700"
                />
                <div className="flex-1">
                  <p className="font-semibold text-stone-900">{o.name}</p>
                  <p className="text-sm text-stone-500">{o.description}</p>
                </div>
                <p className="font-bold text-stone-900">{money(o.cost)}</p>
              </label>
            ))}
          </div>
          {summary && (
            <dl className="mt-5 space-y-1.5 border-t border-stone-100 pt-4 text-sm">
              <div className="flex justify-between"><dt className="text-stone-600">Subtotal</dt><dd>{money(summary.subTotal)}</dd></div>
              <div className="flex justify-between"><dt className="text-stone-600">Shipping</dt><dd>{money(summary.shippingCost)}</dd></div>
              <div className="flex justify-between"><dt className="text-stone-600">Tax</dt><dd>{money(summary.taxAmount)}</dd></div>
              <div className="flex justify-between border-t border-stone-100 pt-2 text-base font-bold"><dt>Total</dt><dd>{money(summary.total)}</dd></div>
            </dl>
          )}
          <div className="mt-6 flex gap-3">
            <button
              type="button"
              onClick={() => setStep('address')}
              className="rounded-lg border border-stone-300 px-6 py-3 text-sm font-semibold text-stone-700 hover:bg-stone-100"
            >
              Back
            </button>
            <button
              type="submit"
              disabled={busy}
              className="flex-1 rounded-lg bg-stone-900 px-6 py-3 text-sm font-bold text-white hover:bg-emerald-700 disabled:opacity-60"
            >
              {busy ? 'Saving…' : 'Continue to payment'}
            </button>
          </div>
        </form>
      )}

      {step === 'payment' && (
        <form onSubmit={submitPayment} className="rounded-xl border border-stone-200 bg-white p-6">
          <h2 className="mb-5 text-lg font-bold text-stone-900">Payment</h2>

          <div className="mb-5 flex gap-2" role="radiogroup" aria-label="Payment method">
            {['Card', 'CashOnDelivery'].map((m) => (
              <button
                key={m}
                type="button"
                role="radio"
                aria-checked={paymentMethod === m}
                onClick={() => setPaymentMethod(m)}
                className={`rounded-lg border px-4 py-2.5 text-sm font-semibold ${
                  paymentMethod === m
                    ? 'border-emerald-700 bg-emerald-50 text-emerald-800'
                    : 'border-stone-300 text-stone-700 hover:border-stone-400'
                }`}
              >
                {m === 'Card' ? 'Credit / Debit card' : 'Cash on delivery'}
              </button>
            ))}
          </div>

          {paymentMethod === 'Card' && (
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="sm:col-span-2">
                <Field label="Cardholder name" required value={card.cardholderName} onChange={(e) => setCard({ ...card, cardholderName: e.target.value })} error={fieldErrors.cardholderName} />
              </div>
              <div className="sm:col-span-2">
                <Field label="Card number" required inputMode="numeric" placeholder="4111 1111 1111 1111" value={card.cardNumber} onChange={(e) => setCard({ ...card, cardNumber: e.target.value })} error={fieldErrors.cardNumber} hint="Demo checkout — no real charge is made." />
              </div>
              <Field label="Expiry month" required inputMode="numeric" placeholder="MM" value={card.expiryMonth} onChange={(e) => setCard({ ...card, expiryMonth: e.target.value })} error={fieldErrors.expiryMonth} />
              <Field label="Expiry year" required inputMode="numeric" placeholder="YYYY" value={card.expiryYear} onChange={(e) => setCard({ ...card, expiryYear: e.target.value })} error={fieldErrors.expiryYear} />
              <div className="sm:col-span-2 sm:max-w-[50%]">
                <Field label="CVV" required inputMode="numeric" placeholder="123" value={card.cvv} onChange={(e) => setCard({ ...card, cvv: e.target.value })} error={fieldErrors.cvv} />
              </div>
            </div>
          )}

          {summary && (
            <dl className="mt-5 space-y-1.5 border-t border-stone-100 pt-4 text-sm">
              <div className="flex justify-between"><dt className="text-stone-600">Items ({summary.itemCount})</dt><dd>{money(summary.subTotal)}</dd></div>
              <div className="flex justify-between"><dt className="text-stone-600">Shipping ({summary.shippingMethod})</dt><dd>{money(summary.shippingCost)}</dd></div>
              <div className="flex justify-between"><dt className="text-stone-600">Tax</dt><dd>{money(summary.taxAmount)}</dd></div>
              <div className="flex justify-between border-t border-stone-100 pt-2 text-base font-bold"><dt>Total</dt><dd>{money(summary.total)}</dd></div>
            </dl>
          )}

          <div className="mt-6 flex gap-3">
            <button
              type="button"
              onClick={() => setStep('shipping')}
              className="rounded-lg border border-stone-300 px-6 py-3 text-sm font-semibold text-stone-700 hover:bg-stone-100"
            >
              Back
            </button>
            <button
              type="submit"
              disabled={busy}
              className="flex-1 rounded-lg bg-emerald-700 px-6 py-3 text-sm font-bold text-white hover:bg-emerald-800 disabled:opacity-60"
            >
              {busy ? 'Placing order…' : `Place order${summary ? ` · ${money(summary.total)}` : ''}`}
            </button>
          </div>
        </form>
      )}

      {step === 'confirmation' && orderNumber && (
        <div className="rounded-xl border border-stone-200 bg-white p-8 text-center">
          <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-emerald-100">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-8 w-8 text-emerald-700" aria-hidden="true">
              <path d="M5 13l4 4L19 7" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </div>
          <h2 className="mt-4 text-2xl font-extrabold text-stone-900">Order placed!</h2>
          <p className="mt-2 text-stone-600">
            Thank you for your order. Your order number is{' '}
            <span className="font-bold text-stone-900">{orderNumber}</span>.
          </p>
          {summary && (
            <dl className="mx-auto mt-6 max-w-sm space-y-1.5 border-t border-stone-100 pt-4 text-left text-sm">
              <div className="flex justify-between"><dt className="text-stone-600">Items</dt><dd>{money(summary.subTotal)}</dd></div>
              <div className="flex justify-between"><dt className="text-stone-600">Shipping</dt><dd>{money(summary.shippingCost)}</dd></div>
              <div className="flex justify-between"><dt className="text-stone-600">Tax</dt><dd>{money(summary.taxAmount)}</dd></div>
              <div className="flex justify-between border-t border-stone-100 pt-2 font-bold"><dt>Total charged</dt><dd>{money(summary.total)}</dd></div>
            </dl>
          )}
          <div className="mt-8 flex flex-wrap justify-center gap-3">
            <Link
              to="/shop"
              className="rounded-lg border border-stone-300 px-6 py-2.5 text-sm font-semibold text-stone-700 hover:bg-stone-100"
            >
              Continue shopping
            </Link>
            {user && orderId !== null && (
              <Link
                to={`/orders/${orderId}`}
                className="rounded-lg bg-stone-900 px-6 py-2.5 text-sm font-semibold text-white hover:bg-emerald-700"
              >
                View order
              </Link>
            )}
            {!user && (
              <Link
                to="/register"
                className="rounded-lg bg-stone-900 px-6 py-2.5 text-sm font-semibold text-white hover:bg-emerald-700"
              >
                Create an account to track orders
              </Link>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
