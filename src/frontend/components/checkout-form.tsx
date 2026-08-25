"use client";

import { useState, useTransition } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { useCart } from "@/components/cart-provider";
import { createOrderAction } from "@/lib/actions/order-actions";
import { formatCurrency } from "@/lib/format";
import type { ShippingAddress } from "@/lib/types";

export function CheckoutForm({ addresses }: { addresses: ShippingAddress[] }) {
  const { items, totalAmount, clear } = useCart();
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const defaultAddress = addresses.find((a) => a.is_default) ?? addresses[0];
  const [shippingAddressId, setShippingAddressId] = useState<number>(defaultAddress?.shipping_address_id);
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
        shippingAddressId,
        items.map((item) => ({
          product_id: item.product_id,
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

      <div className="flex flex-col gap-3 rounded-xl border p-4">
        <h2 className="font-semibold">ที่อยู่จัดส่ง</h2>
        {addresses.map((address) => (
          <label
            key={address.shipping_address_id}
            className={`flex cursor-pointer items-start gap-3 rounded-lg border p-3 text-sm transition-colors ${
              shippingAddressId === address.shipping_address_id
                ? "border-primary ring-1 ring-primary"
                : "border-border hover:bg-muted"
            }`}
          >
            <input
              type="radio"
              name="shipping_address_id"
              className="mt-1"
              checked={shippingAddressId === address.shipping_address_id}
              onChange={() => setShippingAddressId(address.shipping_address_id)}
            />
            <span>
              <span className="flex items-center gap-2">
                <span className="font-medium">{address.address_line1}</span>
                {address.is_default && (
                  <span className="rounded-full bg-primary px-2 py-0.5 text-xs text-primary-foreground">
                    ค่าเริ่มต้น
                  </span>
                )}
              </span>
              {address.address_line2 && <span className="block text-muted-foreground">{address.address_line2}</span>}
              <span className="block text-muted-foreground">
                {[address.city, address.state, address.postal_code].filter(Boolean).join(" ")}
              </span>
              <span className="block text-muted-foreground">{address.country}</span>
            </span>
          </label>
        ))}
      </div>

      <div className="rounded-xl border px-4">
        {items.map((item) => (
          <div
            key={item.product_id}
            className="flex items-center justify-between gap-4 border-b py-4 last:border-b-0"
          >
            <div className="flex flex-col gap-0.5">
              <span className="font-medium">{item.product_name}</span>
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
        <Button onClick={handleSubmit} disabled={isPending || !shippingAddressId}>
          {isPending ? "กำลังสั่งซื้อ..." : "ยืนยันสั่งซื้อ"}
        </Button>
      </div>
    </div>
  );
}
