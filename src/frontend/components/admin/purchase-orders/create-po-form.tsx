"use client";

import { useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { Plus, Trash2 } from "lucide-react";

import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { DialogFooter } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { createPurchaseOrderAction } from "@/lib/actions/admin-purchase-order-actions";
import type { Product, Vendor } from "@/lib/types";

type Line = { productId?: number; quantity: number; unitCostQuoted?: number };

export function CreatePoForm({
  vendors,
  products,
  onSuccess,
}: {
  vendors: Vendor[];
  products: Product[];
  onSuccess?: () => void;
}) {
  const router = useRouter();
  const [vendorId, setVendorId] = useState<number | undefined>(undefined);
  const [lines, setLines] = useState<Line[]>([{ quantity: 1 }]);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function updateLine(index: number, patch: Partial<Line>) {
    setLines((prev) => prev.map((line, i) => (i === index ? { ...line, ...patch } : line)));
  }

  function handleSubmit() {
    setError(null);

    const items = lines
      .filter((line) => line.productId && line.quantity > 0)
      .map((line) => ({
        product_id: line.productId!,
        quantity: line.quantity,
        unit_cost_quoted: line.unitCostQuoted,
      }));

    if (!vendorId) {
      setError("กรุณาเลือกผู้ขาย");
      return;
    }
    if (items.length === 0) {
      setError("กรุณาเพิ่มรายการสินค้าอย่างน้อย 1 รายการ");
      return;
    }

    startTransition(async () => {
      const result = await createPurchaseOrderAction(vendorId, items);
      if (result.error) {
        setError(result.error);
        return;
      }
      onSuccess?.();
      if (result.purchaseOrderId) {
        router.push(`/admin/purchase-orders/${result.purchaseOrderId}`);
      }
    });
  }

  return (
    <div className="flex flex-col gap-3">
      {error && (
        <Alert variant="destructive">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="po_vendor_id">ผู้ขาย</Label>
        <Select
          items={vendors.map((v) => ({ value: String(v.vendor_id), label: v.vendor_name }))}
          onValueChange={(value) => setVendorId(Number(value))}
        >
          <SelectTrigger id="po_vendor_id" className="w-full">
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

      <div className="flex flex-col gap-2">
        <Label>รายการสินค้า</Label>
        {lines.map((line, index) => (
          <div key={index} className="flex items-end gap-2">
            <div className="flex flex-1 flex-col gap-1.5">
              <Select
                items={products.map((p) => ({ value: String(p.product_id), label: p.product_name }))}
                onValueChange={(value) => updateLine(index, { productId: Number(value) })}
              >
                <SelectTrigger className="w-full">
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
            <Input
              type="number"
              min={1}
              value={line.quantity}
              onChange={(e) => updateLine(index, { quantity: Math.max(1, Number(e.target.value) || 1) })}
              className="w-20"
              placeholder="จำนวน"
            />
            <Input
              type="number"
              min={0}
              step="0.01"
              value={line.unitCostQuoted ?? ""}
              onChange={(e) =>
                updateLine(index, { unitCostQuoted: e.target.value === "" ? undefined : Number(e.target.value) })
              }
              className="w-28"
              placeholder="ราคาอ้างอิง"
            />
            <Button
              type="button"
              variant="ghost"
              size="icon"
              aria-label="ลบรายการ"
              onClick={() => setLines((prev) => prev.filter((_, i) => i !== index))}
              disabled={lines.length === 1}
            >
              <Trash2 className="size-4" />
            </Button>
          </div>
        ))}
        <Button
          type="button"
          variant="outline"
          size="sm"
          className="self-start"
          onClick={() => setLines((prev) => [...prev, { quantity: 1 }])}
        >
          <Plus />
          เพิ่มรายการ
        </Button>
      </div>

      <DialogFooter>
        <Button type="button" onClick={handleSubmit} disabled={isPending}>
          {isPending ? "กำลังสร้าง..." : "สร้างใบสั่งซื้อ (ฉบับร่าง)"}
        </Button>
      </DialogFooter>
    </div>
  );
}
