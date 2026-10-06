# 06 — Cart and Checkout Flow

### Byte 19: Two carts — session for guests, database for members

**Builds on:** Bytes 11, 14

**In plain terms:**

There are two places a cart can live. Anonymous shoppers keep theirs in server-side session (a per-visitor slot, 25-minute idle timeout) as a small list of `SessionCartItem` records — just product id, variant id, quantity. Signed-in shoppers *also* get their cart persisted to the `CartItems` table on every change, so it survives across devices and sessions.

**The code:**

```csharp
// DTOs/Cart/CartDtos.cs — the whole session cart is this tiny record.
public record SessionCartItem(int ProductId, int? VariantId, int Quantity);
```

```csharp
// CartEndpoints.cs — every mutation writes session, then persists for members.
var result = cart.AddItem(http.Session.GetCartItems(), request.ProductId,
                          request.VariantId, request.Quantity);
// ...
http.Session.SetCartItems(result.Data!);
PersistForUser(http, cart);   // no-op when anonymous

private static void PersistForUser(HttpContext http, ICartService cart)
{
    var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!string.IsNullOrEmpty(userId))
        cart.SaveForUser(userId, http.Session.GetCartItems());
}
```

```csharp
// Infrastructure/SessionCartExtensions.cs — session access is JSON in, JSON out.
public static List<SessionCartItem> GetCartItems(this ISession session) { ... }
public static void SetCartItems(this ISession session, IEnumerable<SessionCartItem> items) { ... }
```

The key insight: the *session* is the source of truth during a visit; the database is a backup for signed-in users. Prices are never stored in either — they're recomputed from the product on every read (see `CartService.BuildLine`), so price changes apply immediately.

### Byte 20: Cart merge on login

**Builds on:** Byte 19

**In plain terms:**

If you add items as a guest and then log in, you don't lose anything. On sign-in, the persisted cart rows are loaded and merged into the session cart: matching lines have their quantities added together, new lines are appended. The merged result is written back to both places.

**The code:**

```csharp
// AuthEndpoints.cs — runs after every successful login/register.
private static void MergePersistedCart(HttpContext http, ICartService cart, string? userId)
{
    if (string.IsNullOrWhiteSpace(userId)) return;

    var merged = http.Session.GetCartItems().ToList();
    foreach (var saved in cart.LoadForUser(userId))
    {
        var existing = merged.FirstOrDefault(x =>
            x.ProductId == saved.ProductId && x.VariantId == saved.VariantId);
        if (existing is not null)
        {
            merged.Remove(existing);
            merged.Add(existing with { Quantity = existing.Quantity + saved.Quantity });
        }
        else
        {
            merged.Add(new SessionCartItem(saved.ProductId, saved.VariantId, saved.Quantity));
        }
    }
    http.Session.SetCartItems(merged);
    cart.SaveForUser(userId, merged);
}
```

Note the merge key is `(ProductId, VariantId)` — the same product in two variants stays two separate lines. If a "my cart disappeared after login" bug ever appears, this method and `CartRepository.Save` (which replaces all rows for the user) are the two places to look.

### Byte 21: The checkout wizard is a server-side state machine

**Builds on:** Bytes 19–20

**In plain terms:**

Checkout has three steps — address, shipping method, place order — and the server enforces the order. Your address and shipping choice are stored in session as you complete each step; jumping ahead returns `409 Conflict`. The client can't skip steps by calling URLs directly.

**The code:**

```csharp
// CheckoutEndpoints.cs — step 2 refuses to run before step 1.
group.MapGet("/shipping-options", (HttpContext http, ICartService cart, ICheckoutService checkout) =>
    {
        var address = http.Session.GetCheckoutAddress<AddressRequest>();
        if (address is null)
            return Results.Conflict(new { message = "Complete the address step first." });
        // ...
    });
```

```csharp
// PlaceOrder requires both earlier steps, then wipes the wizard state.
var address = http.Session.GetCheckoutAddress<AddressRequest>();
if (address is null)
    return Results.Conflict(new { message = "Complete the address step first." });

var shippingMethod = http.Session.GetCheckoutShippingMethod();
if (string.IsNullOrWhiteSpace(shippingMethod))
    return Results.Conflict(new { message = "Choose a shipping method first." });
// ... place the order ...
http.Session.ClearCheckoutState();
http.Session.SetLastOrderId(result.Data!.Id);
```

`409` here means "right request, wrong time" — distinct from `400` ("wrong request"). After a successful order the wizard state is cleared so a refresh can't double-submit, and the new order's id is remembered in session for the confirmation page (Byte 18).

### Byte 22: PlaceOrder — the snapshot principle

**Builds on:** Bytes 11, 21

**In plain terms:**

Placing an order freezes the purchase in time. Product names, SKUs, and unit prices are *copied* into `OrderLine` rows, and the address is copied onto the order — so later price changes or product edits can never rewrite history. The order gets a `LE-YYYYMMDD-XXXXXX` number (retried on the unlikely collision), and everything saves in one unit of work.

**The code:**

```csharp
// CheckoutService.cs — the order is built from the cart, then snapshotted.
var order = new Order
{
    OrderNumber = GenerateOrderNumber(),   // LE-20261005-A1B2C3
    // ...
    ShipFirstName = address.FirstName,     // address copied, not referenced
    // ...
    OrderLines = cart.Items.Select(l => new OrderLine
    {
        ProductId = l.ProductId,
        ProductName = l.ProductName,       // copied at purchase time
        Sku = l.Sku,
        UnitPrice = l.UnitPrice,           // frozen price
        Quantity = l.Quantity,
        LineTotal = l.LineTotal
    }).ToList()
};

_orders.Add(order);
_unitOfWork.SaveChanges();                 // order + lines persist together
```

Payment method defaults to `"CashOnDelivery"` when not specified; card payments are validated by the `ValidationFilter` on `PlaceOrderRequest` before this code runs. If an order's totals ever look wrong, recompute from `OrderLines` — they are the source of truth, not the current product catalog.

## PUTTING IT TOGETHER

The cart is session-first with a database backup for members, merged on login by `(ProductId, VariantId)`. Checkout is a server-enforced three-step state machine kept in session, and placing an order snapshots everything into `Order`/`OrderLine` rows in a single unit of work. Prices are always recomputed from the catalog until the moment of purchase — and frozen forever after.
