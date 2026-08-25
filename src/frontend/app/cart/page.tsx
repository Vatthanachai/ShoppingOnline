"use client";

import Link from "next/link";

import { Button } from "@/components/ui/button";
import { CartLineItem } from "@/components/cart-line-item";
import { useCart } from "@/components/cart-provider";
import { formatCurrency } from "@/lib/format";

export default function CartPage() {
  const { items, totalAmount } = useCart();

  if (items.length === 0) {
    return (
      <div className="flex flex-col items-center gap-4 py-16 text-center">
        <h1 className="text-2xl font-semibold">ตะกร้าสินค้าว่างเปล่า</h1>
        <p className="text-muted-foreground">เลือกซื้อสินค้าที่คุณสนใจแล้วกลับมาที่นี่</p>
        <Button nativeButton={false} render={<Link href="/products" />}>
          ไปเลือกซื้อสินค้า
        </Button>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-semibold">ตะกร้าสินค้า</h1>

      <div className="rounded-xl border px-4">
        {items.map((item) => (
          <CartLineItem key={item.product_id} item={item} />
        ))}
      </div>

      <div className="flex items-center justify-between rounded-xl border p-4">
        <span className="text-lg font-semibold">ยอดรวม</span>
        <span className="text-lg font-semibold">{formatCurrency(totalAmount)}</span>
      </div>

      <div className="flex justify-end gap-3">
        <Button variant="outline" nativeButton={false} render={<Link href="/products" />}>
          เลือกซื้อสินค้าเพิ่ม
        </Button>
        <Button nativeButton={false} render={<Link href="/checkout" />}>
          ดำเนินการสั่งซื้อ
        </Button>
      </div>
    </div>
  );
}
