/* Legacy eCommerce storefront - authentication (jQuery).
 * Login/register post to the .NET 8 auth API; the backend owns Identity,
 * cookies and lockout policy. jQuery Unobtrusive Validation handles
 * client-side rules; server errors render into the validation summary.
 */
(function ($) {
    'use strict';

    function returnUrl() {
        var m = window.location.search.match(/[?&]returnUrl=([^&]*)/);
        return m ? decodeURIComponent(m[1]) : 'index.html';
    }

    $(function () {
        // Already logged in -> no need for these pages.
        legacyApi.me().done(function () {
            window.location.href = returnUrl();
        }).fail(function () { /* anonymous: stay */ });

        var $login = $('#login-form');
        if ($login.length) {
            $.validator.unobtrusive.parse($login);
            $login.on('submit', function (e) {
                e.preventDefault();
                if (!$login.valid()) return;
                var $btn = $login.find('button[type="submit"]').prop('disabled', true);
                legacyApi.login($('#Email').val(), $('#Password').val())
                    .done(function () { window.location.href = returnUrl(); })
                    .fail(function (err) { legacyLayout.showErrors($('#login-errors'), err); })
                    .always(function () { $btn.prop('disabled', false); });
            });
        }

        var $reg = $('#register-form');
        if ($reg.length) {
            $.validator.unobtrusive.parse($reg);
            $reg.on('submit', function (e) {
                e.preventDefault();
                if (!$reg.valid()) return;
                var $btn = $reg.find('button[type="submit"]').prop('disabled', true);
                legacyApi.register({
                    firstName: $('#FirstName').val(),
                    lastName: $('#LastName').val(),
                    email: $('#Email').val(),
                    password: $('#Password').val(),
                    confirmPassword: $('#ConfirmPassword').val()
                })
                    .done(function () { window.location.href = returnUrl(); })
                    .fail(function (err) { legacyLayout.showErrors($('#register-errors'), err); })
                    .always(function () { $btn.prop('disabled', false); });
            });
        }
    });
})(jQuery);
