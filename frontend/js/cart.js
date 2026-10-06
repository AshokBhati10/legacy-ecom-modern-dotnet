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
        var html = '<table class="table table-striped table-bordered"><thead><tr>' +
            '<th>Product</th><th>Unit Price</th><th>Quantity</th><th>Line Total</th><th></th></tr></thead><tbody>';
        $.each(cart.items, function (i, item) {
            html += '<tr data-product-id="' + item.productId + '" data-variant-id="' + (item.variantId || '') + '">' +
                '<td><a href="product.html?id=' + item.productId + '">' + legacyApi.esc(item.productName) + '</a>' +
                (item.variantName ? '<br /><small class="text-muted">' + legacyApi.esc(item.variantName) + '</small>' : '') + '</td>' +
                '<td>' + legacyApi.money(item.unitPrice) + '</td>' +
                '<td><form class="form-inline cart-update-form">' +
                '<input type="number" name="quantity" value="' + item.quantity + '" min="0" max="99" class="form-control input-sm" style="width:70px" /> ' +
                '<button type="submit" class="btn btn-default btn-sm">Update</button></form></td>' +
                '<td>' + legacyApi.money(item.lineTotal) + '</td>' +
                '<td><button type="button" class="btn btn-danger btn-sm cart-remove">Remove</button></td>' +
                '</tr>';
        });
        html += '</tbody><tfoot><tr><td colspan="3" class="text-right"><strong>Subtotal (' +
            cart.itemCount + ' item(s)):</strong></td><td colspan="2"><strong>' +
            legacyApi.money(cart.subTotal) + '</strong></td></tr></tfoot></table>';
        html += '<div class="row"><div class="col-md-6">' +
            '<button type="button" class="btn btn-warning" id="cart-clear">Clear Cart</button></div>' +
            '<div class="col-md-6 text-right">' +
            '<a href="catalog.html" class="btn btn-default">Continue Shopping</a> ' +
            '<a href="checkout.html" class="btn btn-success">Proceed to Checkout</a></div></div>';
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

        // Update quantity (AJAX, no full-page reload).
        $('#cart-content').on('submit', '.cart-update-form', function (e) {
            e.preventDefault();
            var $row = $(this).closest('tr');
            var productId = parseInt($row.data('product-id'), 10);
            var variantRaw = $row.data('variant-id');
            var variantId = variantRaw === '' ? null : parseInt(variantRaw, 10);
            var quantity = parseInt($(this).find('[name="quantity"]').val(), 10) || 0;
            legacyApi.updateCartItem(productId, variantId, quantity)
                .done(reload)
                .fail(function (err) { legacyLayout.alert('danger', err.message); });
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
