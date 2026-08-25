"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useCart } from "@/components/cart-provider";
import { formatCurrency } from "@/lib/format";
import type { Product, Stock } from "@/lib/types";

export function AddToCartForm({ product, stocks }: { product: Product; stocks: Stock[] }) {
  const { addItem } = useCart();
  const router = useRouter();

  const availableStocks = stocks.filter((s) => s.quantity > 0);
  const [selectedVendorId, setSelectedVendorId] = useState<number | undefined>(availableStocks[0]?.vendor_id);
  const [quantity, setQuantity] = useState(1);
  const [justAdded, setJustAdded] = useState(false);

  const selectedStock = stocks.find((s) => s.vendor_id === selectedVendorId);

  if (stocks.length === 0) {
    return <p className="text-sm text-muted-foreground">ยังไม่มีผู้ขายสินค้านี้</p>;
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-2">
        <Label>เลือกผู้ขาย</Label>
        <div className="flex flex-col gap-2">
          {stocks.map((stock) => {
            const outOfStock = stock.quantity === 0;
            const selected = stock.vendor_id === selectedVendorId;
            return (
              <button
                key={stock.stock_id}
                type="button"
                disabled={outOfStock}
                onClick={() => setSelectedVendorId(stock.vendor_id)}
                className={`flex items-center justify-between rounded-lg border px-3 py-2 text-left text-sm transition-colors disabled:cursor-not-allowed disabled:opacity-50 ${
                  selected ? "border-primary ring-1 ring-primary" : "border-border hover:bg-muted"
                }`}
              >
                <span>
                  <span className="font-medium">{stock.vendor_name}</span>
                  <span className="ml-2 text-muted-foreground">
                    {outOfStock ? "สินค้าหมด" : `คงเหลือ ${stock.quantity} ชิ้น`}
                  </span>
                </span>
                <span className="font-medium">{formatCurrency(stock.price)}</span>
              </button>
            );
          })}
        </div>
      </div>

      {selectedStock && (
        <div className="flex items-end gap-3">
          <div className="flex flex-col gap-2">
            <Label htmlFor="quantity">จำนวน</Label>
            <Input
              id="quantity"
              type="number"
              min={1}
              max={selectedStock.quantity}
              value={quantity}
              onChange={(e) => {
                const value = Number(e.target.value);
                setQuantity(Number.isFinite(value) ? Math.max(1, Math.min(value, selectedStock.quantity)) : 1);
              }}
              className="w-24"
            />
          </div>
          <Button
            type="button"
            onClick={() => {
              addItem({
                product_id: product.product_id,
                vendor_id: selectedStock.vendor_id,
                product_name: product.product_name,
                vendor_name: selectedStock.vendor_name,
                price: selectedStock.price,
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
      )}
    </div>
  );
}
