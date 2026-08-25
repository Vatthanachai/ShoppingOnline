"use server";

import { revalidatePath } from "next/cache";

import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";
import type { ShippingAddressRequest } from "@/lib/types";

export type ActionState = { error?: string };

function readAddressForm(formData: FormData): ShippingAddressRequest {
  return {
    address_line1: String(formData.get("address_line1") ?? "").trim(),
    address_line2: String(formData.get("address_line2") ?? "").trim(),
    city: String(formData.get("city") ?? "").trim(),
    state: String(formData.get("state") ?? "").trim(),
    postal_code: String(formData.get("postal_code") ?? "").trim(),
    country: String(formData.get("country") ?? "").trim(),
  };
}

export async function createAddressAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const req = readAddressForm(formData);

  if (!req.address_line1 || !req.city || !req.postal_code || !req.country) {
    return { error: "กรุณากรอกข้อมูลที่อยู่ให้ครบถ้วน" };
  }

  try {
    const client = await getApiClient();
    await client.createAddress(req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/account/addresses");
  return {};
}

export async function updateAddressAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("shipping_address_id"));
  const req = readAddressForm(formData);

  if (!id) {
    return { error: "ไม่พบที่อยู่ที่ต้องการแก้ไข" };
  }

  if (!req.address_line1 || !req.city || !req.postal_code || !req.country) {
    return { error: "กรุณากรอกข้อมูลที่อยู่ให้ครบถ้วน" };
  }

  try {
    const client = await getApiClient();
    await client.updateAddress(id, req);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/account/addresses");
  return {};
}

export async function deleteAddressAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const id = Number(formData.get("shipping_address_id"));

  if (!id) {
    return { error: "ไม่พบที่อยู่ที่ต้องการลบ" };
  }

  try {
    const client = await getApiClient();
    await client.deleteAddress(id);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/account/addresses");
  return {};
}

export async function setDefaultAddressAction(addressId: number): Promise<ActionState> {
  try {
    const client = await getApiClient();
    await client.setDefaultAddress(addressId);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/account/addresses");
  revalidatePath("/checkout");
  return {};
}
