# Knowledge Bytes — the modern API

Short, story-driven answers to one question: **"When I use this website, what
happens behind the scenes?"** — now for the .NET 8 Minimal API version.

---

## Byte 1 — Where did the controllers go?

**Builds on:** nothing. Start here.

**In plain terms:** The old site had MVC controllers — classes with actions
returning web pages. The modern API has endpoint classes instead: small static
methods wired directly to URLs with `MapGet`/`MapPost`. No controllers, no
views — each endpoint takes a request, calls a service, and returns JSON.

**The code:** `src/LegacyEcom.Api/Endpoints/CatalogEndpoints.cs` maps
`GET /api/products` to a method that asks `CatalogService` for a listing and
returns it.

**PUTTING IT TOGETHER:** A URL is no longer "which controller action runs" but
"which one-line endpoint function runs". Everything after that — the real work —
lives in services, which is why the same logic can be unit-tested without HTTP.

---

## Byte 2 — Who does the math when I add to cart?

**Builds on:** Byte 1.

**In plain terms:** The endpoint itself does no math. It hands your
`productId` and `quantity` to `CartService`, which looks up the product,
picks the sale price when one exists, adds any variant surcharge, and caps the
line at 99 units. Price rules live in one place — `PriceCalculator` — so the
website can never disagree with itself.

**The code:** `CartService.AddItemAsync` in
`src/LegacyEcom.Application/Services/CartService.cs`.

**PUTTING IT TOGETHER:** Endpoints are waiters taking your order; services are
the kitchen doing the cooking. If a price rule ever changes, it changes in one
kitchen, not at every table.

---

## Byte 3 — Where is my cart when I'm not logged in?

**Builds on:** Byte 2.

**In plain terms:** In the server's session — a per-visitor memory slot keyed
by a session cookie. No account, no database row; the cart simply rides along
with your browser session. Log in, and the API pours that session cart into
your permanent database cart (adding quantities, still capped at 99) — exactly
like the old site did.

**The code:** `CartService` reads/writes session lines; `CartRepository`
persists `CartItems` rows for authenticated users.

**PUTTING IT TOGETHER:** Guest carts are sticky notes on the server; login
turns the sticky note into a ledger entry. You never lose what you picked up.

---

## Byte 4 — How does checkout remember my steps?

**Builds on:** Byte 3.

**In plain terms:** The Address → Shipping → Payment wizard is enforced by the
server, not by the page you're on. Your address and shipping choice are stored
in session as you complete each step, and the API refuses steps out of order
(HTTP 409). There is no "clever URL" that lets you skip ahead.

**The code:** `CheckoutEndpoints` + `CheckoutService` in the Application layer;
`GET /api/checkout/summary` shows the running total before you commit.

**PUTTING IT TOGETHER:** The wizard is a state machine on the server. The UI
just renders whatever step the server says you're on.

---

## Byte 5 — What happens when I click "Place order"?

**Builds on:** Bytes 2, 4.

**In plain terms:** The API freezes everything about the purchase: it copies
the product name, SKU, and unit price into `OrderLines` (so later price changes
can't rewrite history), computes subtotal + shipping + 8% tax, stamps an order
number like `LE-20261005-000123`, clears your cart, and — for guests —
remembers the order id in session so you can still view your own order.

**The code:** `CheckoutService.PlaceOrderAsync` — one unit of work, one
`SaveChanges`.

**PUTTING IT TOGETHER:** An order is a photograph of the cart at the moment of
purchase, not a link to it. That's why your receipt never changes.

---

## Byte 6 — How do I prove who I am without OWIN?

**Builds on:** Byte 1.

**In plain terms:** The old site used OWIN middleware and Identity 2. The
modern API uses ASP.NET Core Identity with a signed cookie. Logging in stores
an encrypted ticket in a cookie; every later request presents it, and the API
answers 401 instead of redirecting you to a login page — because API clients
aren't browsers loading pages.

**The code:** `AuthEndpoints` (`/api/auth/login`, `/me`, `/logout`); Identity
options in `ServiceExtensions` mirror the legacy password and lockout policy.

**PUTTING IT TOGETHER:** Same house rules as before (strong passwords, five
strikes lockout) — new lock on the door, and the door now speaks JSON.

---

## Byte 7 — What is the XSRF token dance?

**Builds on:** Byte 6.

**In plain terms:** Cookies are sent by browsers automatically — including by
malicious sites. So every change-making request must also carry a secret token
the attacker's site can't know: you fetch it from `/api/auth/xsrf-token`
(cookie `XSRF-TOKEN`), then echo it in the `X-XSRF-TOKEN` header. After login
or logout you fetch a fresh one, because the token is tied to who you are.

**The code:** Antiforgery wiring in `Program.cs`; enforced on all
POST/PUT/DELETE endpoints.

**PUTTING IT TOGETHER:** The cookie says *who* you are; the header proves the
request really came *from you*. Both must agree.

---

## Byte 8 — Where did the database-first model go?

**Builds on:** Bytes 1–7.

**In plain terms:** The EDMX diagram is gone. Each table now has a small C#
configuration class stating its columns, keys, and relationships, and a
*migration* — a timestamped C# script — builds the database from them. Table
names and shapes match the legacy schema, so the data model your business
knows is the data model the code uses; only the machinery changed.

**The code:** `src/LegacyEcom.Infrastructure/Data/Configurations/` and
`Data/Migrations/20261005143635_InitialCreate.cs`.

**PUTTING IT TOGETHER:** The database is no longer a picture you edit — it's a
versioned script the code owns. Deployments apply it automatically on startup.
