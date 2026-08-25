"use client";

import { useActionState, useEffect } from "react";

import { createVendorAction, updateVendorAction, type ActionState } from "@/lib/actions/admin-vendor-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import type { Vendor } from "@/lib/types";

const initialState: ActionState = {};

type VendorFormProps = {
  mode: "create" | "edit";
  initial?: Vendor;
  onSuccess?: () => void;
};

export function VendorForm({ mode, initial, onSuccess }: VendorFormProps) {
  const action = mode === "edit" ? updateVendorAction : createVendorAction;
  const [state, formAction, isPending] = useActionState(action, initialState);

  useEffect(() => {
    if (state !== initialState && !state.error) {
      onSuccess?.();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state]);

  return (
    <form action={formAction} className="flex flex-col gap-3">
      {mode === "edit" && initial && <input type="hidden" name="vendor_id" value={initial.vendor_id} />}

      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="vendor_name">ชื่อผู้ขาย</Label>
        <Input id="vendor_name" name="vendor_name" type="text" defaultValue={initial?.vendor_name} required />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="contact_person">ผู้ติดต่อ</Label>
        <Input id="contact_person" name="contact_person" type="text" defaultValue={initial?.contact_person} />
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="email">อีเมล</Label>
          <Input id="email" name="email" type="email" defaultValue={initial?.email} required />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="phone">เบอร์โทรศัพท์</Label>
          <Input id="phone" name="phone" type="text" defaultValue={initial?.phone} />
        </div>
      </div>

      <DialogFooter>
        <Button type="submit" disabled={isPending}>
          {isPending ? "กำลังบันทึก..." : mode === "edit" ? "บันทึกการเปลี่ยนแปลง" : "เพิ่มผู้ขาย"}
        </Button>
      </DialogFooter>
    </form>
  );
}
