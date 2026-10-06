# Northwind Market — Frontend

Modern React storefront for the already-modernized **legacy-ecom-modern-dotnet**
.NET 8 Minimal API backend. This frontend recreates the customer-facing
functionality of the original .NET 4.7 MVC / Razor / jQuery application (see
the class-plan PDF in `workspace/user/files/`) against the existing REST API.

**The backend is frozen.** This project contains zero backend code and makes
zero backend changes. All business rules (pricing, discounts, cart totals,
shipping, tax, checkout validation, order creation, auth) stay server-side;
the UI only displays data and submits forms.

## Stack

- React 19 + TypeScript + Vite
- React Router (client-side routing)
- Tailwind CSS v4
- No UI kit — small hand-built components

## Prerequisites

- Node.js 20+ and npm
- The backend API running (see the repo's `docs/Running-Locally.md`).
  Default: `http://localhost:5174`.

## Run (development)

```bash
cd frontend
npm install
npm run dev
```

Open http://localhost:5173.

### API connection

The backend has no CORS configured (it is treated as frozen), so the Vite dev
server proxies `/api` to the backend — same-origin from the browser's point of
view, which is what makes cookie auth + session cart + XSRF work with no
backend change.

```ts
// vite.config.ts
server: { proxy: { '/api': { target: 'http://localhost:5174', changeOrigin: true } } }
```

Point at a different backend with:

```bash
VITE_API_PROXY_TARGET=http://localhost:5099 npm run dev
```

For production, serve the built `dist/` from the same origin as the API
(reverse proxy) or enable CORS on the backend — the built app itself is static.

## Build

```bash
npm run build   # type-checks (tsc) + produces dist/
```

## Project structure

```
frontend/src/
  api/            # one module per backend area; no raw fetch() in components
    client.ts     # fetch wrapper: credentials, XSRF, ProblemDetails errors
    auth.ts       # /api/auth/*
    catalog.ts    # /api/products/*, /api/categories/*
    cart.ts       # /api/cart/*
    checkout.ts   # /api/checkout/*
    orders.ts     # /api/orders/*
  state/
    AuthContext.tsx   # user session (GET /api/auth/me, login/register/logout)
    CartContext.tsx   # cart + mini-cart drawer state
  components/     # Header, Footer, ProductCard, ProductImage, MiniCart,
                  # Field, Pagination, ProtectedRoute
  pages/          # Home, CatalogPage, ProductDetailPage, CartPage,
                  # CheckoutPage, LoginPage, RegisterPage, OrdersPage,
                  # OrderDetailPage
  types.ts        # DTO mirrors (camelCase, matching ASP.NET Core JSON)
  utils/format.ts # money/date formatting
```

## Legacy → modern mapping

| Legacy (Razor) | Modern (React) |
|---|---|
| `_Layout.cshtml` | `App.tsx` layout shell |
| `_Header.cshtml` | `components/Header.tsx` |
| `_Footer.cshtml` | `components/Footer.tsx` |
| `_MiniCart.cshtml` | `components/MiniCart.tsx` (drawer) |
| `_ProductCard.cshtml` | `components/ProductCard.tsx` |
| `Product/Index.cshtml` | `pages/CatalogPage.tsx` |
| `Product/_ProductList.cshtml` (AJAX partial) | same page, API-driven re-render |
| `Product/Detail.cshtml` (+ Fancybox) | `pages/ProductDetailPage.tsx` (lightbox) |
| `_Sidebar.cshtml` (category tree) | expandable `CategoryNode` in `CatalogPage.tsx` |
| `Cart/Index.cshtml` | `pages/CartPage.tsx` |
| `Checkout/Address.cshtml` | `CheckoutPage.tsx` step 1 |
| `Checkout/Shipping.cshtml` | `CheckoutPage.tsx` step 2 |
| `Checkout/Payment.cshtml` | `CheckoutPage.tsx` step 3 |
| `Checkout/Confirmation` | `CheckoutPage.tsx` step 4 |
| `Account/Login.cshtml` | `pages/LoginPage.tsx` |
| `Account/Register.cshtml` | `pages/RegisterPage.tsx` |
| `Account/Orders.cshtml` (+ DataTables) | `pages/OrdersPage.tsx` (plain table) |

## Notes

- Product images: seed data references `/Content/images/...` paths the API
  does not serve; `ProductImage` renders a neutral placeholder when an image
  fails to load.
- Checkout enforces the backend's wizard order (Address → Shipping → Payment);
  a 409 from the API sends the user back to the address step.
- Payment is demo-only; the backend validates card fields but makes no real charge.
