/* Legacy eCommerce storefront - product detail (jQuery + Fancybox 3). */
(function ($) {
    'use strict';

    function priceHtml(p) {
        if (p.salePrice && p.salePrice > 0) {
            return '<span class="price-was">' + legacyApi.money(p.price) + '</span> ' +
                '<span class="price-sale">' + legacyApi.money(p.salePrice) + '</span> ' +
                '<span class="badge-sale" style="position:static">Sale</span>';
        }
        return '<span class="price-now">' + legacyApi.money(p.effectivePrice || p.price) + '</span>';
    }

    function imageOrPlaceholder(url, alt, cls) {
        var src = legacyApi.imageUrl(url);
        return '<img src="' + legacyApi.esc(src) + '" alt="' + legacyApi.esc(alt) + '" onerror="legacyApi.imageFallback(this)" class="' + (cls || '') + '" />';
    }

    $(function () {
        var id = new URLSearchParams(window.location.search).get('id');
        if (!id) {
            $('#product-detail').html('<div class="alert alert-danger">No product selected.</div>');
            return;
        }

        legacyApi.getProduct(id)
            .done(function (p) {
                document.title = p.name + ' - Northwind Market';
                $('#crumb-name').text(p.name);

                var images = p.images || [];
                var main = images.filter(function (i) { return i.isMain; })[0] || images[0];

                var gallery = '';
                if (images.length) {
                    gallery = '<div class="detail-gallery"><div class="main-image">' +
                        '<a data-fancybox="gallery" href="' + legacyApi.esc(legacyApi.imageUrl(main.url)) + '" data-caption="' + legacyApi.esc(p.name) + '">' +
                        imageOrPlaceholder(main.url, p.name) + '</a></div>';
                    if (images.length > 1) {
                        gallery += '<div class="row detail-thumbs">';
                        $.each(images, function (i, img) {
                            gallery += '<div class="col-xs-3">' +
                                '<a data-fancybox="gallery" href="' + legacyApi.esc(legacyApi.imageUrl(img.url)) + '" data-caption="' + legacyApi.esc(p.name) + '" data-thumb="' + i + '">' +
                                imageOrPlaceholder(img.url, img.altText || p.name) + '</a></div>';
                        });
                        gallery += '</div></div>';
                    } else {
                        gallery += '</div>';
                    }
                } else {
                    gallery = '<div class="detail-gallery"><div class="main-image">' + imageOrPlaceholder(p.thumbnailUrl, p.name) + '</div></div>';
                }

                var variants = '';
                if (p.variants && p.variants.length) {
                    variants = '<div class="form-group"><label>Option</label><div class="variant-pills" role="radiogroup" aria-label="Product variant">';
                    $.each(p.variants, function (i, v) {
                        var adj = v.priceAdjustment
                            ? ' (' + (v.priceAdjustment > 0 ? '+' : '') + legacyApi.money(v.priceAdjustment) + ')'
                            : '';
                        variants += '<label class="' + (i === 0 ? 'selected' : '') + '">' +
                            '<input type="radio" name="variantId" value="' + v.id + '"' + (i === 0 ? ' checked="checked"' : '') + ' />' +
                            legacyApi.esc(v.name) + legacyApi.esc(adj) + '</label>';
                    });
                    variants += '</div></div>';
                }

                var stock = p.stockQuantity > 0
                    ? '<p class="stock-note"><span class="glyphicon glyphicon-ok-circle"></span>' + p.stockQuantity + ' in stock — ready to ship</p>'
                    : '<p class="text-danger"><span class="glyphicon glyphicon-remove-circle"></span>Out of stock</p>';

                $('#product-detail').html(
                    '<div class="row">' +
                    '  <div class="col-md-6">' + gallery + '</div>' +
                    '  <div class="col-md-6 detail-info">' +
                    '    <h1>' + legacyApi.esc(p.name) + '</h1>' +
                    '    <p class="detail-meta">SKU: ' + legacyApi.esc(p.sku) + (p.categoryName ? ' &nbsp;·&nbsp; ' + legacyApi.esc(p.categoryName) : '') + '</p>' +
                    '    <p class="detail-price">' + priceHtml(p) + '</p>' +
                    '    <p>' + legacyApi.esc(p.shortDescription || '') + '</p>' +
                    '    <form id="detail-add-form" method="post">' +
                    '      <input type="hidden" name="productId" value="' + p.id + '" />' +
                    variants +
                    '      <div class="form-group"><label>Quantity</label><br />' +
                    '        <span class="qty-stepper">' +
                    '          <button type="button" class="btn btn-default qty-minus" aria-label="Decrease quantity">&minus;</button>' +
                    '          <input type="number" id="quantity" name="quantity" value="1" min="1" max="99" aria-label="Quantity" />' +
                    '          <button type="button" class="btn btn-default qty-plus" aria-label="Increase quantity">+</button>' +
                    '        </span></div>' +
                    '      <div class="form-group">' +
                    '        <button type="submit" class="btn btn-primary btn-lg"' + (p.stockQuantity > 0 ? '' : ' disabled') + '>' +
                    '          <span class="glyphicon glyphicon-shopping-cart"></span> Add to Cart</button>' +
                    '      </div>' +
                    '    </form>' +
                    stock +
                    '    <hr />' +
                    '    <h4>Description</h4>' +
                    '    <p>' + legacyApi.esc(p.description || '') + '</p>' +
                    '  </div>' +
                    '</div>');

                legacyLayout.bindAddToCart($('#detail-add-form'));

                // Variant pills: visual selected state.
                $('#product-detail').on('change', '.variant-pills input', function () {
                    $(this).closest('.variant-pills').find('label').removeClass('selected');
                    $(this).closest('label').addClass('selected');
                });

                // Quantity stepper.
                $('#product-detail').on('click', '.qty-minus', function () {
                    var $q = $('#quantity');
                    $q.val(Math.max(1, parseInt($q.val(), 10) - 1 || 1));
                });
                $('#product-detail').on('click', '.qty-plus', function () {
                    var $q = $('#quantity');
                    $q.val(Math.min(99, (parseInt($q.val(), 10) || 1) + 1));
                });

                // Thumbnail click swaps the main image; Fancybox still opens the lightbox.
                $('#product-detail').on('click', '.detail-thumbs a', function (e) {
                    e.preventDefault();
                    var $a = $(this);
                    $('#product-detail .detail-thumbs a').removeClass('active');
                    $a.addClass('active');
                    var $main = $('#product-detail .detail-gallery .main-image');
                    $main.find('a').attr('href', $a.attr('href'));
                    $main.find('img').attr('src', $a.find('img').attr('src'));
                });
                $('#product-detail .detail-thumbs a').first().addClass('active');

                // Fancybox 3 gallery.
                $('[data-fancybox="gallery"]').fancybox({
                    buttons: ['close'],
                    loop: true
                });

                // Related products.
                var $rel = $('#related-products').empty();
                $.each(p.relatedProducts || [], function (i, r) { $rel.append(legacyLayout.productCard(r)); });
                legacyLayout.bindAddToCart($rel.find('.add-to-cart-form'));
            })
            .fail(function () {
                $('#product-detail').html('<div class="alert alert-danger">Product not found.</div>');
            });
    });
})(jQuery);
