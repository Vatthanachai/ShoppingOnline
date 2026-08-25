"use server";

import { revalidatePath } from "next/cache";

import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";
import type { CategoryRequest } from "@/lib/types";

export type ActionState = { error?: string };

function readCategoryForm(formData: FormData): CategoryRequest {
  return {
    category_name: String(formData.get("category_name") ?? "").trim(),
    description: String(formData.get("description") ?? "").trim(),
  };
}

export async function createCategoryAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const req = readCategoryForm(formData);

  if (!req.category_name) {
    return { error: "กรุณากรอกชื่อหมวดหมู่" };
  }

  try {
    const client = await getApiClient();
    await client.createCategory(req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/categories");
  return {};
}

export async function updateCategoryAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("product_category_id"));
  const req = readCategoryForm(formData);

  if (!id) {
    return { error: "ไม่พบหมวดหมู่ที่ต้องการแก้ไข" };
  }

  if (!req.category_name) {
    return { error: "กรุณากรอกชื่อหมวดหมู่" };
  }

  try {
    const client = await getApiClient();
    await client.updateCategory(id, req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/categories");
  return {};
}

export async function deactivateCategoryAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("product_category_id"));

  if (!id) {
    return { error: "ไม่พบหมวดหมู่ที่ต้องการปิดใช้งาน" };
  }

  try {
    const client = await getApiClient();
    await client.deactivateCategory(id);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/categories");
  return {};
}
