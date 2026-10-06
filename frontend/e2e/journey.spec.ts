import { test, expect } from '@playwright/test';

/**
 * Full customer journey against the real backend (via the Vite /api proxy).
 * Backend must be running (default http://localhost:5174) with seed data.
 */

const EMAIL = `qa.e2e.${Date.now()}@example.com`;

test.describe('customer journey', () => {
  test('browse, search, detail, cart, register, checkout, orders, logout', async ({ page }) => {
    // 1. HOME
    await page.goto('/');
    await expect(page.getByText('Everything you need, delivered to your door.')).toBeVisible();
    await expect(page.getByText('Popular categories')).toBeVisible();
    await expect(page.getByText('Featured products')).toBeVisible();
    const featuredCards = page.locator('section', { hasText: 'Featured products' }).locator('a[href^="/products/"]');
    await expect(featuredCards.first()).toBeVisible();

    // 2. CATALOG + SEARCH + CATEGORY FILTER
    await page.getByRole('link', { name: 'Shop all products' }).click();
    await expect(page).toHaveURL(/\/shop/);
    await expect(page.getByRole('heading', { name: 'Shop' })).toBeVisible();

    // search
    const searchForm = page.locator('main form[role="search"]');
    await searchForm.getByRole('searchbox').fill('headphones');
    await searchForm.getByRole('button', { name: 'Search' }).click();
    await expect(page.getByText(/product.*for.*headphones/i)).toBeVisible();
    await expect(page.getByRole('link', { name: /wireless headphones pro/i }).first()).toBeVisible();

    // empty search state
    await searchForm.getByRole('searchbox').fill('zzzznonexistent');
    await searchForm.getByRole('button', { name: 'Search' }).click();
    await expect(page.getByText('No products found')).toBeVisible();

    // clear search -> category filter (Audio is nested under Electronics)
    await page.getByRole('button', { name: 'Clear' }).click();
    await page.getByRole('button', { name: 'Expand Electronics' }).click();
    await page.getByRole('button', { name: 'Audio', exact: true }).click();
    await expect(page).toHaveURL(/categoryId=/);

    // 3. PRODUCT DETAIL
    await page.getByRole('link', { name: /wireless headphones pro/i }).first().click();
    await expect(page).toHaveURL(/\/products\/\d+/);
    await expect(page.getByRole('heading', { name: /wireless headphones pro/i })).toBeVisible();

    // variant selection (headphones have variants)
    const variantGroup = page.getByRole('radiogroup', { name: 'Product variant' });
    await expect(variantGroup).toBeVisible();
    await variantGroup.getByRole('radio', { name: /arctic silver/i }).click();

    // quantity 2 + add to cart (detail-page button includes the total price)
    await page.getByRole('button', { name: 'Increase quantity' }).click();
    await page.getByRole('button', { name: /add to cart · \$/i }).click();

    // 4. MINI CART drawer opens
    const drawer = page.getByRole('dialog', { name: 'Shopping cart' });
    await expect(drawer).toBeVisible();
    await expect(drawer.getByText(/wireless headphones pro/i)).toBeVisible();
    await expect(drawer.getByText('Arctic Silver')).toBeVisible();
    await drawer.getByRole('button', { name: 'Close cart' }).click();

    // 5. FULL CART — change quantity
    await page.goto('/cart');
    await expect(page.getByRole('heading', { name: 'Shopping cart' })).toBeVisible();
    const subtotalBefore = await page.getByText('$', { exact: false }).last().textContent();
    await page.getByRole('button', { name: 'Increase quantity' }).click();
    await expect(page.getByText('3', { exact: true }).first()).toBeVisible({ timeout: 5000 });
    expect(await page.getByText('$', { exact: false }).last().textContent()).not.toBe(subtotalBefore);

    // 6. REGISTER
    await page.goto('/register');
    await page.getByLabel('First name').fill('QA');
    await page.getByLabel('Last name').fill('Tester');
    await page.getByLabel('Email').fill(EMAIL);
    await page.getByLabel('Password', { exact: true }).fill('Test1234!');
    await page.getByLabel('Confirm password').fill('Test1234!');
    await page.getByRole('button', { name: 'Register', exact: true }).click();
    await expect(page).toHaveURL(/\/$/, { timeout: 10000 });
    await expect(page.getByText('Hi, QA')).toBeVisible();

    // 7. CHECKOUT
    await page.goto('/checkout');
    // address step
    await page.getByLabel('Email').fill(EMAIL);
    await page.getByLabel('First name').fill('QA');
    await page.getByLabel('Last name').fill('Tester');
    await page.getByLabel('Phone (optional)').fill('555-0100');
    await page.getByLabel('Street address').fill('1 Test Way');
    await page.getByLabel('City').fill('Springfield');
    await page.getByLabel('State / Province').fill('IL');
    await page.getByLabel('Postal code').fill('62701');
    await page.getByLabel('Country').fill('USA');
    await page.getByRole('button', { name: 'Continue to shipping' }).click();

    // shipping step
    await expect(page.getByText('Shipping method')).toBeVisible({ timeout: 10000 });
    await expect(page.getByText('Standard')).toBeVisible();
    await page.getByRole('radio', { name: /express/i }).check();
    await page.getByRole('button', { name: 'Continue to payment' }).click();

    // payment step
    await expect(page.getByText('Payment', { exact: true }).first()).toBeVisible({ timeout: 10000 });
    await page.getByLabel('Cardholder name').fill('QA Tester');
    await page.getByLabel('Card number').fill('4111111111111111');
    await page.getByLabel('Expiry month').fill('12');
    await page.getByLabel('Expiry year').fill('2030');
    await page.getByLabel('CVV').fill('123');
    await page.getByRole('button', { name: /place order/i }).click();

    // confirmation
    await expect(page.getByText('Order placed!')).toBeVisible({ timeout: 15000 });
    const orderNumber = await page.locator('span.font-bold', { hasText: /^LE-/ }).textContent();
    expect(orderNumber).toMatch(/^LE-\d{8}-[A-Z0-9]{6}$/);

    // 8. ORDERS
    await page.goto('/orders');
    await expect(page.getByText(orderNumber!.trim())).toBeVisible({ timeout: 10000 });
    await page.getByRole('link', { name: 'Details' }).first().click();
    await expect(page).toHaveURL(/\/orders\/\d+/);
    await expect(page.getByText(/wireless headphones pro/i)).toBeVisible();
    await expect(page.getByText('1 Test Way')).toBeVisible();

    // 9. LOGOUT
    await page.getByRole('button', { name: 'Logout' }).click();
    await expect(page.getByRole('link', { name: 'Login' }).first()).toBeVisible();

    // 10. CHECKOUT WITH EMPTY CART redirects to cart
    await page.goto('/checkout');
    await expect(page).toHaveURL(/\/cart/);
  });

  test('mobile viewport renders without major overflow', async ({ page }) => {
    await page.setViewportSize({ width: 390, height: 844 });
    await page.goto('/');
    await expect(page.getByText('Everything you need, delivered to your door.')).toBeVisible();

    // mobile menu
    await page.getByRole('button', { name: 'Toggle menu' }).click();
    await expect(page.getByRole('link', { name: 'Shop' }).first()).toBeVisible();

    await page.goto('/shop');
    await expect(page.getByRole('heading', { name: 'Shop' })).toBeVisible();

    await page.goto('/cart');
    await expect(page.getByRole('heading', { name: 'Shopping cart' })).toBeVisible();
  });

  test('validation errors surface on bad input', async ({ page }) => {
    await page.goto('/register');
    // Valid email format (passes native validation) but weak password ->
    // server-side validation error should surface.
    await page.getByLabel('First name').fill('QA');
    await page.getByLabel('Last name').fill('T');
    await page.getByLabel('Email').fill('weak-password@example.com');
    await page.getByLabel('Password', { exact: true }).fill('123');
    await page.getByLabel('Confirm password').fill('123');
    await page.getByRole('button', { name: 'Register', exact: true }).click();
    await expect(page.getByRole('alert')).toBeVisible({ timeout: 10000 });
  });
});
