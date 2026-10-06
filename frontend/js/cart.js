/* Legacy eCommerce storefront - shopping cart (jQuery).
 * The backend owns cart persistence; this page only calls the cart APIs
 * and re-renders, mirroring the legacy Cart/Index postback behaviour.
 */
(function ($) {
    'use strict';

    function render(cart) {
        var $c = $('#cart-content');
        if (!cart.items || !cart.items.length) {
            $c.html('<div class="alert alert-info">Your cart is empty. <a href="catalog.html">Continue shopping</a>.</div>');
            return;
        }
        var html = '<div class="row"><div class="col-md-8">' +
            '<table class="table cart-table"><thead><tr>' +
            '<th>Product</th><th class="text-right">Price</th><th class="qty-cell">Quantity</th><th class="text-right">Total</th><th></th></tr></thead><tbody>';
        $.each(cart.items, function (i, item) {
            var img = legacyApi.imageUrl(item.thumbnailUrl);
            html += '<tr data-product-id="' + item.productId + '" data-variant-id="' + (item.variantId || '') + '">' +
                '<td><div class="media" style="margin:0"><div class="media-left">' +
                '<img class="cart-item-media" src="' + legacyApi.esc(img) + '" alt="" onerror="legacyApi.imageFallback(this)" /></div>' +
                '<div class="media-body"><strong><a href="product.html?id=' + item.productId + '" style="color:var(--ink)">' +
                legacyApi.esc(item.productName) + '</a></strong>' +
                (item.variantName ? '<br /><small class="text-muted">' + legacyApi.esc(item.variantName) + '</small>' : '') + '</div></div></td>' +
                '<td class="text-right">' + legacyApi.money(item.unitPrice) + '</td>' +
                '<td><form class="form-inline cart-update-form">' +
                '<span class="qty-stepper"><button type="button" class="btn btn-default btn-sm qty-dec" aria-label="Decrease">&minus;</button>' +
                '<input type="number" name="quantity" value="' + item.quantity + '" min="0" max="99" aria-label="Quantity" />' +
                '<button type="button" class="btn btn-default btn-sm qty-inc" aria-label="Increase">+</button></span></form></td>' +
                '<td class="text-right"><strong>' + legacyApi.money(item.lineTotal) + '</strong></td>' +
                '<td class="text-right"><button type="button" class="btn btn-link btn-sm cart-remove" title="Remove" aria-label="Remove item">' +
                '<span class="glyphicon glyphicon-trash"></span></button></td>' +
                '</tr>';
        });
        html += '</tbody></table>' +
            '<p><button type="button" class="btn btn-link" id="cart-clear"><span class="glyphicon glyphicon-trash"></span> Clear cart</button> ' +
            '<a href="catalog.html" class="btn btn-link">Continue shopping</a></p></div>' +
            '<div class="col-md-4 cart-summary-card"><div class="panel panel-default">' +
            '<div class="panel-heading">Order summary</div><div class="panel-body">' +
            '<div class="totals-row"><span>Subtotal (' + cart.itemCount + ' items)</span><span>' + legacyApi.money(cart.subTotal) + '</span></div>' +
            '<div class="totals-row"><span>Shipping</span><span class="text-muted">at checkout</span></div>' +
            '<div class="totals-row grand"><span>Total</span><span>' + legacyApi.money(cart.subTotal) + '</span></div>' +
            '<a href="checkout.html" class="btn btn-success btn-lg btn-block" style="margin-top:12px">Proceed to Checkout</a>' +
            '</div></div></div></div>';
        $c.html(html);
    }

    function reload() {
        legacyApi.getCart()
            .done(function (cart) {
                render(cart);
                legacyLayout.refreshMiniCart();
            })
            .fail(function () {
                $('#cart-content').html('<p class="text-danger">Could not load your cart.</p>');
            });
    }

    $(function () {
        reload();

        function submitUpdate($form) {
            var $row = $form.closest('tr');
            var productId = parseInt($row.data('product-id'), 10);
            var variantRaw = $row.data('variant-id');
            var variantId = variantRaw === '' ? null : parseInt(variantRaw, 10);
            var quantity = parseInt($form.find('[name="quantity"]').val(), 10) || 0;
            legacyApi.updateCartItem(productId, variantId, quantity)
                .done(reload)
                .fail(function (err) { legacyLayout.alert('danger', err.message); });
        }

        // Stepper buttons update immediately (AJAX, no full-page reload).
        $('#cart-content').on('click', '.qty-dec', function () {
            var $input = $(this).closest('.qty-stepper').find('[name="quantity"]');
            $input.val(Math.max(0, (parseInt($input.val(), 10) || 0) - 1));
            submitUpdate($(this).closest('form'));
        });
        $('#cart-content').on('click', '.qty-inc', function () {
            var $input = $(this).closest('.qty-stepper').find('[name="quantity"]');
            $input.val(Math.min(99, (parseInt($input.val(), 10) || 0) + 1));
            submitUpdate($(this).closest('form'));
        });
        // Manual edit still submits on Enter/blur.
        $('#cart-content').on('change', '.cart-update-form [name="quantity"]', function () {
            submitUpdate($(this).closest('form'));
        });

        // Remove item.
        $('#cart-content').on('click', '.cart-remove', function () {
            var $row = $(this).closest('tr');
            var productId = parseInt($row.data('product-id'), 10);
            var variantRaw = $row.data('variant-id');
            var variantId = variantRaw === '' ? null : parseInt(variantRaw, 10);
            legacyApi.removeCartItem(productId, variantId)
                .done(reload)
                .fail(function (err) { legacyLayout.alert('danger', err.message); });
        });

        // Clear cart (jQuery UI confirm dialog, like the legacy pattern).
        $('#cart-content').on('click', '#cart-clear', function () {
            var $dlg = $('<div title="Clear cart?"><p>Remove all items from your cart?</p></div>');
            $dlg.dialog({
                modal: true,
                buttons: {
                    'Clear cart': function () {
                        $dlg.dialog('close');
                        legacyApi.clearCart().done(reload)
                            .fail(function (err) { legacyLayout.alert('danger', err.message); });
                    },
                    Cancel: function () { $dlg.dialog('close'); }
                },
                close: function () { $dlg.remove(); }
            });
        });
    });
})(jQuery);
