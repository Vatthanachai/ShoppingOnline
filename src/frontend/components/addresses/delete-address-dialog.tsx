"use client";

import { useActionState, useEffect, useState } from "react";
import { Trash2 } from "lucide-react";

import { deleteAddressAction, type ActionState } from "@/lib/actions/address-actions";
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

export function DeleteAddressDialog({ addressId }: { addressId: number }) {
  const [open, setOpen] = useState(false);
  const [state, formAction, isPending] = useActionState(deleteAddressAction, initialState);

  useEffect(() => {
    // Reacting to the Server Action's result (an external event, not a derived value) is
    // exactly what an effect is for here; there's no way to close the dialog from render.
    if (state !== initialState && !state.error) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setOpen(false);
    }
  }, [state]);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger render={<Button variant="destructive" size="icon-sm" aria-label="ลบที่อยู่" />}>
        <Trash2 />
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ลบที่อยู่</DialogTitle>
          <DialogDescription>คุณแน่ใจหรือไม่ว่าต้องการลบที่อยู่นี้? การดำเนินการนี้ไม่สามารถย้อนกลับได้</DialogDescription>
        </DialogHeader>

        {state.error && (
          <Alert variant="destructive">
            <AlertDescription>{state.error}</AlertDescription>
          </Alert>
        )}

        <form action={formAction}>
          <input type="hidden" name="shipping_address_id" value={addressId} />
          <DialogFooter>
            <Button type="submit" variant="destructive" disabled={isPending}>
              {isPending ? "กำลังลบ..." : "ลบที่อยู่"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
