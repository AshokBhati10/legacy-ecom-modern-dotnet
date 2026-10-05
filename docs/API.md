# API reference

Base URL: `http://localhost:5099`. JSON everywhere. Errors are RFC 7807
ProblemDetails.

## Conventions

- Success: `200 OK` (queries/updates), `201 Created` (register, place-order —
  with `Location`), `204 No Content` (logout, address save, clear cart).
- Failure: `400` validation, `401` not authenticated, `404` not found,
  `409` conflict (bad wizard order, duplicate email).
- Auth: cookie (`POST /api/auth/login`). Order history/detail and logout
  require authentication.
- Antiforgery: every `POST`/`PUT`/`DELETE` needs the `XSRF-TOKEN` cookie value
  echoed in the `X-XSRF-TOKEN` header. Get it from `GET /api/auth/xsrf-token`.
  **Refresh it after login, logout, or register** — tokens are identity-bound.

## Catalog

| Method | Path | Notes |
|---|---|---|
| GET | `/api/products` | `?q=` search (case-insensitive), `?categoryId=`, `?page=`, `?pageSize=` (max 100). Returns `{ products, totalCount, page, pageSize, totalPages }`. |
| GET | `/api/products/featured` | `?take=` (default 8) |
| GET | `/api/products/{id}` | Detail with images + variants; 404 if missing/inactive |
| GET | `/api/categories` | Active categories |
| GET | `/api/categories/tree` | Parent/child hierarchy |

## Cart (session for guests, persistent when logged in)

| Method | Path | Body |
|---|---|---|
| GET | `/api/cart` | Full cart: lines, subtotal, item count |
| GET | `/api/cart/mini` | `{ itemCount, subTotal }` for headers/badges |
| POST | `/api/cart/items` | `{ productId, variantId?, quantity }` — 404 unknown product/variant |
| PUT | `/api/cart/items` | `{ productId, variantId?, quantity }` — quantity 0 removes the line |
| DELETE | `/api/cart/items` | `{ productId, variantId? }` |
| POST | `/api/cart/clear` | Empties the cart |

Quantity is capped at 99 per line (existing + added).

## Checkout wizard

Steps must run in order; skipping ahead returns `409`.

| Method | Path | Notes |
|---|---|---|
| GET/POST | `/api/checkout/address` | Save/get shipping address (email, name, street, city, state, postal, country) |
| GET | `/api/checkout/shipping-options` | Standard ($4.99, free ≥ $50) and Express ($12.99); 409 before address |
| POST | `/api/checkout/shipping` | `{ shippingMethod: "Standard" \| "Express" }` |
| GET | `/api/checkout/summary` | Address + method + lines + subtotal + shipping + tax (8%) + total |
| POST | `/api/checkout/place-order` | `{ paymentMethod: "Card" \| "CashOnDelivery", cardNumber?, cardExpiry?, cardCvv?, cardName? }` — card fields required for Card; returns 201 with order id |

Placing the order snapshots prices into order lines, clears the cart, and (for
guests) records the order id in session so the buyer can view it.

## Orders

| Method | Path | Notes |
|---|---|---|
| GET | `/api/orders` | Auth required. Own order history. |
| GET | `/api/orders/{id}` | Auth required, or the guest session that placed it. Otherwise 404. |

## Auth

| Method | Path | Notes |
|---|---|---|
| GET | `/api/auth/xsrf-token` | `204`; sets `XSRF-TOKEN` cookie |
| POST | `/api/auth/register` | `{ firstName, lastName, email, password, confirmPassword }` → 201; signs the user in; 409 duplicate email |
| POST | `/api/auth/login` | `{ email, password, rememberMe? }` → 200; merges session cart; 401 bad credentials |
| POST | `/api/auth/logout` | → 204 |
| GET | `/api/auth/me` | Current user or 401 |

## Health

`GET /api/health` → `{ "status": "healthy", "time": "..." }` (no auth).
