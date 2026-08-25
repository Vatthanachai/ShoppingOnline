"use client";

import { useActionState, useEffect, useState } from "react";
import { Trash2 } from "lucide-react";

import { deleteStockAction, type ActionState } from "@/lib/actions/admin-stock-actions";
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

export function DeleteStockDialog({ stockId }: { stockId: number }) {
  const [open, setOpen] = useState(false);
  const [state, formAction, isPending] = useActionState(deleteStockAction, initialState);

  useEffect(() => {
    if (state !== initialState && !state.error) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setOpen(false);
    }
  }, [state]);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger render={<Button variant="destructive" size="icon-sm" aria-label="ลบสต็อก" />}>
        <Trash2 />
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ลบสต็อก</DialogTitle>
          <DialogDescription>คุณแน่ใจหรือไม่ว่าต้องการลบสต็อกนี้? การดำเนินการนี้ไม่สามารถย้อนกลับได้</DialogDescription>
        </DialogHeader>

        {state.error && (
          <Alert variant="destructive">
            <AlertDescription>{state.error}</AlertDescription>
          </Alert>
        )}

        <form action={formAction}>
          <input type="hidden" name="stock_id" value={stockId} />
          <DialogFooter>
            <Button type="submit" variant="destructive" disabled={isPending}>
              {isPending ? "กำลังลบ..." : "ลบสต็อก"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
