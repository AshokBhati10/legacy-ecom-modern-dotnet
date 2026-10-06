/* Legacy eCommerce storefront - centralized API layer (jQuery).
 *
 * All pages talk to the .NET 8 Minimal API through this module.
 * - Same-origin /api calls (dev server proxies /api to the backend).
 * - Cookies (auth + session cart) flow via xhrFields.withCredentials.
 * - XSRF: GET /api/auth/xsrf-token sets the readable XSRF-TOKEN cookie;
 *   echo it as the X-XSRF-TOKEN header on state-changing requests.
 *   Tokens are identity-bound: refresh after login/register/logout.
 * - API errors surface as rejected promises carrying { status, message, errors }.
 */
var legacyApi = legacyApi || {};

(function ($) {
    'use strict';

    var API_BASE = '/api';
    var xsrfToken = null;

    function readCookie(name) {
        var match = document.cookie.match(new RegExp('(?:^|;\\s*)' + name + '=([^;]*)'));
        return match ? decodeURIComponent(match[1]) : null;
    }

    function ensureXsrf() {
        if (xsrfToken) {
            return $.Deferred().resolve(xsrfToken).promise();
        }
        return $.ajax({
            url: API_BASE + '/auth/xsrf-token',
            method: 'GET',
            xhrFields: { withCredentials: true }
        }).then(function () {
            xsrfToken = readCookie('XSRF-TOKEN');
            return xsrfToken;
        });
    }

    function refreshXsrf() {
        xsrfToken = null;
        return ensureXsrf();
    }

    function parseError(jqXHR) {
        var message = 'Request failed. Please try again.';
        var errors = {};
        try {
            var data = jqXHR.responseJSON || JSON.parse(jqXHR.responseText || '{}');
            if (data.title) message = data.title;
            if (data.detail) message = data.detail;
            if (data.errors) {
                errors = data.errors;
                var first = Object.keys(data.errors)[0];
                if (first && data.errors[first] && data.errors[first].length) {
                    message = data.errors[first][0];
                }
            } else if (typeof data === 'string' && data) {
                message = data;
            }
        } catch (e) { /* keep default */ }
        if (jqXHR.status === 0) message = 'Network error. Is the server running?';
        if (jqXHR.status === 401) message = 'Please log in to continue.';
        if (jqXHR.status === 403) message = 'You are not allowed to do that.';
        return { status: jqXHR.status, message: message, errors: errors };
    }

    function request(method, path, data, needsXsrf) {
        var run = function () {
            var options = {
                url: API_BASE + path,
                method: method,
                dataType: 'json',
                xhrFields: { withCredentials: true }
            };
            if (needsXsrf && xsrfToken) {
                options.headers = { 'X-XSRF-TOKEN': xsrfToken };
            }
            if (data !== undefined && data !== null) {
                options.contentType = 'application/json; charset=utf-8';
                options.data = JSON.stringify(data);
            }
            return $.ajax(options).fail(function (jqXHR) {
                // surface parsed error on the rejection
                var d = $.Deferred();
                d.reject(parseError(jqXHR), jqXHR);
                return d.promise();
            });
        };
        if (needsXsrf) {
            return ensureXsrf().then(run);
        }
        return run();
    }

    function get(path) { return request('GET', path, null, false); }
    function post(path, data) { return request('POST', path, data, true); }
    function put(path, data) { return request('PUT', path, data, true); }
    function del(path) { return request('DELETE', path, null, true); }

    // -- Catalog -----------------------------------------------------------
    function getProducts(params) {
        var clean = {};
        $.each(params || {}, function (k, v) {
            if (v !== undefined && v !== null && v !== '') clean[k] = v;
        });
        var qs = $.param(clean);
        return get('/products' + (qs ? '?' + qs : ''));
    }
    function getProduct(id) { return get('/products/' + id); }
    function getFeatured(take) { return get('/products/featured' + (take ? '?take=' + take : '')); }
    function getCategories() { return get('/categories'); }
    function getCategoryTree(parentId) {
        return get('/categories/tree' + (parentId ? '?parentId=' + parentId : ''));
    }

    // -- Cart --------------------------------------------------------------
    function getCart() { return get('/cart/'); }
    function getMiniCart() { return get('/cart/mini'); }
    function addToCart(productId, variantId, quantity) {
        return post('/cart/items', { productId: productId, variantId: variantId || null, quantity: quantity || 1 });
    }
    function updateCartItem(productId, variantId, quantity) {
        return put('/cart/items', { productId: productId, variantId: variantId || null, quantity: quantity });
    }
    function removeCartItem(productId, variantId) {
        var qs = '?productId=' + productId + (variantId ? '&variantId=' + variantId : '');
        return del('/cart/items' + qs);
    }
    function clearCart() { return post('/cart/clear', {}); }

    // -- Checkout ----------------------------------------------------------
    function getCheckoutAddress() { return get('/checkout/address'); }
    function saveCheckoutAddress(address) { return post('/checkout/address', address); }
    function getShippingOptions() { return get('/checkout/shipping-options'); }
    function setShippingMethod(method) { return post('/checkout/shipping', { shippingMethod: method }); }
    function getCheckoutSummary(shippingMethod) {
        return get('/checkout/summary' + (shippingMethod ? '?shippingMethod=' + encodeURIComponent(shippingMethod) : ''));
    }
    function placeOrder(payment) { return post('/checkout/place-order', payment); }

    // -- Auth --------------------------------------------------------------
    function register(data) {
        return post('/auth/register', data).then(function (r) { refreshXsrf(); return r; });
    }
    function login(email, password) {
        return post('/auth/login', { email: email, password: password })
            .then(function (r) { refreshXsrf(); return r; });
    }
    function logout() {
        return post('/auth/logout', {}).then(function (r) { refreshXsrf(); return r; });
    }
    function me() { return get('/auth/me'); }

    // -- Orders ------------------------------------------------------------
    function getOrders() { return get('/orders/'); }
    function getOrder(id) { return get('/orders/' + id); }

    // -- Formatting helpers ------------------------------------------------
    function money(value) {
        if (value === null || value === undefined) return '';
        return '$' + Number(value).toFixed(2);
    }
    function formatDate(value) {
        if (!value) return '';
        var d = new Date(value);
        return d.toLocaleString();
    }
    function esc(value) {
        return String(value === null || value === undefined ? '' : value)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    legacyApi.getProducts = getProducts;
    legacyApi.getProduct = getProduct;
    legacyApi.getFeatured = getFeatured;
    legacyApi.getCategories = getCategories;
    legacyApi.getCategoryTree = getCategoryTree;
    legacyApi.getCart = getCart;
    legacyApi.getMiniCart = getMiniCart;
    legacyApi.addToCart = addToCart;
    legacyApi.updateCartItem = updateCartItem;
    legacyApi.removeCartItem = removeCartItem;
    legacyApi.clearCart = clearCart;
    legacyApi.getCheckoutAddress = getCheckoutAddress;
    legacyApi.saveCheckoutAddress = saveCheckoutAddress;
    legacyApi.getShippingOptions = getShippingOptions;
    legacyApi.setShippingMethod = setShippingMethod;
    legacyApi.getCheckoutSummary = getCheckoutSummary;
    legacyApi.placeOrder = placeOrder;
    legacyApi.register = register;
    legacyApi.login = login;
    legacyApi.logout = logout;
    legacyApi.me = me;
    legacyApi.getOrders = getOrders;
    legacyApi.getOrder = getOrder;
    legacyApi.refreshXsrf = refreshXsrf;
    legacyApi.money = money;
    legacyApi.formatDate = formatDate;
    legacyApi.esc = esc;
})(jQuery);
