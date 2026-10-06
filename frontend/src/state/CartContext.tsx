import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import { cartApi } from '../api/cart';
import { ApiError } from '../api/client';
import type { CartDto, MiniCartDto } from '../types';

interface CartContextValue {
  cart: CartDto | null;
  mini: MiniCartDto | null;
  loading: boolean;
  drawerOpen: boolean;
  setDrawerOpen: (open: boolean) => void;
  refresh: () => Promise<void>;
  addItem: (productId: number, variantId: number | null, quantity: number) => Promise<void>;
  updateItem: (productId: number, variantId: number | null, quantity: number) => Promise<void>;
  removeItem: (productId: number, variantId: number | null) => Promise<void>;
  clear: () => Promise<void>;
  lastError: string | null;
  clearError: () => void;
}

const CartContext = createContext<CartContextValue | null>(null);

export function CartProvider({ children }: { children: ReactNode }) {
  const [cart, setCart] = useState<CartDto | null>(null);
  const [mini, setMini] = useState<MiniCartDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [lastError, setLastError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    try {
      const [c, m] = await Promise.all([cartApi.get(), cartApi.mini()]);
      setCart(c);
      setMini(m);
    } catch (err) {
      if (!(err instanceof ApiError)) console.error('Cart refresh failed', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh]);

  const apply = useCallback((next: CartDto) => {
    setCart(next);
    setMini({ itemCount: next.itemCount, subTotal: next.subTotal });
    setLastError(null);
  }, []);

  const fail = useCallback((err: unknown) => {
    const message = err instanceof ApiError ? err.readableMessage() : 'Something went wrong.';
    setLastError(message);
    throw err;
  }, []);

  const addItem = useCallback(
    async (productId: number, variantId: number | null, quantity: number) => {
      try {
        apply(await cartApi.addItem(productId, variantId, quantity));
      } catch (err) {
        fail(err);
      }
    },
    [apply, fail],
  );

  const updateItem = useCallback(
    async (productId: number, variantId: number | null, quantity: number) => {
      try {
        apply(await cartApi.updateItem(productId, variantId, quantity));
      } catch (err) {
        fail(err);
      }
    },
    [apply, fail],
  );

  const removeItem = useCallback(
    async (productId: number, variantId: number | null) => {
      try {
        apply(await cartApi.removeItem(productId, variantId));
      } catch (err) {
        fail(err);
      }
    },
    [apply, fail],
  );

  const clear = useCallback(async () => {
    try {
      apply(await cartApi.clear());
    } catch (err) {
      fail(err);
    }
  }, [apply, fail]);

  const clearError = useCallback(() => setLastError(null), []);

  const value = useMemo(
    () => ({
      cart,
      mini,
      loading,
      drawerOpen,
      setDrawerOpen,
      refresh,
      addItem,
      updateItem,
      removeItem,
      clear,
      lastError,
      clearError,
    }),
    [cart, mini, loading, drawerOpen, refresh, addItem, updateItem, removeItem, clear, lastError, clearError],
  );
  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart(): CartContextValue {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
