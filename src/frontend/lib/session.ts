import "server-only";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";

import { ApiError, createApiClient, type ApiClient } from "@/lib/api-client";

const TOKEN_COOKIE = "session_token";
const MUST_CHANGE_PASSWORD_COOKIE = "must_change_password";

/** Stores the Paseto token in an httpOnly cookie right after sign-up/sign-in. */
export async function createSession(token: string, expiresAt: string, mustChangePassword: boolean) {
  const store = await cookies();
  const expires = new Date(expiresAt);

  store.set(TOKEN_COOKIE, token, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    path: "/",
    expires,
  });

  if (mustChangePassword) {
    store.set(MUST_CHANGE_PASSWORD_COOKIE, "1", {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      expires,
    });
  } else {
    store.delete(MUST_CHANGE_PASSWORD_COOKIE);
  }
}

/** Clears the session, e.g. after a successful password change or explicit sign-out. */
export async function clearMustChangePassword() {
  const store = await cookies();
  store.delete(MUST_CHANGE_PASSWORD_COOKIE);
}

export async function destroySession() {
  const store = await cookies();
  store.delete(TOKEN_COOKIE);
  store.delete(MUST_CHANGE_PASSWORD_COOKIE);
}

export async function getToken(): Promise<string | undefined> {
  const store = await cookies();
  return store.get(TOKEN_COOKIE)?.value;
}

export async function mustChangePassword(): Promise<boolean> {
  const store = await cookies();
  return store.get(MUST_CHANGE_PASSWORD_COOKIE)?.value === "1";
}

/** Builds an ApiClient bound to the current visitor's session token, if signed in. */
export async function getApiClient() {
  const token = await getToken();
  return createApiClient(token);
}

/**
 * For Server Components behind the proxy's auth guard (which only checks that the session
 * cookie is *present*, not that the token is still valid - e.g. it may have expired, or the
 * server's signing key changed). Runs `fn` with an authenticated client and, if the backend
 * rejects the token with 401, redirects to sign-in instead of letting the ApiError crash the
 * page render.
 */
export async function withAuthRedirect<T>(fn: (client: ApiClient) => Promise<T>): Promise<T> {
  const client = await getApiClient();
  try {
    return await fn(client);
  } catch (error) {
    if (error instanceof ApiError && error.status === 401) {
      redirect("/sign-in");
    }
    throw error;
  }
}

/** Fetches the current user's profile, or null when signed out. */
export async function getCurrentProfile() {
  const token = await getToken();
  if (!token) return null;

  try {
    return await createApiClient(token).getProfile();
  } catch {
    return null;
  }
}
