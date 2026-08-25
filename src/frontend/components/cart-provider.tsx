"use client";

import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";

export interface CartItem {
  product_id: number;
  vendor_id: number;
  product_name: string;
  vendor_name: string;
  price: number;
  quantity: number;
}

interface CartContextValue {
  items: CartItem[];
  addItem: (item: CartItem) => void;
  updateQuantity: (productId: number, vendorId: number, quantity: number) => void;
  removeItem: (productId: number, vendorId: number) => void;
  clear: () => void;
  totalItems: number;
  totalAmount: number;
}

const CartContext = createContext<CartContextValue | undefined>(undefined);
const STORAGE_KEY = "shopping-cart";

function sameLine(a: { product_id: number; vendor_id: number }, b: { product_id: number; vendor_id: number }) {
  return a.product_id === b.product_id && a.vendor_id === b.vendor_id;
}

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartItem[]>([]);
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    // Deliberately deferred to a post-mount effect: localStorage isn't available during
    // SSR, so hydrating synchronously here (instead of via a lazy useState initializer)
    // keeps the server-rendered and first client render markup identical, avoiding a
    // hydration mismatch. The one extra render this causes is the intended trade-off.
    try {
      const raw = window.localStorage.getItem(STORAGE_KEY);
      // eslint-disable-next-line react-hooks/set-state-in-effect
      if (raw) setItems(JSON.parse(raw));
    } catch {
      // ignore corrupted storage
    }
    setLoaded(true);
  }, []);

  useEffect(() => {
    if (!loaded) return;
    try {
      window.localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
    } catch {
      // storage unavailable (private mode, etc.) - cart just won't persist
    }
  }, [items, loaded]);

  const value = useMemo<CartContextValue>(() => {
    const addItem = (item: CartItem) => {
      setItems((prev) => {
        const existing = prev.find((line) => sameLine(line, item));
        if (existing) {
          return prev.map((line) =>
            sameLine(line, item) ? { ...line, quantity: line.quantity + item.quantity } : line,
          );
        }
        return [...prev, item];
      });
    };

    const updateQuantity = (productId: number, vendorId: number, quantity: number) => {
      setItems((prev) =>
        quantity <= 0
          ? prev.filter((line) => !sameLine(line, { product_id: productId, vendor_id: vendorId }))
          : prev.map((line) =>
              sameLine(line, { product_id: productId, vendor_id: vendorId }) ? { ...line, quantity } : line,
            ),
      );
    };

    const removeItem = (productId: number, vendorId: number) => {
      setItems((prev) => prev.filter((line) => !sameLine(line, { product_id: productId, vendor_id: vendorId })));
    };

    const clear = () => setItems([]);

    const totalItems = items.reduce((sum, line) => sum + line.quantity, 0);
    const totalAmount = items.reduce((sum, line) => sum + line.quantity * line.price, 0);

    return { items, addItem, updateQuantity, removeItem, clear, totalItems, totalAmount };
  }, [items]);

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error("useCart must be used within a CartProvider");
  return ctx;
}
