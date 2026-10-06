/* Legacy eCommerce storefront - product detail (jQuery + Fancybox 3). */
(function ($) {
    'use strict';

    function priceHtml(p) {
        if (p.salePrice && p.salePrice > 0) {
            return '<span class="text-muted"><s>' + legacyApi.money(p.price) + '</s></span> ' +
                '<span class="text-danger">' + legacyApi.money(p.salePrice) + '</span> ' +
                '<span class="label label-danger">SALE</span>';
        }
        return legacyApi.money(p.price);
    }

    function imageOrPlaceholder(url, alt, cls) {
        if (url) {
            return '<img src="' + legacyApi.esc(url) + '" alt="' + legacyApi.esc(alt) + '" class="img-responsive img-thumbnail ' + (cls || '') + '" onerror="this.onerror=null;this.src=\'images/no-image.png\'" />';
        }
        return '<img src="images/no-image.png" alt="' + legacyApi.esc(alt) + '" class="img-responsive img-thumbnail ' + (cls || '') + '" />';
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
                    gallery = '<a data-fancybox="gallery" href="' + legacyApi.esc(main.url) + '" data-caption="' + legacyApi.esc(p.name) + '">' +
                        imageOrPlaceholder(main.url, p.name) + '</a>';
                    if (images.length > 1) {
                        gallery += '<div class="row product-thumbs">';
                        $.each(images, function (i, img) {
                            gallery += '<div class="col-xs-3">' +
                                '<a data-fancybox="gallery" href="' + legacyApi.esc(img.url) + '" data-caption="' + legacyApi.esc(p.name) + '">' +
                                imageOrPlaceholder(img.url, img.altText || p.name) + '</a></div>';
                        });
                        gallery += '</div>';
                    }
                } else {
                    gallery = imageOrPlaceholder(p.thumbnailUrl, p.name);
                }

                var variants = '';
                if (p.variants && p.variants.length) {
                    variants = '<div class="form-group"><label for="variantId">Option</label><select id="variantId" name="variantId" class="form-control">';
                    $.each(p.variants, function (i, v) {
                        var adj = v.priceAdjustment
                            ? ' (' + (v.priceAdjustment > 0 ? '+' : '') + legacyApi.money(v.priceAdjustment) + ')'
                            : '';
                        variants += '<option value="' + v.id + '">' + legacyApi.esc(v.name) + legacyApi.esc(adj) + '</option>';
                    });
                    variants += '</select></div>';
                }

                var stock = p.stockQuantity > 0
                    ? '<p class="text-muted">Availability: ' + p.stockQuantity + ' in stock</p>'
                    : '<p class="text-danger">Out of stock</p>';

                $('#product-detail').html(
                    '<h2>' + legacyApi.esc(p.name) + '</h2>' +
                    '<div class="row">' +
                    '  <div class="col-md-6">' + gallery + '</div>' +
                    '  <div class="col-md-6">' +
                    '    <p class="text-muted">SKU: ' + legacyApi.esc(p.sku) + (p.categoryName ? ' | Category: ' + legacyApi.esc(p.categoryName) : '') + '</p>' +
                    '    <h3 class="product-price">' + priceHtml(p) + '</h3>' +
                    '    <p>' + legacyApi.esc(p.shortDescription || '') + '</p>' +
                    '    <form id="detail-add-form" class="form-inline" method="post">' +
                    '      <input type="hidden" name="productId" value="' + p.id + '" />' +
                    '      <div class="form-group">' + variants + '</div> ' +
                    '      <div class="form-group"><label for="quantity">Qty</label> ' +
                    '        <input type="number" id="quantity" name="quantity" value="1" min="1" max="99" class="form-control" style="width:80px" /></div> ' +
                    '      <button type="submit" class="btn btn-primary"' + (p.stockQuantity > 0 ? '' : ' disabled') + '>Add to Cart</button>' +
                    '    </form>' +
                    '    <h4>Description</h4>' +
                    '    <p>' + legacyApi.esc(p.description || '') + '</p>' +
                    stock +
                    '  </div>' +
                    '</div>');

                legacyLayout.bindAddToCart($('#detail-add-form'));

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
