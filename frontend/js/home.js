/* Legacy eCommerce storefront - home page (jQuery). */
(function ($) {
    'use strict';

    // Reusable product card, mirroring _ProductCard.cshtml.
    function productCard(p) {
        var price;
        var badge = '';
        if (p.salePrice && p.salePrice > 0) {
            price = '<span class="price-was">' + legacyApi.money(p.price) + '</span>' +
                '<span class="price-sale">' + legacyApi.money(p.salePrice) + '</span>';
            badge = '<span class="badge-sale">Sale</span>';
        } else {
            price = '<span class="price-now">' + legacyApi.money(p.effectivePrice || p.price) + '</span>';
        }
        var imgSrc = legacyApi.imageUrl(p.thumbnailUrl);
        return '' +
            '<div class="col-xs-6 col-sm-6 col-md-4 col-lg-3 product-card">' +
            '  <div class="card">' +
            '    <a class="product-media" href="product.html?id=' + p.id + '">' + badge +
            '      <img src="' + legacyApi.esc(imgSrc) + '" alt="' + legacyApi.esc(p.name) + '" loading="lazy" onerror="legacyApi.imageFallback(this)" />' +
            '    </a>' +
            '    <div class="product-body">' +
            '      <h4 class="product-name"><a href="product.html?id=' + p.id + '">' + legacyApi.esc(p.name) + '</a></h4>' +
            '      <p class="product-price">' + price + '</p>' +
            '      <div class="product-actions">' +
            '        <form class="add-to-cart-form" method="post" style="display:contents">' +
            '          <input type="hidden" name="productId" value="' + p.id + '" />' +
            '          <input type="hidden" name="quantity" value="1" />' +
            '          <button type="submit" class="btn btn-primary btn-sm">Add to Cart</button>' +
            '        </form>' +
            '        <a href="product.html?id=' + p.id + '" class="btn btn-default btn-sm">Details</a>' +
            '      </div>' +
            '    </div>' +
            '  </div>' +
            '</div>';
    }

    legacyLayout.productCard = productCard;

    // Category metadata for richer cards (icons + taglines).
    var CATEGORY_META = {
        'Electronics': { icon: 'glyphicon-phone', tag: 'Laptops, phones & more' },
        'Audio': { icon: 'glyphicon-headphones', tag: 'Headphones & speakers' },
        'Home & Kitchen': { icon: 'glyphicon-home', tag: 'Cookware & appliances' },
        'Sports & Outdoors': { icon: 'glyphicon-flag', tag: 'Fitness & training' },
        'Books': { icon: 'glyphicon-book', tag: 'Classics & collections' },
        'Clothing': { icon: 'glyphicon-tags', tag: 'Everyday apparel' }
    };

    $(function () {
        // Categories -> cards.
        legacyApi.getCategories()
            .done(function (cats) {
                var $row = $('#home-categories').empty();
                $.each(cats.slice(0, 6), function (i, c) {
                    var meta = CATEGORY_META[c.name] || { icon: 'glyphicon-th-large', tag: 'Shop the collection' };
                    $row.append(
                        '<div class="col-xs-6 col-sm-4 col-md-2">' +
                        '  <a class="category-card" href="catalog.html?categoryId=' + c.id + '">' +
                        '    <span class="cat-icon"><span class="glyphicon ' + meta.icon + '"></span></span>' +
                        '    <h4>' + legacyApi.esc(c.name) + '</h4>' +
                        '    <p>' + legacyApi.esc(meta.tag) + '</p>' +
                        '    <span class="cat-cta">Shop now &rarr;</span>' +
                        '  </a>' +
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
