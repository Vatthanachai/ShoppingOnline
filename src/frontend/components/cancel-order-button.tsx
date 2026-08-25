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
import { cancelOrderAction } from "@/lib/actions/order-actions";

export function CancelOrderButton({ orderId }: { orderId: number }) {
  const [open, setOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function handleConfirm() {
    setError(null);
    startTransition(async () => {
      const result = await cancelOrderAction(orderId);
      if (result.error) {
        setError(result.error);
        return;
      }
      setOpen(false);
    });
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger render={<Button variant="destructive" />}>ยกเลิกคำสั่งซื้อ</DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ยกเลิกคำสั่งซื้อนี้หรือไม่?</DialogTitle>
          <DialogDescription>เมื่อยกเลิกแล้วจะไม่สามารถย้อนกลับได้</DialogDescription>
        </DialogHeader>

        {error && (
          <Alert variant="destructive">
            <AlertDescription>{error}</AlertDescription>
          </Alert>
        )}

        <DialogFooter>
          <DialogClose render={<Button variant="outline" />}>ยกเลิก</DialogClose>
          <Button variant="destructive" onClick={handleConfirm} disabled={isPending}>
            {isPending ? "กำลังยกเลิก..." : "ยืนยันการยกเลิก"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
