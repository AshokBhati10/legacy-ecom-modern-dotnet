/* Legacy eCommerce storefront - home page (jQuery). */
(function ($) {
    'use strict';

    // Reusable product card, mirroring _ProductCard.cshtml.
    function productCard(p) {
        var price;
        if (p.salePrice && p.salePrice > 0) {
            price = '<span class="text-muted"><s>' + legacyApi.money(p.price) + '</s></span> ' +
                '<strong class="text-danger">' + legacyApi.money(p.salePrice) + '</strong>';
        } else {
            price = '<strong>' + legacyApi.money(p.price) + '</strong>';
        }
        var img = p.thumbnailUrl
            ? '<img src="' + legacyApi.esc(p.thumbnailUrl) + '" alt="' + legacyApi.esc(p.name) + '" class="img-responsive product-thumb" onerror="this.onerror=null;this.src=\'images/no-image.png\'" />'
            : '<img src="images/no-image.png" alt="' + legacyApi.esc(p.name) + '" class="img-responsive product-thumb" />';
        return '' +
            '<div class="col-sm-6 col-md-4 col-lg-3 product-card">' +
            '  <div class="thumbnail">' +
            '    <a href="product.html?id=' + p.id + '">' + img + '</a>' +
            '    <div class="caption">' +
            '      <h4 class="product-name"><a href="product.html?id=' + p.id + '">' + legacyApi.esc(p.name) + '</a></h4>' +
            '      <p class="product-price">' + price + '</p>' +
            '      <p>' +
            '        <form class="add-to-cart-form" method="post">' +
            '          <input type="hidden" name="productId" value="' + p.id + '" />' +
            '          <input type="hidden" name="quantity" value="1" />' +
            '          <button type="submit" class="btn btn-primary btn-sm">Add to Cart</button> ' +
            '          <a href="product.html?id=' + p.id + '" class="btn btn-default btn-sm">Details</a>' +
            '        </form>' +
            '      </p>' +
            '    </div>' +
            '  </div>' +
            '</div>';
    }

    legacyLayout.productCard = productCard;

    $(function () {
        // Categories -> tiles.
        legacyApi.getCategories()
            .done(function (cats) {
                var $row = $('#home-categories').empty();
                $.each(cats.slice(0, 6), function (i, c) {
                    $row.append(
                        '<div class="col-xs-6 col-sm-4 col-md-2">' +
                        '  <div class="panel panel-default category-tile">' +
                        '    <div class="panel-body">' +
                        '      <h4>' + legacyApi.esc(c.name) + '</h4>' +
                        '      <a href="catalog.html?categoryId=' + c.id + '" class="btn btn-link btn-sm">Shop now &rarr;</a>' +
                        '    </div>' +
                        '  </div>' +
                        '</div>');
                });
            })
            .fail(function () {
                $('#home-categories').html('<div class="col-xs-12"><p class="text-danger">Could not load categories.</p></div>');
            });

        // Featured products -> cards with AJAX add-to-cart.
        legacyApi.getFeatured(8)
            .done(function (products) {
                var $row = $('#home-featured').empty();
                if (!products.length) {
                    $row.html('<div class="col-xs-12"><p class="text-muted">No featured products right now.</p></div>');
                    return;
                }
                $.each(products, function (i, p) { $row.append(productCard(p)); });
                legacyLayout.bindAddToCart($row.find('.add-to-cart-form'));
            })
            .fail(function () {
                $('#home-featured').html('<div class="col-xs-12"><p class="text-danger">Could not load featured products.</p></div>');
            });
    });
})(jQuery);
