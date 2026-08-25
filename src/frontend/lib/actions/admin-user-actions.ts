"use server";

import { revalidatePath } from "next/cache";

import { ApiError } from "@/lib/api-client";
import { getApiClient } from "@/lib/session";

export type ActionState = { error?: string };

export async function setUserActiveAction(userId: number, active: boolean): Promise<ActionState> {
  try {
    const client = await getApiClient();
    if (active) {
      await client.activateUser(userId);
    } else {
      await client.deactivateUser(userId);
    }
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  revalidatePath("/admin/users");
  return {};
}
