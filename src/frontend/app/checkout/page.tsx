"use client";

import { useState, useTransition } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { useCart } from "@/components/cart-provider";
import { createOrderAction } from "@/lib/actions/order-actions";
import { formatCurrency } from "@/lib/format";

export default function CheckoutPage() {
  const { items, totalAmount, clear } = useCart();
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  if (items.length === 0) {
    return (
      <div className="flex flex-col items-center gap-4 py-16 text-center">
        <h1 className="text-2xl font-semibold">ไม่มีสินค้าสำหรับสั่งซื้อ</h1>
        <Button nativeButton={false} render={<Link href="/products" />}>
          ไปเลือกซื้อสินค้า
        </Button>
      </div>
    );
  }

  function handleSubmit() {
    setError(null);
    startTransition(async () => {
      const result = await createOrderAction(
        items.map((item) => ({
          product_id: item.product_id,
          vendor_id: item.vendor_id,
          quantity: item.quantity,
        })),
      );

      if (!result.ok) {
        setError(result.error);
        return;
      }

      clear();
      router.push(`/orders/${result.order.order_id}`);
    });
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-semibold">ยืนยันคำสั่งซื้อ</h1>

      {error && (
        <Alert variant="destructive">
          <AlertTitle>สั่งซื้อไม่สำเร็จ</AlertTitle>
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      <div className="rounded-xl border px-4">
        {items.map((item) => (
          <div
            key={`${item.product_id}-${item.vendor_id}`}
            className="flex items-center justify-between gap-4 border-b py-4 last:border-b-0"
          >
            <div className="flex flex-col gap-0.5">
              <span className="font-medium">{item.product_name}</span>
              <span className="text-sm text-muted-foreground">ผู้ขาย: {item.vendor_name}</span>
              <span className="text-sm text-muted-foreground">
                {formatCurrency(item.price)} x {item.quantity}
              </span>
            </div>
            <span className="font-medium">{formatCurrency(item.price * item.quantity)}</span>
          </div>
        ))}
      </div>

      <div className="flex items-center justify-between rounded-xl border p-4">
        <span className="text-lg font-semibold">ยอดรวมทั้งหมด</span>
        <span className="text-lg font-semibold">{formatCurrency(totalAmount)}</span>
      </div>

      <div className="flex justify-end gap-3">
        <Button variant="outline" nativeButton={false} render={<Link href="/cart" />}>
          กลับไปแก้ไขตะกร้า
        </Button>
        <Button onClick={handleSubmit} disabled={isPending}>
          {isPending ? "กำลังสั่งซื้อ..." : "ยืนยันสั่งซื้อ"}
        </Button>
      </div>
    </div>
  );
}
