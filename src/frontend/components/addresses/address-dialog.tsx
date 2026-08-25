"use client";

import { useState } from "react";
import { Plus, Pencil } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { AddressForm } from "@/components/addresses/address-form";
import type { ShippingAddress } from "@/lib/types";

type AddressDialogProps = {
  mode: "create" | "edit";
  address?: ShippingAddress;
};

export function AddressDialog({ mode, address }: AddressDialogProps) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={mode === "create" ? <Button /> : <Button variant="outline" size="icon-sm" aria-label="แก้ไขที่อยู่" />}
      >
        {mode === "create" ? (
          <>
            <Plus />
            เพิ่มที่อยู่ใหม่
          </>
        ) : (
          <Pencil />
        )}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{mode === "create" ? "เพิ่มที่อยู่ใหม่" : "แก้ไขที่อยู่"}</DialogTitle>
        </DialogHeader>
        <AddressForm mode={mode} initial={address} onSuccess={() => setOpen(false)} />
      </DialogContent>
    </Dialog>
  );
}
