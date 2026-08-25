"use server";

import { revalidatePath } from "next/cache";

import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";
import type { CreatePurchaseOrderItemRequest, ReceivePurchaseOrderLineRequest } from "@/lib/types";

export type ActionState = { error?: string };

export async function createPurchaseOrderAction(
  vendorId: number,
  items: CreatePurchaseOrderItemRequest[],
): Promise<ActionState & { purchaseOrderId?: number }> {
  if (!vendorId) {
    return { error: "กรุณาเลือกผู้ขาย" };
  }
  if (items.length === 0) {
    return { error: "กรุณาเพิ่มรายการสินค้าอย่างน้อย 1 รายการ" };
  }

  try {
    const client = await getApiClient();
    const po = await client.createPurchaseOrder({ vendor_id: vendorId, items });
    revalidatePath("/admin/purchase-orders");
    return { purchaseOrderId: po.purchase_order_id };
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }
}

export async function sendPurchaseOrderAction(purchaseOrderId: number): Promise<ActionState> {
  try {
    const client = await getApiClient();
    await client.sendPurchaseOrder(purchaseOrderId);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath(`/admin/purchase-orders/${purchaseOrderId}`);
  revalidatePath("/admin/purchase-orders");
  return {};
}

export async function receivePurchaseOrderAction(
  purchaseOrderId: number,
  lines: ReceivePurchaseOrderLineRequest[],
): Promise<ActionState> {
  if (lines.length === 0) {
    return { error: "กรุณากรอกจำนวนที่ได้รับอย่างน้อย 1 รายการ" };
  }

  try {
    const client = await getApiClient();
    await client.receivePurchaseOrder(purchaseOrderId, { lines });
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath(`/admin/purchase-orders/${purchaseOrderId}`);
  revalidatePath("/admin/purchase-orders");
  revalidatePath("/admin/stocks");
  return {};
}
