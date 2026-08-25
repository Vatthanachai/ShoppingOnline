"use client";

import { Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useCart, type CartItem } from "@/components/cart-provider";
import { formatCurrency } from "@/lib/format";

export function CartLineItem({ item }: { item: CartItem }) {
  const { updateQuantity, removeItem } = useCart();

  return (
    <div className="flex items-center justify-between gap-4 border-b py-4 last:border-b-0">
      <div className="flex flex-col gap-0.5">
        <span className="font-medium">{item.product_name}</span>
        <span className="text-sm text-muted-foreground">{formatCurrency(item.price)} / ชิ้น</span>
      </div>

      <div className="flex items-center gap-3">
        <Input
          type="number"
          min={1}
          value={item.quantity}
          onChange={(e) => {
            const value = Number(e.target.value);
            updateQuantity(item.product_id, Number.isFinite(value) ? Math.max(1, value) : 1);
          }}
          className="w-20"
        />
        <span className="w-24 text-right font-medium">{formatCurrency(item.price * item.quantity)}</span>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          onClick={() => removeItem(item.product_id)}
          aria-label="ลบสินค้า"
        >
          <Trash2 className="size-4" />
        </Button>
      </div>
    </div>
  );
}
