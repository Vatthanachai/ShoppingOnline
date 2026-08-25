"use client";

import { useState } from "react";
import { Plus } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { CreatePoForm } from "@/components/admin/purchase-orders/create-po-form";
import type { Product, Vendor } from "@/lib/types";

export function CreatePoDialog({ vendors, products }: { vendors: Vendor[]; products: Product[] }) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger render={<Button />}>
        <Plus />
        สร้างใบสั่งซื้อใหม่
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>สร้างใบสั่งซื้อใหม่</DialogTitle>
        </DialogHeader>
        <CreatePoForm vendors={vendors} products={products} onSuccess={() => setOpen(false)} />
      </DialogContent>
    </Dialog>
  );
}
