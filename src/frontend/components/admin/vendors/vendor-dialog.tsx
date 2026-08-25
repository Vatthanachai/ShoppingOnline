"use client";

import { useState } from "react";
import { Plus, Pencil } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { VendorForm } from "@/components/admin/vendors/vendor-form";
import type { Vendor } from "@/lib/types";

type VendorDialogProps = {
  mode: "create" | "edit";
  vendor?: Vendor;
};

export function VendorDialog({ mode, vendor }: VendorDialogProps) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={mode === "create" ? <Button /> : <Button variant="outline" size="icon-sm" aria-label="แก้ไขผู้ขาย" />}
      >
        {mode === "create" ? (
          <>
            <Plus />
            เพิ่มผู้ขายใหม่
          </>
        ) : (
          <Pencil />
        )}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{mode === "create" ? "เพิ่มผู้ขายใหม่" : "แก้ไขผู้ขาย"}</DialogTitle>
        </DialogHeader>
        <VendorForm mode={mode} initial={vendor} onSuccess={() => setOpen(false)} />
      </DialogContent>
    </Dialog>
  );
}
