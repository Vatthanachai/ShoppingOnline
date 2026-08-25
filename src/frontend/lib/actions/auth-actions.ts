"use server";

import { redirect } from "next/navigation";

import { ApiError, createApiClient } from "@/lib/api-client";
import { createSession } from "@/lib/session";

export type ActionState = { error?: string };

/** Returns a same-origin relative path to redirect to after sign-in, defaulting to /account. */
function resolveNextPath(next: FormDataEntryValue | null): string {
  if (typeof next === "string" && next.startsWith("/") && !next.startsWith("//")) {
    return next;
  }
  return "/account";
}

export async function signUpAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const name = String(formData.get("name") ?? "").trim();
  const email = String(formData.get("email") ?? "").trim();
  const phone = String(formData.get("phone") ?? "").trim();

  if (!name || !email || !phone) {
    return { error: "กรุณากรอกข้อมูลให้ครบถ้วน" };
  }

  try {
    const client = createApiClient();
    await client.signUp({ name, email, phone });
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  redirect("/sign-in?signed_up=1");
}

export async function signInAction(_prevState: ActionState, formData: FormData): Promise<ActionState> {
  const email = String(formData.get("email") ?? "").trim();
  const password = String(formData.get("password") ?? "");
  const next = resolveNextPath(formData.get("next"));

  if (!email || !password) {
    return { error: "กรุณากรอกอีเมลและรหัสผ่าน" };
  }

  let result;
  try {
    const client = createApiClient();
    result = await client.signIn({ email, password });
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.status === 401 ? "อีเมลหรือรหัสผ่านไม่ถูกต้อง" : error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }

  await createSession(result.token, result.expires_at, result.must_change_password);

  redirect(result.must_change_password ? "/change-password" : next);
}

export async function forgotPasswordAction(_prevState: ActionState, formData: FormData): Promise<ActionState & { success?: string }> {
  const email = String(formData.get("email") ?? "").trim();

  if (!email) {
    return { error: "กรุณากรอกอีเมล" };
  }

  try {
    const client = createApiClient();
    const message = await client.forgotPassword({ email });
    return { success: message || "หากอีเมลนี้มีอยู่ในระบบ เราได้ส่งคำแนะนำในการรีเซ็ตรหัสผ่านไปให้แล้ว" };
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง" };
  }
}
