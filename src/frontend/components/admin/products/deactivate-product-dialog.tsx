"use client";

import { useActionState, useEffect, useState } from "react";
import { Ban } from "lucide-react";

import { deactivateProductAction, type ActionState } from "@/lib/actions/admin-product-actions";
import { Button } from "@/components/ui/button";
import { Alert, AlertDescription } from "@/components/ui/alert";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

const initialState: ActionState = {};

export function DeactivateProductDialog({ productId }: { productId: number }) {
  const [open, setOpen] = useState(false);
  const [state, formAction, isPending] = useActionState(deactivateProductAction, initialState);

  useEffect(() => {
    if (state !== initialState && !state.error) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setOpen(false);
    }
  }, [state]);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger render={<Button variant="destructive" size="icon-sm" aria-label="ปิดใช้งานสินค้า" />}>
        <Ban />
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ปิดใช้งานสินค้า</DialogTitle>
          <DialogDescription>
            คุณแน่ใจหรือไม่ว่าต้องการปิดใช้งานสินค้านี้? การดำเนินการนี้ไม่สามารถย้อนกลับได้
          </DialogDescription>
        </DialogHeader>

        {state.error && (
          <Alert variant="destructive">
            <AlertDescription>{state.error}</AlertDescription>
          </Alert>
        )}

        <form action={formAction}>
          <input type="hidden" name="product_id" value={productId} />
          <DialogFooter>
            <Button type="submit" variant="destructive" disabled={isPending}>
              {isPending ? "กำลังปิดใช้งาน..." : "ปิดใช้งาน"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
