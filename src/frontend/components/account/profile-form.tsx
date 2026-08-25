"use client";

import { useActionState } from "react";

import { updateProfileAction, type ActionState } from "@/lib/actions/account-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import type { Profile } from "@/lib/types";

const initialState: ActionState = {};

export function ProfileForm({ profile }: { profile: Profile }) {
  const [state, formAction, isPending] = useActionState(updateProfileAction, initialState);

  return (
    <form action={formAction} className="flex flex-col gap-4">
      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="name">ชื่อ-นามสกุล</Label>
        <Input id="name" name="name" type="text" defaultValue={profile.name} required />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="phone">เบอร์โทรศัพท์</Label>
        <Input id="phone" name="phone" type="tel" defaultValue={profile.phone} required />
      </div>

      <div>
        <Button type="submit" disabled={isPending}>
          {isPending ? "กำลังบันทึก..." : "บันทึกการเปลี่ยนแปลง"}
        </Button>
      </div>
    </form>
  );
}
