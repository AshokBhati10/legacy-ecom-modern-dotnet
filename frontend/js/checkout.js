/* Legacy eCommerce storefront - checkout wizard (jQuery).
 * Address -> Shipping -> Payment -> Confirmation, mirroring the legacy
 * Checkout/Address|Shipping|Payment|Confirmation pages as wizard steps.
 * jQuery Validate + Unobtrusive Validation on the address form;
 * the backend enforces all business rules and step ordering.
 */
(function ($) {
    'use strict';

    var shippingMethod = null;

    function goStep(name) {
        var order = ['address', 'shipping', 'payment', 'confirm'];
        var idx = order.indexOf(name);
        $('.checkout-step').addClass('hidden');
        $('#step-' + name).removeClass('hidden');
        $('#checkout-steps li').each(function (i) {
            $(this).toggleClass('active', i === idx).toggleClass('done', i < idx);
        });
        $(window).scrollTop(0);
    }

    function fail(err) {
        legacyLayout.showErrors($('#checkout-errors'), err);
    }

    function summaryHtml(s) {
        var html = '<p>Items (' + s.itemCount + '): ' + legacyApi.money(s.subTotal) + '</p>' +
            '<p>Shipping' + (s.shippingMethod ? ' (' + legacyApi.esc(s.shippingMethod) + ')' : '') + ': ' +
            legacyApi.money(s.shippingCost) + '</p>' +
            '<p>Tax: ' + legacyApi.money(s.taxAmount) + '</p><hr />' +
            '<p><strong>Total: ' + legacyApi.money(s.total) + '</strong></p>';
        return html;
    }

    function paymentSummaryHtml(s) {
        var html = '';
        $.each(s.items || [], function (i, item) {
            html += '<p>' + item.quantity + ' x ' + legacyApi.esc(item.productName) +
                '<span class="pull-right">' + legacyApi.money(item.lineTotal) + '</span></p>';
        });
        html += '<hr /><p>Subtotal: <span class="pull-right">' + legacyApi.money(s.subTotal) + '</span></p>' +
            '<p>Shipping (' + legacyApi.esc(s.shippingMethod) + '): <span class="pull-right">' +
            legacyApi.money(s.shippingCost) + '</span></p>' +
            '<p>Tax: <span class="pull-right">' + legacyApi.money(s.taxAmount) + '</span></p>' +
            '<p><strong>Total: <span class="pull-right">' + legacyApi.money(s.total) + '</span></strong></p>';
        return html;
    }

    function loadShippingStep() {
        goStep('shipping');
        legacyApi.getShippingOptions()
            .done(function (result) {
                var options = result.options || [];
                var $box = $('#shipping-options').empty();
                if (!options.length) {
                    $box.html('<p class="text-danger">No shipping options available.</p>');
                    return;
                }
                $.each(options, function (i, o) {
                    $box.append('<label class="shipping-option' + (i === 0 ? ' selected' : '') + '">' +
                        '<input type="radio" name="ShippingMethod" value="' + legacyApi.esc(o.code) + '"' +
                        (i === 0 ? ' checked="checked"' : '') + ' class="shipping-option-input" /> ' +
                        '<strong>' + legacyApi.esc(o.name) + '</strong>' +
                        '<span class="pull-right"><strong>' + legacyApi.money(o.cost) + '</strong></span>' +
                        '<br /><small class="text-muted">' + legacyApi.esc(o.description) + '</small>' +
                        '</label>');
                });
                refreshSummary();
            })
            .fail(fail);
    }

    function refreshSummary() {
        var method = $('input[name="ShippingMethod"]:checked').val();
        legacyApi.getCheckoutSummary(method)
            .done(function (s) {
                shippingMethod = s.shippingMethod;
                $('#shipping-summary').html(summaryHtml(s));
            })
            .fail(fail);
    }

    function loadPaymentStep() {
        goStep('payment');
        legacyApi.getCheckoutSummary(shippingMethod)
            .done(function (s) { $('#payment-summary').html(paymentSummaryHtml(s)); })
            .fail(fail);
    }

    $(function () {
        // Empty cart -> back to cart (legacy redirected too).
        legacyApi.getCart().done(function (cart) {
            if (!cart.items || !cart.items.length) {
                window.location.href = 'cart.html';
            }
        });

        // Prefill address for logged-in users.
        legacyApi.getCheckoutAddress().done(function (a) {
            if (!a) return;
            $.each(['Email', 'FirstName', 'LastName', 'Phone', 'Street', 'City', 'State', 'PostalCode', 'Country'], function (i, f) {
                var key = f.charAt(0).toLowerCase() + f.slice(1);
                if (a[key]) $('#' + f).val(a[key]);
            });
        });

        // jQuery Unobtrusive Validation parses data-val-* attributes.
        $.validator.unobtrusive.parse('#address-form');

        $('#address-form').on('submit', function (e) {
            e.preventDefault();
            if (!$(this).valid()) return;
            var address = {
                email: $('#Email').val(),
                firstName: $('#FirstName').val(),
                lastName: $('#LastName').val(),
                phone: $('#Phone').val() || null,
                street: $('#Street').val(),
                city: $('#City').val(),
                state: $('#State').val(),
                postalCode: $('#PostalCode').val(),
                country: $('#Country').val()
            };
            legacyApi.saveCheckoutAddress(address)
                .done(loadShippingStep)
                .fail(fail);
        });

        // Recompute the summary when the shipping method changes.
        $('#shipping-options').on('change', '.shipping-option-input', function () {
            $('#shipping-options .shipping-option').removeClass('selected');
            $(this).closest('.shipping-option').addClass('selected');
            refreshSummary();
        });

        $('#shipping-form').on('submit', function (e) {
            e.preventDefault();
            var method = $('input[name="ShippingMethod"]:checked').val();
            if (!method) {
                legacyLayout.alert('warning', 'Please choose a shipping method.');
                return;
            }
            legacyApi.setShippingMethod(method)
                .done(function () { shippingMethod = method; loadPaymentStep(); })
                .fail(fail);
        });

        // Expiry dropdowns (legacy built these server-side; here via jQuery).
        for (var m = 1; m <= 12; m++) {
            var mm = (m < 10 ? '0' : '') + m;
            $('#ExpiryMonth').append('<option value="' + m + '">' + mm + '</option>');
        }
        var year = new Date().getFullYear();
        for (var y = year; y < year + 15; y++) {
            $('#ExpiryYear').append('<option value="' + y + '">' + y + '</option>');
        }

        // Toggle card fields by payment method (legacy Payment.cshtml pattern).
        $('input[name="PaymentMethod"]').on('change', function () {
            $('#card-fields').toggle($('input[name="PaymentMethod"]:checked').val() === 'Card');
        });

        // Card field validation rules (only when Card is selected).
        $('#payment-form').validate({
            rules: {
                CardholderName: { required: function () { return $('input[name="PaymentMethod"]:checked').val() === 'Card'; } },
                CardNumber: { required: function () { return $('input[name="PaymentMethod"]:checked').val() === 'Card'; }, digits: true, minlength: 13, maxlength: 19 },
                ExpiryMonth: { required: function () { return $('input[name="PaymentMethod"]:checked').val() === 'Card'; } },
                ExpiryYear: { required: function () { return $('input[name="PaymentMethod"]:checked').val() === 'Card'; } },
                Cvv: { required: function () { return $('input[name="PaymentMethod"]:checked').val() === 'Card'; }, digits: true, minlength: 3, maxlength: 4 }
            },
            errorClass: 'text-danger'
        });

        $('#payment-form').on('submit', function (e) {
            e.preventDefault();
            if (!$(this).valid()) return;
            var method = $('input[name="PaymentMethod"]:checked').val();
            var payment = { paymentMethod: method };
            if (method === 'Card') {
                payment.cardholderName = $('#CardholderName').val();
                payment.cardNumber = $('#CardNumber').val();
                payment.expiryMonth = parseInt($('#ExpiryMonth').val(), 10);
                payment.expiryYear = parseInt($('#ExpiryYear').val(), 10);
                payment.cvv = $('#Cvv').val();
            }
            var $btn = $(this).find('button[type="submit"]').prop('disabled', true);
            legacyApi.placeOrder(payment)
                .done(function (order) {
                    $('#confirm-order-number').text(order.orderNumber);
                    goStep('confirm');
                    legacyLayout.refreshMiniCart();
                })
                .fail(fail)
                .always(function () { $btn.prop('disabled', false); });
        });
    });
})(jQuery);
