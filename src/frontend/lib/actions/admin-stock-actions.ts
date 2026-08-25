"use server";

import { revalidatePath } from "next/cache";

import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";

export type ActionState = { error?: string };

export async function createStockAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const req = {
    product_id: Number(formData.get("product_id")),
    vendor_id: Number(formData.get("vendor_id")),
    quantity: Number(formData.get("quantity")),
    price: Number(formData.get("price")),
  };

  if (!req.product_id || !req.vendor_id || req.quantity < 0 || req.price < 0) {
    return { error: "กรุณากรอกข้อมูลสต็อกให้ครบถ้วนและถูกต้อง" };
  }

  try {
    const client = await getApiClient();
    await client.createStock(req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/stocks");
  return {};
}

export async function updateStockAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("stock_id"));
  const req = {
    quantity: Number(formData.get("quantity")),
    price: Number(formData.get("price")),
  };

  if (!id) {
    return { error: "ไม่พบสต็อกที่ต้องการแก้ไข" };
  }

  if (req.quantity < 0 || req.price < 0) {
    return { error: "กรุณากรอกจำนวนและราคาให้ถูกต้อง" };
  }

  try {
    const client = await getApiClient();
    await client.updateStock(id, req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/stocks");
  return {};
}

export async function deleteStockAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("stock_id"));

  if (!id) {
    return { error: "ไม่พบสต็อกที่ต้องการลบ" };
  }

  try {
    const client = await getApiClient();
    await client.deleteStock(id);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/stocks");
  return {};
}
