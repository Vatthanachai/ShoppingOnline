"use client";

import { useActionState, useEffect } from "react";

import { createProductAction, updateProductAction, type ActionState } from "@/lib/actions/admin-product-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { Category, Product, Vendor } from "@/lib/types";

const initialState: ActionState = {};

type ProductFormProps = {
  mode: "create" | "edit";
  initial?: Product;
  categories: Category[];
  vendors: Vendor[];
  onSuccess?: () => void;
};

export function ProductForm({ mode, initial, categories, vendors, onSuccess }: ProductFormProps) {
  const action = mode === "edit" ? updateProductAction : createProductAction;
  const [state, formAction, isPending] = useActionState(action, initialState);

  useEffect(() => {
    if (state !== initialState && !state.error) {
      onSuccess?.();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state]);

  return (
    <form action={formAction} className="flex flex-col gap-3">
      {mode === "edit" && initial && <input type="hidden" name="product_id" value={initial.product_id} />}

      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="product_category_id">หมวดหมู่</Label>
          <Select
            name="product_category_id"
            items={categories.map((c) => ({ value: String(c.product_category_id), label: c.category_name }))}
            defaultValue={initial ? String(initial.product_category_id) : undefined}
          >
            <SelectTrigger id="product_category_id" className="w-full">
              <SelectValue placeholder="เลือกหมวดหมู่" />
            </SelectTrigger>
            <SelectContent>
              {categories.map((c) => (
                <SelectItem key={c.product_category_id} value={String(c.product_category_id)}>
                  {c.category_name}
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
            defaultValue={initial ? String(initial.vendor_id) : undefined}
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
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="product_code">รหัสสินค้า</Label>
          <Input id="product_code" name="product_code" type="text" defaultValue={initial?.product_code} required />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="product_name">ชื่อสินค้า</Label>
          <Input id="product_name" name="product_name" type="text" defaultValue={initial?.product_name} required />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="sell_price">ราคาขาย (ไม่รวมภาษี)</Label>
          <Input
            id="sell_price"
            name="sell_price"
            type="number"
            min={0}
            step="0.01"
            defaultValue={initial?.sell_price}
            required
          />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="tax_rate_percent">อัตราภาษี (%)</Label>
          <Input
            id="tax_rate_percent"
            name="tax_rate_percent"
            type="number"
            min={0}
            step="0.01"
            defaultValue={initial?.tax_rate_percent ?? 7}
            required
          />
        </div>
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="description">รายละเอียด</Label>
        <Input id="description" name="description" type="text" defaultValue={initial?.description} />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="image_path">ลิงก์รูปภาพ (ถ้ามี)</Label>
        <Input
          id="image_path"
          name="image_path"
          type="text"
          defaultValue={initial?.image_path ?? undefined}
          placeholder="https://..."
        />
      </div>

      <DialogFooter>
        <Button type="submit" disabled={isPending}>
          {isPending ? "กำลังบันทึก..." : mode === "edit" ? "บันทึกการเปลี่ยนแปลง" : "เพิ่มสินค้า"}
        </Button>
      </DialogFooter>
    </form>
  );
}
