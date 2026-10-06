import { del, get, post, put } from './client';
import type { CartDto, MiniCartDto } from '../types';

export const cartApi = {
  get: () => get<CartDto>('/api/cart'),
  mini: () => get<MiniCartDto>('/api/cart/mini'),
  addItem: (productId: number, variantId: number | null, quantity: number) =>
    post<CartDto>('/api/cart/items', { productId, variantId, quantity }),
  updateItem: (productId: number, variantId: number | null, quantity: number) =>
    put<CartDto>('/api/cart/items', { productId, variantId, quantity }),
  removeItem: (productId: number, variantId: number | null) => {
    const search = new URLSearchParams({ productId: String(productId) });
    if (variantId !== null) search.set('variantId', String(variantId));
    return del<CartDto>(`/api/cart/items?${search.toString()}`);
  },
  clear: () => post<CartDto>('/api/cart/clear'),
};
