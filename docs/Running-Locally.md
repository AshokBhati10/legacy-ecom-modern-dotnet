# Running locally

## Prerequisites

- .NET 8 SDK (`dotnet --list-sdks` should show 8.x)
- SQL Server reachable over TCP (Express or Developer). Note the server name,
  database name, login, and password.

## 1. Configure

```bash
export ConnectionStrings__EcommerceDb="Server=localhost;Database=LegacyEcommerceModernDb;User Id=sa;Password=<your-password>;TrustServerCertificate=True;Encrypt=True"
```

Optional: copy `src/LegacyEcom.Api/appsettings.example.json` values into
environment variables or user secrets. Do not put real passwords in
`appsettings.json`.

## 2. Build

```bash
dotnet build LegacyEcom.Modern.sln
```

## 3. Create + migrate the database

```bash
dotnet tool install --global dotnet-ef   # once
dotnet ef database update \
  --project src/LegacyEcom.Infrastructure \
  --startup-project src/LegacyEcom.Api
```

(The API also auto-applies migrations on startup.)

## 4. Run

```bash
cd src/LegacyEcom.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run   # seeds the catalog
```

- API: `http://localhost:5099`
- Swagger: `http://localhost:5099/swagger` (Development only)
- Health: `curl http://localhost:5099/api/health`

Run from the **API project directory** so `appsettings.*.json` resolve.
Starting the compiled DLL from another working directory skips Development
config and seeding.

## 5. Try the flow (curl)

```bash
# antiforgery token (cookie jar keeps session + auth cookies)
curl -c jar -s http://localhost:5099/api/auth/xsrf-token -o /dev/null
TOKEN=$(grep XSRF-TOKEN jar | awk '{print $NF}')

# catalog
curl -s "http://localhost:5099/api/products?q=headphones" | head -c 300

# cart
curl -b jar -c jar -s -X POST http://localhost:5099/api/cart/items \
  -H "Content-Type: application/json" -H "X-XSRF-TOKEN: $TOKEN" \
  -d '{"productId":1,"quantity":2}'

# register (also logs you in; refresh the token afterwards - it is identity-bound)
curl -b jar -c jar -s -X POST http://localhost:5099/api/auth/register \
  -H "Content-Type: application/json" -H "X-XSRF-TOKEN: $TOKEN" \
  -d '{"firstName":"Ada","lastName":"Lovelace","email":"ada@example.com","password":"Str0ng!Pass","confirmPassword":"Str0ng!Pass"}'
curl -c jar -b jar -s http://localhost:5099/api/auth/xsrf-token -o /dev/null
TOKEN=$(grep XSRF-TOKEN jar | awk '{print $NF}')

# checkout wizard: address -> shipping -> place order
curl -b jar -c jar -s -X POST http://localhost:5099/api/checkout/address \
  -H "Content-Type: application/json" -H "X-XSRF-TOKEN: $TOKEN" \
  -d '{"email":"ada@example.com","firstName":"Ada","lastName":"Lovelace","street":"1 Market St","city":"Springfield","state":"IL","postalCode":"62701","country":"USA"}' -o /dev/null -w "%{http_code}\n"
curl -b jar -s http://localhost:5099/api/checkout/shipping-options | head -c 200; echo
curl -b jar -c jar -s -X POST http://localhost:5099/api/checkout/shipping \
  -H "Content-Type: application/json" -H "X-XSRF-TOKEN: $TOKEN" \
  -d '{"shippingMethod":"Standard"}' -o /dev/null -w "%{http_code}\n"
curl -b jar -c jar -s -X POST http://localhost:5099/api/checkout/place-order \
  -H "Content-Type: application/json" -H "X-XSRF-TOKEN: $TOKEN" \
  -d '{"paymentMethod":"CashOnDelivery"}' -D -
```

## 6. Tests

```bash
dotnet test tests/LegacyEcom.Tests/LegacyEcom.Tests.csproj
```

39 tests: pricing rules, cart/checkout services (EF InMemory), and full API
integration (register → cart → checkout → order, auth/XSRF rejections).

> Sandbox note: in this particular container `dotnet test` cannot run because
> the runtime redirects `TcpClient` connections to the egress proxy, which
> breaks the vstest↔testhost channel. The suite was executed here with an
> in-process xUnit runner instead (39/39 pass). On any normal machine
> `dotnet test` works unchanged.
