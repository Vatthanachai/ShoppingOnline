"use client";

import { useActionState, useState } from "react";

import { deactivateAccountAction, type ActionState } from "@/lib/actions/account-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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

export function DeactivateDialog() {
  const [open, setOpen] = useState(false);
  const [state, formAction, isPending] = useActionState(deactivateAccountAction, initialState);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger render={<Button variant="destructive" />}>ปิดใช้งานบัญชี</DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ปิดใช้งานบัญชี</DialogTitle>
          <DialogDescription>
            คุณแน่ใจหรือไม่ว่าต้องการปิดใช้งานบัญชี? กรุณากรอกรหัสผ่านปัจจุบันเพื่อยืนยัน การดำเนินการนี้จะทำให้คุณออกจากระบบทันที
          </DialogDescription>
        </DialogHeader>

        {state.error && (
          <Alert variant="destructive">
            <AlertDescription>{state.error}</AlertDescription>
          </Alert>
        )}

        <form action={formAction} className="flex flex-col gap-3">
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="deactivate-current-password">รหัสผ่านปัจจุบัน</Label>
            <Input
              id="deactivate-current-password"
              name="current_password"
              type="password"
              autoComplete="current-password"
              required
            />
          </div>
          <DialogFooter>
            <Button type="submit" variant="destructive" disabled={isPending}>
              {isPending ? "กำลังดำเนินการ..." : "ยืนยันการปิดใช้งานบัญชี"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
