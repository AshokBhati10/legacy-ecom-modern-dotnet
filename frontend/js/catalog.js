/* Legacy eCommerce storefront - product catalog (jQuery).
 * Mirrors the legacy Product/Index flow: filter form GETs the API and
 * replaces #product-list, AJAX paging, lazy category tree.
 */
(function ($) {
    'use strict';

    var PAGE_SIZE = 9;
    var state = { q: '', categoryId: null, page: 1 };

    function readQuery() {
        var params = new URLSearchParams(window.location.search);
        state.q = params.get('q') || '';
        var cat = params.get('categoryId');
        state.categoryId = cat ? parseInt(cat, 10) : null;
        state.page = parseInt(params.get('page') || '1', 10) || 1;
        $('#filter-q').val(state.q);
    }

    function loadProducts() {
        var $list = $('#product-list');
        $list.fadeTo(200, 0.4);
        legacyApi.getProducts({
            q: state.q || undefined,
            categoryId: state.categoryId || undefined,
            page: state.page,
            pageSize: PAGE_SIZE
        }).done(function (result) {
            $list.empty().fadeTo(200, 1);
            var items = result.items || result.products || [];
            $('#product-count').text(result.totalCount + ' product(s)' +
                (state.q ? ' for "' + state.q + '"' : ''));
            if (!items.length) {
                $list.html('<div class="col-xs-12"><div class="alert alert-info">' +
                    'No products found. Try a different search or category.</div></div>');
            } else {
                $.each(items, function (i, p) { $list.append(legacyLayout.productCard(p)); });
                legacyLayout.bindAddToCart($list.find('.add-to-cart-form'));
            }
            renderPager(result.totalPages || 1, result.page || 1);
            markActiveCategory();
        }).fail(function () {
            $list.fadeTo(200, 1).html(
                '<div class="col-xs-12"><p class="text-danger">Could not load products. Please try again.</p></div>');
        });
    }

    function renderPager(totalPages, page) {
        var $pager = $('#product-pager').empty();
        if (totalPages <= 1) return;
        for (var p = 1; p <= totalPages; p++) {
            $pager.append('<li class="' + (p === page ? 'active' : '') + '">' +
                '<a href="#" data-page="' + p + '">' + p + '</a></li>');
        }
    }

    function markActiveCategory() {
        $('#category-tree li').removeClass('active');
        var sel = state.categoryId
            ? '#category-tree a[data-cat-id="' + state.categoryId + '"]'
            : '#category-tree .cat-all a';
        $(sel).closest('li').addClass('active');
    }

    function categoryNode(cat) {
        var toggle = cat.hasChildren
            ? '<a href="#" class="cat-toggle" data-cat-id="' + cat.id + '" title="Expand">' +
              '<span class="glyphicon glyphicon-plus"></span></a> '
            : '';
        return '<li>' + toggle +
            '<a href="#" data-cat-id="' + cat.id + '">' + legacyApi.esc(cat.name) + '</a>' +
            '<ul class="nav cat-children" data-parent="' + cat.id + '" data-loaded="false" style="display:none;margin-left:15px"></ul>' +
            '</li>';
    }

    function loadTree() {
        legacyApi.getCategoryTree()
            .done(function (cats) {
                var $tree = $('#category-tree');
                $.each(cats, function (i, c) { $tree.append(categoryNode(c)); });
                markActiveCategory();
            });
    }

    $(function () {
        readQuery();
        loadTree();
        loadProducts();

        // Filter form -> reload list without a full page refresh.
        $('#product-filter-form').on('submit', function (e) {
            e.preventDefault();
            state.q = $('#filter-q').val();
            state.page = 1;
            loadProducts();
        });
        $('#filter-clear').on('click', function () {
            $('#filter-q').val('');
            state.q = '';
            state.categoryId = null;
            state.page = 1;
            loadProducts();
        });

        // AJAX paging inside the product list.
        $('#product-pager').on('click', 'a', function (e) {
            e.preventDefault();
            state.page = parseInt($(this).data('page'), 10);
            loadProducts();
            $(window).scrollTop($('#product-list').offset().top - 80);
        });

        // Category selection.
        $('#category-tree').on('click', 'a[data-cat-id]', function (e) {
            if ($(this).hasClass('cat-toggle')) return;
            e.preventDefault();
            var id = $(this).data('cat-id');
            state.categoryId = id === '' ? null : parseInt(id, 10);
            state.page = 1;
            loadProducts();
        });

        // Category tree: lazy-load child categories via AJAX.
        $('#category-tree').on('click', '.cat-toggle', function (e) {
            e.preventDefault();
            var $toggle = $(this);
            var id = $toggle.data('cat-id');
            var $box = $('.cat-children[data-parent="' + id + '"]');
            var $icon = $toggle.find('.glyphicon');
            var expand = function () {
                $box.slideDown(150);
                $icon.removeClass('glyphicon-plus').addClass('glyphicon-minus');
            };
            if ($box.is(':visible')) {
                $box.slideUp(150);
                $icon.removeClass('glyphicon-minus').addClass('glyphicon-plus');
                return;
            }
            if ($box.data('loaded')) { expand(); return; }
            legacyApi.getCategoryTree(id).done(function (cats) {
                $.each(cats, function (i, c) { $box.append(categoryNode(c)); });
                $box.data('loaded', true);
                expand();
            });
        });
    });
})(jQuery);
