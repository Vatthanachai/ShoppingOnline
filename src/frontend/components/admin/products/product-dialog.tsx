"use client";

import { useState } from "react";
import { Plus, Pencil } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { ProductForm } from "@/components/admin/products/product-form";
import type { Category, Product, Vendor } from "@/lib/types";

type ProductDialogProps = {
  mode: "create" | "edit";
  product?: Product;
  categories: Category[];
  vendors: Vendor[];
};

export function ProductDialog({ mode, product, categories, vendors }: ProductDialogProps) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={mode === "create" ? <Button /> : <Button variant="outline" size="icon-sm" aria-label="แก้ไขสินค้า" />}
      >
        {mode === "create" ? (
          <>
            <Plus />
            เพิ่มสินค้าใหม่
          </>
        ) : (
          <Pencil />
        )}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{mode === "create" ? "เพิ่มสินค้าใหม่" : "แก้ไขสินค้า"}</DialogTitle>
        </DialogHeader>
        <ProductForm
          mode={mode}
          initial={product}
          categories={categories}
          vendors={vendors}
          onSuccess={() => setOpen(false)}
        />
      </DialogContent>
    </Dialog>
  );
}
