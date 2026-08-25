"use client";

import { useState } from "react";
import { Plus, Pencil } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { StockCreateForm } from "@/components/admin/stocks/stock-create-form";
import { StockEditForm } from "@/components/admin/stocks/stock-edit-form";
import type { Product, Stock, Vendor } from "@/lib/types";

type StockDialogProps =
  | { mode: "create"; products: Product[]; vendors: Vendor[] }
  | { mode: "edit"; stock: Stock };

export function StockDialog(props: StockDialogProps) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={
          props.mode === "create" ? <Button /> : <Button variant="outline" size="icon-sm" aria-label="แก้ไขสต็อก" />
        }
      >
        {props.mode === "create" ? (
          <>
            <Plus />
            เพิ่มสต็อกใหม่
          </>
        ) : (
          <Pencil />
        )}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{props.mode === "create" ? "เพิ่มสต็อกใหม่" : "แก้ไขสต็อก"}</DialogTitle>
        </DialogHeader>
        {props.mode === "create" ? (
          <StockCreateForm products={props.products} vendors={props.vendors} onSuccess={() => setOpen(false)} />
        ) : (
          <StockEditForm stock={props.stock} onSuccess={() => setOpen(false)} />
        )}
      </DialogContent>
    </Dialog>
  );
}
