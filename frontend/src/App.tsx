import { BrowserRouter, Link, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './state/AuthContext';
import { CartProvider } from './state/CartContext';
import Header from './components/Header';
import Footer from './components/Footer';
import MiniCart from './components/MiniCart';
import ProtectedRoute from './components/ProtectedRoute';
import Home from './pages/Home';
import CatalogPage from './pages/CatalogPage';
import ProductDetailPage from './pages/ProductDetailPage';
import CartPage from './pages/CartPage';
import CheckoutPage from './pages/CheckoutPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import OrdersPage from './pages/OrdersPage';
import OrderDetailPage from './pages/OrderDetailPage';

function NotFound() {
  return (
    <div className="mx-auto max-w-7xl px-4 py-16 text-center sm:px-6">
      <p className="text-lg font-semibold text-stone-800">Page not found</p>
      <Link to="/" className="mt-3 inline-block text-sm font-semibold text-emerald-700 hover:underline">
        Back to home →
      </Link>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <CartProvider>
          <div className="flex min-h-screen flex-col">
            <Header />
            <main className="flex-1">
              <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/shop" element={<CatalogPage />} />
                <Route path="/products/:id" element={<ProductDetailPage />} />
                <Route path="/cart" element={<CartPage />} />
                <Route path="/checkout" element={<CheckoutPage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route
                  path="/orders"
                  element={
                    <ProtectedRoute>
                      <OrdersPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/orders/:id"
                  element={
                    <ProtectedRoute>
                      <OrderDetailPage />
                    </ProtectedRoute>
                  }
                />
                <Route path="*" element={<NotFound />} />
              </Routes>
            </main>
            <Footer />
            <MiniCart />
          </div>
        </CartProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}
