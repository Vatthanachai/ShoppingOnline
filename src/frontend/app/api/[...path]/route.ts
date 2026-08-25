import { NextResponse, type NextRequest } from "next/server";

import { getToken } from "@/lib/session";

const BASE_URL = process.env.API_URL ?? "http://localhost:5018";

/**
 * Generic reverse proxy for the ShoppingOnline backend.
 *
 * Most pages don't need this - Server Components and Server Actions already call the
 * backend directly via `lib/api-client.ts` (see `lib/session.ts#getApiClient`), which is
 * the primary way this app talks to the API. This route exists for cases that need a
 * same-origin `/api/*` URL to fetch from the browser (e.g. a future client component that
 * wants to call the API directly): it forwards the request server-side, attaching the
 * signed-in user's token as a Bearer header, so the backend's real address and the raw
 * token are never exposed to the browser.
 */
async function proxy(request: NextRequest, { params }: { params: Promise<{ path: string[] }> }) {
  const { path } = await params;
  const token = await getToken();

  const url = new URL(`/api/${path.join("/")}`, BASE_URL);
  url.search = request.nextUrl.search;

  const headers = new Headers();
  const contentType = request.headers.get("content-type");
  if (contentType) headers.set("content-type", contentType);
  if (token) headers.set("authorization", `Bearer ${token}`);

  const hasBody = request.method !== "GET" && request.method !== "HEAD";

  const response = await fetch(url, {
    method: request.method,
    headers,
    body: hasBody ? await request.text() : undefined,
    cache: "no-store",
  });

  const body = await response.text();
  return new NextResponse(body, {
    status: response.status,
    headers: { "content-type": response.headers.get("content-type") ?? "application/json" },
  });
}

export {
  proxy as GET,
  proxy as POST,
  proxy as PUT,
  proxy as DELETE,
  proxy as PATCH,
};
