import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { catalogApi } from '../api/catalog';
import { useCart } from '../state/CartContext';
import type { ProductDetailDto } from '../types';
import { money } from '../utils/format';
import ProductCard from '../components/ProductCard';
import ProductImage from '../components/ProductImage';

/** Modern equivalent of Product/Detail.cshtml. */
export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { addItem, setDrawerOpen } = useCart();
  const [product, setProduct] = useState<ProductDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);
  const [activeImage, setActiveImage] = useState(0);
  const [lightbox, setLightbox] = useState(false);
  const [variantId, setVariantId] = useState<number | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [adding, setAdding] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setNotFound(false);
    catalogApi
      .detail(Number(id))
      .then((p) => {
        if (cancelled) return;
        setProduct(p);
        setActiveImage(0);
        setVariantId(p.variants.length > 0 ? p.variants[0].id : null);
        setQuantity(1);
      })
      .catch(() => {
        if (!cancelled) setNotFound(true);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [id]);

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') setLightbox(false);
    };
    if (lightbox) window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [lightbox]);

  if (loading) {
    return <div className="mx-auto max-w-7xl px-4 py-16 text-stone-500 sm:px-6">Loading…</div>;
  }
  if (notFound || !product) {
    return (
      <div className="mx-auto max-w-7xl px-4 py-16 text-center sm:px-6">
        <p className="text-lg font-semibold text-stone-800">Product not found</p>
        <Link to="/shop" className="mt-3 inline-block text-sm font-semibold text-emerald-700 hover:underline">
          Back to shop →
        </Link>
      </div>
    );
  }

  const selectedVariant = product.variants.find((v) => v.id === variantId) ?? null;
  const unitPrice = product.effectivePrice + (selectedVariant?.priceAdjustment ?? 0);
  const hasSale = product.salePrice !== null && product.salePrice < product.price;
  const images = [...product.images].sort((a, b) => a.displayOrder - b.displayOrder);
  const mainImage = images[activeImage] ?? images[0];

  const onAdd = async () => {
    setAdding(true);
    setError(null);
    try {
      await addItem(product.id, variantId, quantity);
      setDrawerOpen(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not add to cart.');
    } finally {
      setAdding(false);
    }
  };

  return (
    <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6">
      <nav className="mb-6 text-sm text-stone-500" aria-label="Breadcrumb">
        <Link to="/shop" className="hover:text-emerald-700">Shop</Link>
        <span className="mx-2">/</span>
        <span className="text-stone-800">{product.name}</span>
      </nav>

      <div className="grid gap-10 lg:grid-cols-2">
        {/* Gallery — Fancybox equivalent */}
        <div>
          <button
            type="button"
            onClick={() => setLightbox(true)}
            className="block w-full cursor-zoom-in overflow-hidden rounded-xl border border-stone-200 bg-white"
            aria-label="Enlarge product image"
          >
            {mainImage ? (
              <ProductImage src={mainImage.url} alt={mainImage.altText ?? product.name} className="aspect-square w-full" />
            ) : (
              <ProductImage src={product.thumbnailUrl} alt={product.name} className="aspect-square w-full" />
            )}
          </button>
          {images.length > 1 && (
            <div className="mt-3 flex gap-2 overflow-x-auto">
              {images.map((img, i) => (
                <button
                  key={img.id}
                  type="button"
                  onClick={() => setActiveImage(i)}
                  aria-label={`View image ${i + 1}`}
                  className={`shrink-0 overflow-hidden rounded-lg border-2 ${
                    i === activeImage ? 'border-emerald-600' : 'border-transparent'
                  }`}
                >
                  <ProductImage src={img.url} alt={img.altText ?? product.name} className="h-16 w-16" />
                </button>
              ))}
            </div>
          )}
        </div>

        {/* Info */}
        <div>
          <h1 className="text-3xl font-extrabold tracking-tight text-stone-900">{product.name}</h1>
          {product.shortDescription && (
            <p className="mt-2 text-stone-600">{product.shortDescription}</p>
          )}

          <div className="mt-4 flex items-baseline gap-3">
            <span className={`text-3xl font-extrabold ${hasSale ? 'text-rose-600' : 'text-stone-900'}`}>
              {money(unitPrice)}
            </span>
            {hasSale && <span className="text-lg text-stone-400 line-through">{money(product.price)}</span>}
            {hasSale && (
              <span className="rounded-full bg-rose-100 px-2.5 py-1 text-xs font-bold text-rose-700">SALE</span>
            )}
          </div>
          <p className="mt-1 text-xs text-stone-500">SKU: {selectedVariant?.sku ?? product.sku}</p>

          {product.description && (
            <p className="mt-4 whitespace-pre-line text-sm leading-relaxed text-stone-600">
              {product.description}
            </p>
          )}

          {product.variants.length > 0 && (
            <div className="mt-6">
              <p className="mb-2 text-sm font-semibold text-stone-800">Variant</p>
              <div className="flex flex-wrap gap-2" role="radiogroup" aria-label="Product variant">
                {product.variants.map((v) => (
                  <button
                    key={v.id}
                    type="button"
                    role="radio"
                    aria-checked={v.id === variantId}
                    onClick={() => setVariantId(v.id)}
                    disabled={!v.isActive || v.stockQuantity <= 0}
                    className={`rounded-lg border px-4 py-2 text-sm font-medium ${
                      v.id === variantId
                        ? 'border-emerald-700 bg-emerald-50 text-emerald-800'
                        : 'border-stone-300 bg-white text-stone-700 hover:border-stone-400'
                    } disabled:opacity-40`}
                  >
                    {v.name}
                    {v.priceAdjustment > 0 && ` (+${money(v.priceAdjustment)})`}
                  </button>
                ))}
              </div>
            </div>
          )}

          <div className="mt-6 flex items-center gap-4">
            <div>
              <p className="mb-2 text-sm font-semibold text-stone-800">Quantity</p>
              <div className="flex items-center rounded-lg border border-stone-300">
                <button
                  type="button"
                  onClick={() => setQuantity((q) => Math.max(1, q - 1))}
                  className="px-3 py-2 text-lg text-stone-600 hover:bg-stone-100"
                  aria-label="Decrease quantity"
                >
                  −
                </button>
                <span className="w-10 text-center text-sm font-semibold" aria-live="polite">{quantity}</span>
                <button
                  type="button"
                  onClick={() => setQuantity((q) => Math.min(99, q + 1))}
                  className="px-3 py-2 text-lg text-stone-600 hover:bg-stone-100"
                  aria-label="Increase quantity"
                >
                  +
                </button>
              </div>
            </div>
          </div>

          {error && <p className="mt-3 text-sm text-rose-600">{error}</p>}

          <button
            type="button"
            onClick={onAdd}
            disabled={adding}
            className="mt-6 w-full rounded-lg bg-stone-900 px-6 py-3.5 text-sm font-bold text-white hover:bg-emerald-700 disabled:opacity-60 sm:w-auto sm:px-10"
          >
            {adding ? 'Adding…' : `Add to Cart · ${money(unitPrice * quantity)}`}
          </button>

          <p className="mt-3 text-xs text-stone-500">
            {product.stockQuantity > 0 ? `${product.stockQuantity} in stock` : 'Out of stock'}
          </p>
        </div>
      </div>

      {/* Related products */}
      {product.relatedProducts.length > 0 && (
        <section className="mt-16">
          <h2 className="mb-6 text-2xl font-bold text-stone-900">Related products</h2>
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
            {product.relatedProducts.map((p) => (
              <ProductCard key={p.id} product={p} />
            ))}
          </div>
        </section>
      )}

      {/* Lightbox */}
      {lightbox && mainImage && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/80 p-4"
          role="dialog"
          aria-modal="true"
          aria-label="Product image enlarged"
          onClick={() => setLightbox(false)}
        >
          <img
            src={mainImage.url}
            alt={mainImage.altText ?? product.name}
            className="max-h-[90vh] max-w-full rounded-lg object-contain"
            onClick={(e) => e.stopPropagation()}
          />
          <button
            type="button"
            onClick={() => setLightbox(false)}
            className="absolute right-4 top-4 rounded-full bg-white/20 p-2 text-white hover:bg-white/40"
            aria-label="Close"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="h-6 w-6" aria-hidden="true">
              <path d="M6 6l12 12M18 6L6 18" strokeWidth="2" strokeLinecap="round" />
            </svg>
          </button>
        </div>
      )}
    </div>
  );
}
