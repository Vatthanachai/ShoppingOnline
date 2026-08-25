"use client";

import { useState } from "react";
import { Plus, Pencil } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { CategoryForm } from "@/components/admin/categories/category-form";
import type { Category } from "@/lib/types";

type CategoryDialogProps = {
  mode: "create" | "edit";
  category?: Category;
};

export function CategoryDialog({ mode, category }: CategoryDialogProps) {
  const [open, setOpen] = useState(false);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={mode === "create" ? <Button /> : <Button variant="outline" size="icon-sm" aria-label="แก้ไขหมวดหมู่" />}
      >
        {mode === "create" ? (
          <>
            <Plus />
            เพิ่มหมวดหมู่ใหม่
          </>
        ) : (
          <Pencil />
        )}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{mode === "create" ? "เพิ่มหมวดหมู่ใหม่" : "แก้ไขหมวดหมู่"}</DialogTitle>
        </DialogHeader>
        <CategoryForm mode={mode} initial={category} onSuccess={() => setOpen(false)} />
      </DialogContent>
    </Dialog>
  );
}
