"use client";

import { useState, useTransition } from "react";
import { Star } from "lucide-react";

import { Button } from "@/components/ui/button";
import { setDefaultAddressAction } from "@/lib/actions/address-actions";

export function SetDefaultAddressButton({ addressId }: { addressId: number }) {
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function handleClick() {
    setError(null);
    startTransition(async () => {
      const result = await setDefaultAddressAction(addressId);
      if (result.error) {
        setError(result.error);
      }
    });
  }

  return (
    <div className="flex flex-col items-end gap-1">
      <Button variant="outline" size="sm" onClick={handleClick} disabled={isPending}>
        <Star className="size-3.5" />
        {isPending ? "กำลังตั้งค่า..." : "ตั้งเป็นค่าเริ่มต้น"}
      </Button>
      {error && <span className="text-xs text-destructive">{error}</span>}
    </div>
  );
}
