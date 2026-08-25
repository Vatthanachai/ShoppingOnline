"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useCart } from "@/components/cart-provider";
import { formatCurrency } from "@/lib/format";
import type { Product } from "@/lib/types";

export function AddToCartForm({ product }: { product: Product }) {
  const { addItem } = useCart();
  const router = useRouter();

  const [quantity, setQuantity] = useState(1);
  const [justAdded, setJustAdded] = useState(false);

  const outOfStock = product.available_quantity <= 0;

  if (outOfStock) {
    return <p className="text-sm text-muted-foreground">สินค้าหมด ขณะนี้ยังไม่มีสต็อก</p>;
  }

  return (
    <div className="flex flex-col gap-4">
      <p className="text-sm text-muted-foreground">คงเหลือ {product.available_quantity} ชิ้น</p>

      <div className="flex items-end gap-3">
        <div className="flex flex-col gap-2">
          <Label htmlFor="quantity">จำนวน</Label>
          <Input
            id="quantity"
            type="number"
            min={1}
            max={product.available_quantity}
            value={quantity}
            onChange={(e) => {
              const value = Number(e.target.value);
              setQuantity(Number.isFinite(value) ? Math.max(1, Math.min(value, product.available_quantity)) : 1);
            }}
            className="w-24"
          />
        </div>
        <Button
          type="button"
          onClick={() => {
            addItem({
              product_id: product.product_id,
              product_name: product.product_name,
              price: product.price_with_tax,
              quantity,
            });
            setJustAdded(true);
            setTimeout(() => setJustAdded(false), 1500);
          }}
        >
          {justAdded ? "เพิ่มลงตะกร้าแล้ว" : "เพิ่มลงตะกร้า"}
        </Button>
        <Button type="button" variant="outline" onClick={() => router.push("/cart")}>
          ไปที่ตะกร้า
        </Button>
      </div>

      <p className="text-sm text-muted-foreground">
        ราคา {formatCurrency(product.price_with_tax)} ต่อชิ้น (รวมภาษี {product.tax_rate_percent}%)
      </p>
    </div>
  );
}
