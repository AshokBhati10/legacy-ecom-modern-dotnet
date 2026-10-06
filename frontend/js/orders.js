/* Legacy eCommerce storefront - orders (jQuery + DataTables).
 * Order history uses DataTables, mirroring the legacy Account/Orders view.
 */
(function ($) {
    'use strict';

    $(function () {
        var $list = $('#orders-content');
        var $detail = $('#order-detail');

        if ($list.length) {
            legacyLayout.requireAuth().done(function () {
                legacyApi.getOrders()
                    .done(function (orders) {
                        if (!orders.length) {
                            $list.html('<div class="alert alert-info">You have no orders yet. <a href="catalog.html">Start shopping</a>.</div>');
                            return;
                        }
                        var html = '<table id="orders-table" class="table table-striped table-bordered"><thead><tr>' +
                            '<th>Order Number</th><th>Date</th><th>Status</th><th>Items</th><th>Total</th><th></th></tr></thead><tbody>';
                        $.each(orders, function (i, o) {
                            html += '<tr><td>' + legacyApi.esc(o.orderNumber) + '</td>' +
                                '<td>' + legacyApi.esc(legacyApi.formatDate(o.orderDate)) + '</td>' +
                                '<td>' + legacyApi.esc(o.status) + '</td>' +
                                '<td>' + o.itemCount + '</td>' +
                                '<td>' + legacyApi.money(o.total) + '</td>' +
                                '<td><a href="order-detail.html?id=' + o.orderId + '" class="btn btn-default btn-sm">Details</a></td></tr>';
                        });
                        html += '</tbody></table>';
                        $list.html(html);
                        $('#orders-table').DataTable({
                            order: [[1, 'desc']],
                            pageLength: 10
                        });
                    })
                    .fail(function (err) {
                        if (err && err.status === 401) return; // requireAuth already redirected
                        $list.html('<p class="text-danger">Could not load your orders.</p>');
                    });
            });
        }

        if ($detail.length) {
            var id = new URLSearchParams(window.location.search).get('id');
            if (!id) {
                $detail.html('<div class="alert alert-danger">No order selected.</div>');
                return;
            }
            legacyLayout.requireAuth().done(function () {
                legacyApi.getOrder(id)
                    .done(function (o) {
                        $('#crumb-order').text(o.orderNumber);
                        var html = '<h2>Order ' + legacyApi.esc(o.orderNumber) + '</h2>' +
                            '<p class="text-muted">Placed ' + legacyApi.esc(legacyApi.formatDate(o.orderDate)) +
                            ' &middot; Status: ' + legacyApi.esc(o.status) +
                            ' &middot; Payment: ' + legacyApi.esc(o.paymentMethod) +
                            ' &middot; Shipping: ' + legacyApi.esc(o.shippingMethod) + '</p>' +
                            '<div class="row"><div class="col-md-7">' +
                            '<table class="table table-striped table-bordered"><thead><tr>' +
                            '<th>Product</th><th>Unit Price</th><th>Qty</th><th>Line Total</th></tr></thead><tbody>';
                        $.each(o.lines, function (i, l) {
                            html += '<tr><td>' + legacyApi.esc(l.productName) +
                                (l.variantName ? '<br /><small class="text-muted">' + legacyApi.esc(l.variantName) + '</small>' : '') + '</td>' +
                                '<td>' + legacyApi.money(l.unitPrice) + '</td>' +
                                '<td>' + l.quantity + '</td>' +
                                '<td>' + legacyApi.money(l.lineTotal) + '</td></tr>';
                        });
                        html += '</tbody><tfoot>' +
                            '<tr><td colspan="3" class="text-right">Subtotal:</td><td>' + legacyApi.money(o.subTotal) + '</td></tr>' +
                            '<tr><td colspan="3" class="text-right">Shipping:</td><td>' + legacyApi.money(o.shippingCost) + '</td></tr>' +
                            '<tr><td colspan="3" class="text-right">Tax:</td><td>' + legacyApi.money(o.taxAmount) + '</td></tr>' +
                            '<tr><td colspan="3" class="text-right"><strong>Total:</strong></td><td><strong>' + legacyApi.money(o.total) + '</strong></td></tr>' +
                            '</tfoot></table></div>' +
                            '<div class="col-md-5"><div class="panel panel-default">' +
                            '<div class="panel-heading"><h4 class="panel-title">Shipping address</h4></div>' +
                            '<div class="panel-body"><address>' +
                            legacyApi.esc(o.shipFirstName) + ' ' + legacyApi.esc(o.shipLastName) + '<br />' +
                            legacyApi.esc(o.shipStreet) + '<br />' +
                            legacyApi.esc(o.shipCity) + ', ' + legacyApi.esc(o.shipState) + ' ' + legacyApi.esc(o.shipPostalCode) + '<br />' +
                            legacyApi.esc(o.shipCountry) + '<br />' +
                            legacyApi.esc(o.shipEmail) +
                            '</address></div></div></div></div>';
                        $detail.html(html);
                    })
                    .fail(function (err) {
                        if (err && err.status === 401) return;
                        $detail.html('<div class="alert alert-danger">Order not found.</div>');
                    });
            });
        }
    });
})(jQuery);
