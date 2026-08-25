"use client";

import { useActionState, useEffect } from "react";

import { createStockAction, type ActionState } from "@/lib/actions/admin-stock-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { Product, Vendor } from "@/lib/types";

const initialState: ActionState = {};

type StockCreateFormProps = {
  products: Product[];
  vendors: Vendor[];
  onSuccess?: () => void;
};

export function StockCreateForm({ products, vendors, onSuccess }: StockCreateFormProps) {
  const [state, formAction, isPending] = useActionState(createStockAction, initialState);

  useEffect(() => {
    if (state !== initialState && !state.error) {
      onSuccess?.();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state]);

  return (
    <form action={formAction} className="flex flex-col gap-3">
      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="product_id">สินค้า</Label>
        <Select
          name="product_id"
          items={products.map((p) => ({ value: String(p.product_id), label: p.product_name }))}
        >
          <SelectTrigger id="product_id" className="w-full">
            <SelectValue placeholder="เลือกสินค้า" />
          </SelectTrigger>
          <SelectContent>
            {products.map((p) => (
              <SelectItem key={p.product_id} value={String(p.product_id)}>
                {p.product_name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="vendor_id">ผู้ขาย</Label>
        <Select
          name="vendor_id"
          items={vendors.map((v) => ({ value: String(v.vendor_id), label: v.vendor_name }))}
        >
          <SelectTrigger id="vendor_id" className="w-full">
            <SelectValue placeholder="เลือกผู้ขาย" />
          </SelectTrigger>
          <SelectContent>
            {vendors.map((v) => (
              <SelectItem key={v.vendor_id} value={String(v.vendor_id)}>
                {v.vendor_name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="quantity">จำนวน</Label>
          <Input id="quantity" name="quantity" type="number" min={0} step={1} defaultValue={0} required />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="price">ราคา</Label>
          <Input id="price" name="price" type="number" min={0} step="0.01" defaultValue={0} required />
        </div>
      </div>

      <DialogFooter>
        <Button type="submit" disabled={isPending}>
          {isPending ? "กำลังบันทึก..." : "เพิ่มสต็อก"}
        </Button>
      </DialogFooter>
    </form>
  );
}
