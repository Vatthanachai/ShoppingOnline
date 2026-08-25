"use client";

import { useActionState } from "react";
import Link from "next/link";

import { signUpAction, type ActionState } from "@/lib/actions/auth-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";

const initialState: ActionState = {};

export function SignUpForm() {
  const [state, formAction, isPending] = useActionState(signUpAction, initialState);

  return (
    <form action={formAction} className="flex flex-col gap-4">
      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="name">ชื่อ-นามสกุล</Label>
        <Input id="name" name="name" type="text" autoComplete="name" required />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="email">อีเมล</Label>
        <Input id="email" name="email" type="email" autoComplete="email" required />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="phone">เบอร์โทรศัพท์</Label>
        <Input id="phone" name="phone" type="tel" autoComplete="tel" required />
      </div>

      <Button type="submit" disabled={isPending} className="mt-2">
        {isPending ? "กำลังสร้างบัญชี..." : "สมัครสมาชิก"}
      </Button>

      <p className="text-center text-sm text-muted-foreground">
        มีบัญชีอยู่แล้ว?{" "}
        <Link href="/sign-in" className="text-foreground underline underline-offset-4">
          เข้าสู่ระบบ
        </Link>
      </p>
    </form>
  );
}
