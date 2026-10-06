import { test, expect } from '@playwright/test';

/**
 * Full customer journey on the legacy-stack storefront (jQuery + Bootstrap 3)
 * against the real .NET 8 backend (via the dev-server /api proxy).
 */

const EMAIL = `qa.legacy.${Date.now()}@example.com`;

test.describe('legacy storefront customer journey', () => {
  test('browse, search, detail, cart, register, checkout, orders, logout', async ({ page }) => {
    // 1. HOME
    await page.goto('/index.html');
    await expect(page.locator('.hero h1')).toContainText('Everything you need');
    await expect(page.locator('#home-categories .category-card')).toHaveCount(6, { timeout: 10000 });
    await expect(page.locator('#home-featured .product-card').first()).toBeVisible({ timeout: 10000 });

    // 2. CATALOG + SEARCH + CATEGORY
    await page.goto('/catalog.html');
    await expect(page.locator('#product-list .product-card').first()).toBeVisible({ timeout: 10000 });
    await expect(page.locator('#product-count')).toContainText('12 product(s)');

    // search
    await page.locator('#filter-q').fill('headphones');
    await page.locator('#product-filter-form button[type="submit"]').click();
    await expect(page.locator('#product-count')).toContainText('for "headphones"');
    await expect(page.locator('#product-list')).toContainText('Wireless Headphones Pro');

    // empty search state
    await page.locator('#filter-q').fill('zzzznonexistent');
    await page.locator('#product-filter-form button[type="submit"]').click();
    await expect(page.locator('#product-list')).toContainText('No products found');

    // clear + nested category (Audio under Electronics)
    await page.locator('#filter-clear').click();
    await page.locator('.cat-toggle[data-cat-id="1"]').click();
    await expect(page.locator('.cat-children[data-parent="1"] a[data-cat-id="2"]')).toBeVisible();
    await page.locator('.cat-children[data-parent="1"] a[data-cat-id="2"]').click();
    await expect(page.locator('#product-list')).toContainText('Wireless Headphones Pro');

    // 3. PRODUCT DETAIL
    await page.locator('#product-list .product-card a', { hasText: 'Wireless Headphones Pro' }).first().click();
    await expect(page).toHaveURL(/product\.html\?id=\d+/);
    await expect(page.locator('#product-detail h1')).toContainText('Wireless Headphones Pro', { timeout: 10000 });

    // variant (pills) + quantity 2 + add to cart
    await page.locator('.variant-pills input').nth(1).check();
    await page.locator('#quantity').fill('2');
    await page.locator('#detail-add-form button[type="submit"]').click();
    await expect(page.locator('.alert-success')).toContainText('Product added to your cart', { timeout: 10000 });

    // mini-cart updated in header (badge + total)
    await expect(page.locator('#mini-cart .cart-count-badge')).toContainText('2', { timeout: 10000 });

    // 4. FULL CART — update quantity
    await page.goto('/cart.html');
    await expect(page.locator('#cart-content table tbody tr')).toHaveCount(1, { timeout: 10000 });
    await page.locator('.cart-update-form [name="quantity"]').fill('3');
    await page.locator('.cart-update-form [name="quantity"]').dispatchEvent('change');
    await expect(page.locator('.cart-summary-card')).toContainText('Subtotal (3 items)', { timeout: 10000 });

    // 5. REGISTER
    await page.goto('/register.html');
    await page.locator('#FirstName').fill('QA');
    await page.locator('#LastName').fill('Tester');
    await page.locator('#Email').fill(EMAIL);
    await page.locator('#Password').fill('Test1234!');
    await page.locator('#ConfirmPassword').fill('Test1234!');
    await page.locator('#register-form button[type="submit"]').click();
    await expect(page).toHaveURL(/index\.html/, { timeout: 10000 });
    await expect(page.locator('#nav-hello')).toContainText('Hello, QA', { timeout: 10000 });

    // 6. CHECKOUT — address step
    await page.goto('/checkout.html');
    await page.locator('#Email').fill(EMAIL);
    await page.locator('#FirstName').fill('QA');
    await page.locator('#LastName').fill('Tester');
    await page.locator('#Street').fill('1 Test Way');
    await page.locator('#City').fill('Springfield');
    await page.locator('#State').fill('IL');
    await page.locator('#PostalCode').fill('62701');
    await page.locator('#Country').fill('USA');
    await page.locator('#address-form button[type="submit"]').click();

    // shipping step
    await expect(page.locator('#step-shipping')).toBeVisible({ timeout: 10000 });
    await expect(page.locator('#shipping-options')).toContainText('Standard');
    await page.locator('input[name="ShippingMethod"][value="Express"]').check();
    await page.locator('#shipping-form button[type="submit"]').click();

    // payment step
    await expect(page.locator('#step-payment')).toBeVisible({ timeout: 10000 });
    await expect(page.locator('#payment-summary')).toContainText('Total:');
    await page.locator('input[name="PaymentMethod"][value="Card"]').check();
    await expect(page.locator('#card-fields')).toBeVisible();
    await page.locator('#CardholderName').fill('QA Tester');
    await page.locator('#CardNumber').fill('4111111111111111');
    await page.locator('#ExpiryMonth').selectOption('12');
    await page.locator('#ExpiryYear').selectOption('2030');
    await page.locator('#Cvv').fill('123');
    await page.locator('#payment-form button[type="submit"]').click();

    // confirmation
    await expect(page.locator('#step-confirm')).toBeVisible({ timeout: 15000 });
    const orderNumber = await page.locator('#confirm-order-number').textContent();
    expect(orderNumber).toMatch(/^LE-\d{8}-[A-Z0-9]{6}$/);

    // 7. ORDERS (DataTables)
    await page.goto('/orders.html');
    await expect(page.locator('#orders-table')).toBeVisible({ timeout: 10000 });
    await expect(page.locator('#orders-table')).toContainText(orderNumber!.trim());
    await page.locator('#orders-table a.btn', { hasText: 'Details' }).first().click();
    await expect(page).toHaveURL(/order-detail\.html\?id=\d+/);
    await expect(page.locator('#order-detail h2')).toContainText(orderNumber!.trim(), { timeout: 10000 });
    await expect(page.locator('#order-detail')).toContainText('1 Test Way');

    // 8. LOGOUT
    await page.locator('#logout-form button[type="submit"]').click();
    await expect(page).toHaveURL(/index\.html/, { timeout: 10000 });
    await expect(page.locator('.auth-anon a', { hasText: 'Log in' }).first()).toBeVisible({ timeout: 10000 });

    // 9. PROTECTED PAGE redirects to login
    await page.goto('/orders.html');
    await expect(page).toHaveURL(/login\.html\?returnUrl=/, { timeout: 10000 });
  });

  test('mobile viewport renders without major overflow', async ({ page }) => {
    await page.setViewportSize({ width: 390, height: 844 });
    await page.goto('/index.html');
    await expect(page.locator('.hero h1')).toBeVisible({ timeout: 10000 });
    // navbar toggle
    await page.locator('.navbar-toggle').click();
    await expect(page.locator('.navbar-collapse.in a', { hasText: 'Shop' }).first()).toBeVisible();

    await page.goto('/catalog.html');
    await expect(page.locator('#product-list .product-card').first()).toBeVisible({ timeout: 10000 });

    await page.goto('/cart.html');
    await expect(page.locator('#cart-content')).toBeVisible({ timeout: 10000 });
  });

  test('validation errors surface on bad input', async ({ page }) => {
    await page.goto('/register.html');
    // Weak password -> server-side validation error in the summary box.
    await page.locator('#FirstName').fill('QA');
    await page.locator('#LastName').fill('T');
    await page.locator('#Email').fill('weak-password@example.com');
    await page.locator('#Password').fill('123');
    await page.locator('#ConfirmPassword').fill('123');
    // bypass client-side minlength so the server rejects it
    await page.evaluate(() => { (window as any).jQuery('#register-form').validate().destroy(); });
    await page.locator('#register-form button[type="submit"]').click();
    await expect(page.locator('#register-errors')).toBeVisible({ timeout: 10000 });
  });
});
