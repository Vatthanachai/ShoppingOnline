"use client";

import { useState, useTransition } from "react";

import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { sendPurchaseOrderAction } from "@/lib/actions/admin-purchase-order-actions";

export function SendPoButton({ purchaseOrderId }: { purchaseOrderId: number }) {
  const [open, setOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function handleConfirm() {
    setError(null);
    startTransition(async () => {
      const result = await sendPurchaseOrderAction(purchaseOrderId);
      if (result.error) {
        setError(result.error);
        return;
      }
      setOpen(false);
    });
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger render={<Button />}>ส่งใบสั่งซื้อให้ผู้ขาย</DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ส่งใบสั่งซื้อนี้หรือไม่?</DialogTitle>
          <DialogDescription>ระบบจะส่งอีเมลใบสั่งซื้อไปยังผู้ขาย และเปลี่ยนสถานะเป็น &quot;ส่งแล้ว&quot;</DialogDescription>
        </DialogHeader>

        {error && (
          <Alert variant="destructive">
            <AlertDescription>{error}</AlertDescription>
          </Alert>
        )}

        <DialogFooter>
          <DialogClose render={<Button variant="outline" />}>ยกเลิก</DialogClose>
          <Button onClick={handleConfirm} disabled={isPending}>
            {isPending ? "กำลังส่ง..." : "ยืนยันส่ง"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
