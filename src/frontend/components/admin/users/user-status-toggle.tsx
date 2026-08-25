"use client";

import { useState, useTransition } from "react";

import { setUserActiveAction } from "@/lib/actions/admin-user-actions";
import { Switch } from "@/components/ui/switch";

export function UserStatusToggle({ userId, isActive }: { userId: number; isActive: boolean }) {
  const [checked, setChecked] = useState(isActive);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function handleChange(next: boolean) {
    setError(null);
    setChecked(next);
    startTransition(async () => {
      const result = await setUserActiveAction(userId, next);
      if (result.error) {
        setChecked(!next);
        setError(result.error);
      }
    });
  }

  return (
    <div className="flex flex-col items-end gap-1">
      <Switch checked={checked} onCheckedChange={handleChange} disabled={isPending} aria-label="สถานะผู้ใช้" />
      {error && <span className="text-xs text-destructive">{error}</span>}
    </div>
  );
}
