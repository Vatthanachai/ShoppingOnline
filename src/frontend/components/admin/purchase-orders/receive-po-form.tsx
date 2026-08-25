"use client";

import { useState, useTransition } from "react";

import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { receivePurchaseOrderAction } from "@/lib/actions/admin-purchase-order-actions";
import type { PurchaseOrderItem } from "@/lib/types";

type LineInput = { quantityReceived: string; unitCost: string };

export function ReceivePoForm({ purchaseOrderId, items }: { purchaseOrderId: number; items: PurchaseOrderItem[] }) {
  const outstanding = items.filter((i) => i.quantity_received < i.quantity_ordered);

  const [inputs, setInputs] = useState<Record<number, LineInput>>(
    Object.fromEntries(
      outstanding.map((i) => [
        i.purchase_order_item_id,
        { quantityReceived: "", unitCost: i.unit_cost_quoted ? String(i.unit_cost_quoted) : "" },
      ]),
    ),
  );
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  if (outstanding.length === 0) {
    return null;
  }

  function updateInput(id: number, patch: Partial<LineInput>) {
    setInputs((prev) => ({ ...prev, [id]: { ...prev[id], ...patch } }));
  }

  function handleSubmit() {
    setError(null);

    const lines = outstanding
      .map((item) => {
        const input = inputs[item.purchase_order_item_id];
        const quantityReceived = Number(input?.quantityReceived);
        const unitCost = Number(input?.unitCost);
        return { purchase_order_item_id: item.purchase_order_item_id, quantity_received: quantityReceived, unit_cost: unitCost };
      })
      .filter((line) => line.quantity_received > 0);

    if (lines.some((line) => Number.isNaN(line.unit_cost) || line.unit_cost < 0)) {
      setError("กรุณากรอกต้นทุนต่อหน่วยให้ถูกต้อง");
      return;
    }

    startTransition(async () => {
      const result = await receivePurchaseOrderAction(purchaseOrderId, lines);
      if (result.error) {
        setError(result.error);
        return;
      }
      setInputs(Object.fromEntries(outstanding.map((i) => [i.purchase_order_item_id, { quantityReceived: "", unitCost: "" }])));
    });
  }

  return (
    <Card>
      <CardContent className="flex flex-col gap-3">
        <h2 className="font-semibold">รับสินค้าเข้าสต็อก</h2>
        <p className="text-sm text-muted-foreground">กรอกเฉพาะรายการที่ได้รับในรอบนี้ (รองรับรับหลายรอบต่อ 1 ใบสั่งซื้อ)</p>

        {error && (
          <Alert variant="destructive">
            <AlertDescription>{error}</AlertDescription>
          </Alert>
        )}

        <div className="flex flex-col gap-3">
          {outstanding.map((item) => {
            const remaining = item.quantity_ordered - item.quantity_received;
            const input = inputs[item.purchase_order_item_id];
            return (
              <div key={item.purchase_order_item_id} className="flex items-end gap-3 border-b pb-3 last:border-b-0">
                <div className="flex-1">
                  <p className="text-sm font-medium">{item.product_name}</p>
                  <p className="text-xs text-muted-foreground">ค้างรับ {remaining} จาก {item.quantity_ordered}</p>
                </div>
                <div className="flex flex-col gap-1.5">
                  <Label className="text-xs">จำนวนที่ได้รับ</Label>
                  <Input
                    type="number"
                    min={0}
                    max={remaining}
                    value={input?.quantityReceived ?? ""}
                    onChange={(e) => updateInput(item.purchase_order_item_id, { quantityReceived: e.target.value })}
                    className="w-24"
                  />
                </div>
                <div className="flex flex-col gap-1.5">
                  <Label className="text-xs">ต้นทุน/หน่วย</Label>
                  <Input
                    type="number"
                    min={0}
                    step="0.01"
                    value={input?.unitCost ?? ""}
                    onChange={(e) => updateInput(item.purchase_order_item_id, { unitCost: e.target.value })}
                    className="w-28"
                  />
                </div>
              </div>
            );
          })}
        </div>

        <Button type="button" onClick={handleSubmit} disabled={isPending} className="self-end">
          {isPending ? "กำลังบันทึก..." : "บันทึกการรับสินค้า"}
        </Button>
      </CardContent>
    </Card>
  );
}
