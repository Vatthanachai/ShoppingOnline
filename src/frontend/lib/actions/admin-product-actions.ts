"use server";

import { revalidatePath } from "next/cache";

import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";
import type { ProductRequest } from "@/lib/types";

export type ActionState = { error?: string };

function readProductForm(formData: FormData): ProductRequest {
  const imagePath = String(formData.get("image_path") ?? "").trim();
  return {
    product_category_id: Number(formData.get("product_category_id")),
    vendor_id: Number(formData.get("vendor_id")),
    product_code: String(formData.get("product_code") ?? "").trim(),
    product_name: String(formData.get("product_name") ?? "").trim(),
    description: String(formData.get("description") ?? "").trim(),
    image_path: imagePath || undefined,
    sell_price: Number(formData.get("sell_price")),
    tax_rate_percent: Number(formData.get("tax_rate_percent")),
  };
}

export async function createProductAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const req = readProductForm(formData);

  if (!req.product_category_id || !req.vendor_id || !req.product_code || !req.product_name) {
    return { error: "กรุณากรอกข้อมูลสินค้าให้ครบถ้วน" };
  }

  try {
    const client = await getApiClient();
    await client.createProduct(req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/products");
  return {};
}

export async function updateProductAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("product_id"));
  const req = readProductForm(formData);

  if (!id) {
    return { error: "ไม่พบสินค้าที่ต้องการแก้ไข" };
  }

  if (!req.product_category_id || !req.vendor_id || !req.product_code || !req.product_name) {
    return { error: "กรุณากรอกข้อมูลสินค้าให้ครบถ้วน" };
  }

  try {
    const client = await getApiClient();
    await client.updateProduct(id, req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/products");
  return {};
}

export async function deactivateProductAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("product_id"));

  if (!id) {
    return { error: "ไม่พบสินค้าที่ต้องการปิดใช้งาน" };
  }

  try {
    const client = await getApiClient();
    await client.deactivateProduct(id);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/products");
  return {};
}
