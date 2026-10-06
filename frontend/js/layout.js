/* Legacy eCommerce storefront - shared layout behaviour (jQuery 3.4.1).
 *
 * - Loads the header/footer partials into #site-header / #site-footer.
 * - Resolves the current user via GET /api/auth/me and toggles nav links.
 * - Refreshes the header mini-cart via GET /api/cart/mini after cart changes.
 * - Global AJAX cart helpers used by every add-to-cart form.
 */
var legacyLayout = legacyLayout || {};

(function ($) {
    'use strict';

    // Refresh the header mini-cart fragment.
    function refreshMiniCart() {
        legacyApi.getMiniCart()
            .done(function (mini) {
                $('#mini-cart').html(
                    '<a href="cart.html" title="View cart">' +
                    '<span class="glyphicon glyphicon-shopping-cart"></span> ' +
                    '<span class="cart-count-badge">' + mini.itemCount + '</span> ' +
                    '<span class="cart-total-label">' + legacyApi.money(mini.subTotal) + '</span></a>');
            })
            .fail(function () {
                $('#mini-cart').html(
                    '<a href="cart.html"><span class="glyphicon glyphicon-shopping-cart"></span> ' +
                    '<span class="cart-count-badge">0</span></a>');
            });
    }

    // Called on every add-to-cart success (mirrors the legacy OnSuccess hook).
    legacyLayout.cartAdded = function () {
        refreshMiniCart();
        var alert = $('<div class="alert alert-success alert-dismissible" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span></button>' +
            'Product added to your cart.</div>');
        $('.body-content').first().prepend(alert);
        window.setTimeout(function () {
            alert.fadeOut(function () { alert.remove(); });
        }, 2500);
    };

    legacyLayout.cartFailed = function (err) {
        window.alert((err && err.message) || 'Could not add the product to the cart.');
    };

    // Wire a form.post add-to-cart form (used by product cards + detail page).
    legacyLayout.bindAddToCart = function ($form) {
        $form.on('submit', function (e) {
            e.preventDefault();
            var productId = parseInt($form.find('[name="productId"]').val(), 10);
            var variantRaw = $form.find('[name="variantId"]').val();
            var variantId = variantRaw ? parseInt(variantRaw, 10) : null;
            var qtyRaw = $form.find('[name="quantity"]').val();
            var quantity = qtyRaw ? parseInt(qtyRaw, 10) : 1;
            var $btn = $form.find('button[type="submit"]');
            $btn.prop('disabled', true);
            legacyApi.addToCart(productId, variantId, quantity)
                .done(function () { legacyLayout.cartAdded(); })
                .fail(legacyLayout.cartFailed)
                .always(function () { $btn.prop('disabled', false); });
        });
    };

    // Show a dismissible alert at the top of the page body.
    legacyLayout.alert = function (kind, message) {
        var alert = $('<div class="alert alert-' + kind + ' alert-dismissible" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span></button>' +
            legacyApi.esc(message) + '</div>');
        $('.body-content').first().prepend(alert);
        return alert;
    };

    // Render server-side validation errors into a .validation-summary box.
    legacyLayout.showErrors = function ($box, err) {
        $box.empty().removeClass('hidden');
        var list = $('<ul></ul>');
        if (err && err.errors) {
            $.each(err.errors, function (field, messages) {
                $.each(messages, function (i, m) { list.append($('<li></li>').text(m)); });
            });
        }
        if (!list.children().length && err && err.message) {
            list.append($('<li></li>').text(err.message));
        }
        $box.append(list);
        $(window).scrollTop(0);
    };

    // Redirect to login when the API says 401 and we are on a protected page.
    legacyLayout.requireAuth = function () {
        return legacyApi.me().then(
            function (user) { return user; },
            function (err) {
                if (err && err.status === 401) {
                    window.location.href = 'login.html?returnUrl=' + encodeURIComponent(window.location.pathname + window.location.search);
                    return $.Deferred().reject(err).promise();
                }
                return $.Deferred().reject(err).promise();
            });
    };

    $(function () {
        // Load header/footer partials, then wire them up.
        var headerDone = $.get('partials/header.html', function (html) {
            $('#site-header').html(html);
        });
        $.get('partials/footer.html', function (html) {
            $('#site-footer').html(html);
        });

        headerDone.done(function () {
            // Active nav item.
            var page = $('body').data('page');
            if (page) {
                $('.navbar-nav li[data-nav="' + page + '"]').addClass('active');
            }

            // Auth state -> toggle nav links.
            legacyApi.me()
                .done(function (user) {
                    $('.auth-anon').addClass('hidden');
                    $('.auth-user').removeClass('hidden');
                    var name = user.firstName || user.email || 'there';
                    $('#nav-hello').text('Hello, ' + name + '!');
                })
                .fail(function () { /* anonymous: keep anon links */ });

            // Logout posts to the API (XSRF-protected), then reloads.
            $('#logout-form').on('submit', function (e) {
                e.preventDefault();
                legacyApi.logout().always(function () { window.location.href = 'index.html'; });
            });

            refreshMiniCart();
        });

        legacyLayout.refreshMiniCart = refreshMiniCart;
    });
})(jQuery);
