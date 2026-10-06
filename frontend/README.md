# Storefront (legacy-stack frontend)

**The backend is the completed .NET 8 modernization and is not being reverted to the legacy MVC architecture.**

This is a static HTML frontend built with the **original legacy frontend technology stack**, talking to the existing .NET 8 Minimal API as JSON. No backend code was changed for this frontend.

## Technology stack

| Legacy stack | Version | Purpose |
|---|---|---|
| jQuery | 3.4.1 | DOM, AJAX, events (`$(function () { ... })`, `$.ajax`) |
| jQuery UI | 1.12.1 | Dialogs (e.g. clear-cart confirm) |
| Bootstrap | 3.4.1 | Grid, navbar, panels, forms, modals, responsive layout |
| jQuery Validate | 1.19.x | Client-side form validation |
| jQuery Unobtrusive Validation | 3.2.x | `data-val-*` attribute rules |
| jQuery Unobtrusive Ajax | 3.2.x | AJAX form helpers |
| DataTables | 1.10.x | Order history table (sort/paging) |
| Fancybox | 3.5.x | Product image gallery lightbox |

No React, Vue, Angular, Tailwind, or build step. All libraries are vendored as
plain files under `css/` and `js/` (copied from the original application's
`Content/` and `Scripts/` folders).

## Why this stack

The original application was an ASP.NET MVC 5 site using exactly these
libraries. Rebuilding the storefront with them preserves the original
interaction model (server-style pages, jQuery AJAX partial updates, Bootstrap 3
components) while the .NET 8 Minimal API remains the only backend. The visual
language follows the legacy Bootstrap 3 patterns; the reference design
(museshopcart.webflow.io) was inspiration only.

## Architecture

```
Browser (HTML + jQuery)
    ↓  $.ajax JSON, cookies + X-XSRF-TOKEN
.NET 8 Minimal API (/api/...)
    ↓
Application / Service layer
    ↓
EF Core → SQL Server
```

Key difference from the original MVC app: the old app rendered Razor views on
the server (`Controller → View → HTML`); this frontend is static HTML that
calls the JSON API and renders with jQuery, because the .NET 8 backend is
API-only. No Razor, controllers, EF6, OWIN, or Identity 2 were reintroduced.

## Structure

```
frontend/
  index.html            Home (hero, categories, featured)
  catalog.html          Product listing: search, category tree, paging
  product.html?id=      Product detail: Fancybox gallery, variants
  cart.html             Cart: update qty / remove / clear (AJAX)
  checkout.html         Wizard: Address → Shipping → Payment → Confirmation
  login.html / register.html
  orders.html           Order history (DataTables)
  order-detail.html?id= Order detail
  partials/             header.html, footer.html (loaded via jQuery)
  css/                  bootstrap, jquery-ui, datatables, fancybox, site.css
  js/
    api.js              Centralized API layer (all /api calls, XSRF, errors)
    layout.js           Header/footer loading, auth state, mini-cart
    home.js / catalog.js / product.js / cart.js / checkout.js / auth.js / orders.js
  images/               no-image.png fallback (legacy behaviour)
  fonts/                Bootstrap glyphicons
  e2e/                  Playwright journey spec (dev-only, needs npm)
  dev-server.mjs        Dev-only static server + /api proxy (Node built-ins)
```

## Running

```bash
# 1. Start the backend (from the repo root)
cd src/LegacyEcom.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run   # http://localhost:5174

# 2. Start the storefront dev server (from frontend/)
cd frontend
node dev-server.mjs        # http://localhost:5173, proxies /api → backend
```

`dev-server.mjs` uses only Node built-ins (no `npm install` needed). It serves
the static files and proxies `/api/*` to the backend so cookie auth, session
cart, and XSRF work same-origin — no backend CORS change required. Override
the backend with `API_PROXY_TARGET=http://host:port`.

## API integration (`js/api.js`)

- `legacyApi` wraps every endpoint: catalog, cart, checkout, auth, orders.
- `withCredentials` on every request (auth cookie + session cart).
- XSRF: `GET /api/auth/xsrf-token` sets the readable `XSRF-TOKEN` cookie;
  `api.js` echoes it as `X-XSRF-TOKEN` on POST/PUT/DELETE and refreshes the
  token after login/register/logout (tokens are identity-bound).
- Errors parse ProblemDetails/validation dictionaries into
  `{ status, message, errors }`; 401s on protected pages redirect to login.

## Testing

```bash
# in a scratch dir (keeps frontend/ dependency-free):
npm init -y && npm i -D @playwright/test
npx playwright install chromium
npx playwright test /path/to/frontend/e2e/journey.spec.ts
```

The spec covers the full customer journey against the real backend:
home → catalog/search/category/paging → product detail (gallery, variants) →
add-to-cart → mini-cart → cart update/remove → register → login → checkout
(address/shipping/payment/confirmation) → order history (DataTables) →
order detail → logout → protected-page redirect, plus mobile viewport and
validation-error cases.
