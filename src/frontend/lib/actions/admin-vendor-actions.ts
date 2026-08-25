"use server";

import { revalidatePath } from "next/cache";

import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";
import type { VendorRequest } from "@/lib/types";

export type ActionState = { error?: string };

function readVendorForm(formData: FormData): VendorRequest {
  return {
    vendor_name: String(formData.get("vendor_name") ?? "").trim(),
    contact_person: String(formData.get("contact_person") ?? "").trim(),
    email: String(formData.get("email") ?? "").trim(),
    phone: String(formData.get("phone") ?? "").trim(),
  };
}

export async function createVendorAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const req = readVendorForm(formData);

  if (!req.vendor_name || !req.email) {
    return { error: "กรุณากรอกชื่อผู้ขายและอีเมลให้ครบถ้วน" };
  }

  try {
    const client = await getApiClient();
    await client.createVendor(req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/vendors");
  return {};
}

export async function updateVendorAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("vendor_id"));
  const req = readVendorForm(formData);

  if (!id) {
    return { error: "ไม่พบผู้ขายที่ต้องการแก้ไข" };
  }

  if (!req.vendor_name || !req.email) {
    return { error: "กรุณากรอกชื่อผู้ขายและอีเมลให้ครบถ้วน" };
  }

  try {
    const client = await getApiClient();
    await client.updateVendor(id, req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/vendors");
  return {};
}

export async function deactivateVendorAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("vendor_id"));

  if (!id) {
    return { error: "ไม่พบผู้ขายที่ต้องการปิดใช้งาน" };
  }

  try {
    const client = await getApiClient();
    await client.deactivateVendor(id);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/vendors");
  return {};
}
