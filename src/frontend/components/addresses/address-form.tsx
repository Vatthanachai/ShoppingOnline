"use client";

import { useActionState, useEffect } from "react";

import { createAddressAction, updateAddressAction, type ActionState } from "@/lib/actions/address-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import type { ShippingAddress } from "@/lib/types";

const initialState: ActionState = {};

type AddressFormProps = {
  mode: "create" | "edit";
  initial?: ShippingAddress;
  onSuccess?: () => void;
};

export function AddressForm({ mode, initial, onSuccess }: AddressFormProps) {
  const action = mode === "edit" ? updateAddressAction : createAddressAction;
  const [state, formAction, isPending] = useActionState(action, initialState);

  // On success (no error, and a submission actually ran) the list has been revalidated; close the dialog.
  useEffect(() => {
    if (state !== initialState && !state.error) {
      onSuccess?.();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state]);

  return (
    <form action={formAction} className="flex flex-col gap-3">
      {mode === "edit" && initial && (
        <input type="hidden" name="shipping_address_id" value={initial.shipping_address_id} />
      )}

      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="address_line1">ที่อยู่ บรรทัดที่ 1</Label>
        <Input id="address_line1" name="address_line1" type="text" defaultValue={initial?.address_line1} required />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="address_line2">ที่อยู่ บรรทัดที่ 2 (ถ้ามี)</Label>
        <Input id="address_line2" name="address_line2" type="text" defaultValue={initial?.address_line2} />
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="city">เมือง/อำเภอ</Label>
          <Input id="city" name="city" type="text" defaultValue={initial?.city} required />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="state">จังหวัด</Label>
          <Input id="state" name="state" type="text" defaultValue={initial?.state} />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="postal_code">รหัสไปรษณีย์</Label>
          <Input id="postal_code" name="postal_code" type="text" defaultValue={initial?.postal_code} required />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="country">ประเทศ</Label>
          <Input id="country" name="country" type="text" defaultValue={initial?.country ?? "Thailand"} required />
        </div>
      </div>

      <DialogFooter>
        <Button type="submit" disabled={isPending}>
          {isPending ? "กำลังบันทึก..." : mode === "edit" ? "บันทึกการเปลี่ยนแปลง" : "เพิ่มที่อยู่"}
        </Button>
      </DialogFooter>
    </form>
  );
}
