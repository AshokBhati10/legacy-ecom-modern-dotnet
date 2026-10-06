import { Link } from 'react-router-dom';

/** Modern equivalent of the legacy _Footer.cshtml partial. */
export default function Footer() {
  return (
    <footer className="border-t border-stone-200 bg-stone-900 text-stone-300">
      <div className="mx-auto grid max-w-7xl gap-8 px-4 py-12 sm:grid-cols-3 sm:px-6">
        <div>
          <p className="text-lg font-extrabold text-white">Northwind Market</p>
          <p className="mt-2 max-w-xs text-sm text-stone-400">
            A modern storefront for the classic e-commerce catalog — built on the
            modernized .NET 8 API.
          </p>
        </div>
        <div>
          <p className="text-sm font-semibold uppercase tracking-wide text-stone-400">Shop</p>
          <ul className="mt-3 space-y-2 text-sm">
            <li><Link to="/shop" className="hover:text-white">All products</Link></li>
            <li><Link to="/cart" className="hover:text-white">Cart</Link></li>
            <li><Link to="/checkout" className="hover:text-white">Checkout</Link></li>
          </ul>
        </div>
        <div>
          <p className="text-sm font-semibold uppercase tracking-wide text-stone-400">Account</p>
          <ul className="mt-3 space-y-2 text-sm">
            <li><Link to="/login" className="hover:text-white">Login</Link></li>
            <li><Link to="/register" className="hover:text-white">Register</Link></li>
            <li><Link to="/orders" className="hover:text-white">Order history</Link></li>
          </ul>
        </div>
      </div>
      <div className="border-t border-stone-800">
        <p className="mx-auto max-w-7xl px-4 py-4 text-xs text-stone-500 sm:px-6">
          Northwind Market demo storefront · Powered by the legacy-ecom-modern-dotnet API
        </p>
      </div>
    </footer>
  );
}
