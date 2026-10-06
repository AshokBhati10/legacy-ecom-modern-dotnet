import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '../state/CartContext';
import type { ProductCardDto } from '../types';
import { money } from '../utils/format';
import ProductImage from './ProductImage';

interface Props {
  product: ProductCardDto;
}

/** Modern equivalent of the legacy _ProductCard.cshtml partial. */
export default function ProductCard({ product }: Props) {
  const { addItem } = useCart();
  const [adding, setAdding] = useState(false);
  const [added, setAdded] = useState(false);

  const onAdd = async () => {
    setAdding(true);
    try {
      await addItem(product.id, null, 1);
      setAdded(true);
      setTimeout(() => setAdded(false), 1500);
    } finally {
      setAdding(false);
    }
  };

  const hasSale = product.salePrice !== null && product.salePrice < product.price;

  return (
    <div className="group flex flex-col overflow-hidden rounded-xl border border-stone-200 bg-white transition-shadow hover:shadow-lg">
      <Link to={`/products/${product.id}`} className="relative block aspect-square overflow-hidden">
        <ProductImage
          src={product.thumbnailUrl}
          alt={product.name}
          className="h-full w-full transition-transform duration-300 group-hover:scale-105"
        />
        {hasSale && (
          <span className="absolute left-3 top-3 rounded-full bg-rose-600 px-2.5 py-1 text-xs font-semibold text-white">
            Sale
          </span>
        )}
      </Link>

      <div className="flex flex-1 flex-col gap-2 p-4">
        <Link to={`/products/${product.id}`} className="line-clamp-2 text-sm font-medium text-stone-900 hover:text-emerald-700">
          {product.name}
        </Link>

        <div className="mt-auto flex items-baseline gap-2">
          <span className={`text-lg font-bold ${hasSale ? 'text-rose-600' : 'text-stone-900'}`}>
            {money(product.effectivePrice)}
          </span>
          {hasSale && (
            <span className="text-sm text-stone-400 line-through">{money(product.price)}</span>
          )}
        </div>

        <button
          type="button"
          onClick={onAdd}
          disabled={adding}
          className={`mt-1 w-full rounded-lg px-4 py-2 text-sm font-semibold transition-colors ${
            added
              ? 'bg-emerald-100 text-emerald-800'
              : 'bg-stone-900 text-white hover:bg-emerald-700 disabled:opacity-60'
          }`}
        >
          {adding ? 'Adding…' : added ? 'Added ✓' : 'Add to Cart'}
        </button>
      </div>
    </div>
  );
}
