"use client";

import { useActionState, useEffect } from "react";

import { updateStockAction, type ActionState } from "@/lib/actions/admin-stock-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import type { Stock } from "@/lib/types";

const initialState: ActionState = {};

type StockEditFormProps = {
  stock: Stock;
  onSuccess?: () => void;
};

export function StockEditForm({ stock, onSuccess }: StockEditFormProps) {
  const [state, formAction, isPending] = useActionState(updateStockAction, initialState);

  useEffect(() => {
    if (state !== initialState && !state.error) {
      onSuccess?.();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state]);

  return (
    <form action={formAction} className="flex flex-col gap-3">
      <input type="hidden" name="stock_id" value={stock.stock_id} />

      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="quantity">จำนวน</Label>
          <Input
            id="quantity"
            name="quantity"
            type="number"
            min={0}
            step={1}
            defaultValue={stock.quantity}
            required
          />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="price">ราคา</Label>
          <Input id="price" name="price" type="number" min={0} step="0.01" defaultValue={stock.price} required />
        </div>
      </div>

      <DialogFooter>
        <Button type="submit" disabled={isPending}>
          {isPending ? "กำลังบันทึก..." : "บันทึกการเปลี่ยนแปลง"}
        </Button>
      </DialogFooter>
    </form>
  );
}
