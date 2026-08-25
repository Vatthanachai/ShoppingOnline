"use server";

import { redirect } from "next/navigation";

import { ApiError } from "@/lib/api-client";
import { clearMustChangePassword, destroySession, getApiClient } from "@/lib/session";

export type ActionState = { error?: string };

export async function changePasswordAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const currentPassword = String(formData.get("current_password") ?? "");
  const newPassword = String(formData.get("new_password") ?? "");
  const confirmPassword = String(formData.get("confirm_password") ?? "");

  if (!currentPassword || !newPassword) {
    return { error: "กรุณากรอกข้อมูลให้ครบถ้วน" };
  }

  if (newPassword !== confirmPassword) {
    return { error: "รหัสผ่านใหม่และการยืนยันรหัสผ่านไม่ตรงกัน" };
  }

  try {
    const client = await getApiClient();
    await client.changePassword({ current_password: currentPassword, new_password: newPassword });
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  await clearMustChangePassword();
  redirect("/account");
}

export async function updateProfileAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const name = String(formData.get("name") ?? "").trim();
  const phone = String(formData.get("phone") ?? "").trim();

  if (!name || !phone) {
    return { error: "กรุณากรอกข้อมูลให้ครบถ้วน" };
  }

  try {
    const client = await getApiClient();
    await client.updateProfile({ name, phone });
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  redirect("/account");
}

export async function deactivateAccountAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const currentPassword = String(formData.get("current_password") ?? "");

  if (!currentPassword) {
    return { error: "กรุณากรอกรหัสผ่านปัจจุบัน" };
  }

  try {
    const client = await getApiClient();
    await client.deactivateAccount({ current_password: currentPassword });
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  await destroySession();
  redirect("/?deactivated=1");
}

export async function signOutAction(): Promise<void> {
  await destroySession();
  redirect("/");
}
