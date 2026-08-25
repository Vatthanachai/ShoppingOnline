"use client";

import { useActionState, useEffect } from "react";

import { createCategoryAction, updateCategoryAction, type ActionState } from "@/lib/actions/admin-category-actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import type { Category } from "@/lib/types";

const initialState: ActionState = {};

type CategoryFormProps = {
  mode: "create" | "edit";
  initial?: Category;
  onSuccess?: () => void;
};

export function CategoryForm({ mode, initial, onSuccess }: CategoryFormProps) {
  const action = mode === "edit" ? updateCategoryAction : createCategoryAction;
  const [state, formAction, isPending] = useActionState(action, initialState);

  useEffect(() => {
    if (state !== initialState && !state.error) {
      onSuccess?.();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state]);

  return (
    <form action={formAction} className="flex flex-col gap-3">
      {mode === "edit" && initial && (
        <input type="hidden" name="product_category_id" value={initial.product_category_id} />
      )}

      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="category_name">ชื่อหมวดหมู่</Label>
        <Input id="category_name" name="category_name" type="text" defaultValue={initial?.category_name} required />
      </div>

      <div className="flex flex-col gap-1.5">
        <Label htmlFor="description">รายละเอียด</Label>
        <Input id="description" name="description" type="text" defaultValue={initial?.description} />
      </div>

      <DialogFooter>
        <Button type="submit" disabled={isPending}>
          {isPending ? "กำลังบันทึก..." : mode === "edit" ? "บันทึกการเปลี่ยนแปลง" : "เพิ่มหมวดหมู่"}
        </Button>
      </DialogFooter>
    </form>
  );
}
