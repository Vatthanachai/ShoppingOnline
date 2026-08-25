"use server";

import { revalidatePath } from "next/cache";

import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";
import type { CreateOrderItemRequest, OrderDetail } from "@/lib/types";

export type CreateOrderState =
  | { ok: true; order: OrderDetail }
  | { ok: false; error: string };

export type CancelOrderState = { error?: string };

/**
 * Places an order from the client-side cart. Invoked directly (not via <form action>)
 * since the payload is cart state held in localStorage, not form fields.
 */
export async function createOrderAction(
  shippingAddressId: number,
  items: CreateOrderItemRequest[],
): Promise<CreateOrderState> {
  if (items.length === 0) {
    return { ok: false, error: "ตะกร้าสินค้าว่างเปล่า" };
  }

  if (!shippingAddressId) {
    return { ok: false, error: "กรุณาเลือกที่อยู่จัดส่ง" };
  }

  try {
    const client = await getApiClient();
    const order = await client.createOrder({ shipping_address_id: shippingAddressId, items });
    return { ok: true, order };
  } catch (error) {
    if (error instanceof ApiError) {
      return { ok: false, error: error.message };
    }
    return { ok: false, error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }
}

/** Cancels an order (only valid while it's still Pending; backend enforces this too). */
export async function cancelOrderAction(orderId: number): Promise<CancelOrderState> {
  try {
    const client = await getApiClient();
    await client.cancelOrder(orderId);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath(`/orders/${orderId}`);
  revalidatePath("/orders");
  return {};
}
