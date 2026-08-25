"use client";

import { useActionState } from "react";

import { changePasswordAction, type ActionState } from "@/lib/actions/account-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";

const initialState: ActionState = {};

export function ChangePasswordForm({ forced }: { forced: boolean }) {
  const [state, formAction, isPending] = useActionState(changePasswordAction, initialState);

  return (
    <form action={formAction} className="flex flex-col gap-4">
      {forced && (
        <Alert>
          <AlertDescription>เพื่อความปลอดภัย กรุณาเปลี่ยนรหัสผ่านก่อนใช้งานต่อ</AlertDescription>
        </Alert>
      )}

      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="current_password">รหัสผ่านปัจจุบัน</Label>
        <Input
          id="current_password"
          name="current_password"
          type="password"
          autoComplete="current-password"
          required
        />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="new_password">รหัสผ่านใหม่</Label>
        <Input
          id="new_password"
          name="new_password"
          type="password"
          autoComplete="new-password"
          minLength={8}
          required
        />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="confirm_password">ยืนยันรหัสผ่านใหม่</Label>
        <Input
          id="confirm_password"
          name="confirm_password"
          type="password"
          autoComplete="new-password"
          minLength={8}
          required
        />
      </div>

      <Button type="submit" disabled={isPending} className="mt-2">
        {isPending ? "กำลังบันทึก..." : "เปลี่ยนรหัสผ่าน"}
      </Button>
    </form>
  );
}
