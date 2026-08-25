"use client";

import { useActionState, useState } from "react";

import { forgotPasswordAction } from "@/lib/actions/auth-actions";
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

type ForgotPasswordState = { error?: string; success?: string };

const initialState: ForgotPasswordState = {};

export function ForgotPasswordDialog() {
  const [open, setOpen] = useState(false);
  const [state, formAction, isPending] = useActionState(forgotPasswordAction, initialState);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={
          <button type="button" className="text-sm text-muted-foreground underline underline-offset-4 hover:text-foreground" />
        }
      >
        ลืมรหัสผ่าน?
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>ลืมรหัสผ่าน</DialogTitle>
          <DialogDescription>กรอกอีเมลที่ใช้สมัครสมาชิก เราจะส่งคำแนะนำในการรีเซ็ตรหัสผ่านไปให้</DialogDescription>
        </DialogHeader>

        {state.error && (
          <Alert variant="destructive">
            <AlertDescription>{state.error}</AlertDescription>
          </Alert>
        )}

        {state.success ? (
          <Alert>
            <AlertDescription>{state.success}</AlertDescription>
          </Alert>
        ) : (
          <form action={formAction} className="flex flex-col gap-3">
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="forgot-email">อีเมล</Label>
              <Input id="forgot-email" name="email" type="email" autoComplete="email" required />
            </div>
            <DialogFooter>
              <Button type="submit" disabled={isPending}>
                {isPending ? "กำลังส่ง..." : "ส่งคำแนะนำ"}
              </Button>
            </DialogFooter>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}
