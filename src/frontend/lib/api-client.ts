import "server-only";

import type {
  AdminUser,
  Category,
  CategoryRequest,
  ChangePasswordRequest,
  CreateOrderRequest,
  CreatePurchaseOrderRequest,
  DeactivateAccountRequest,
  ForgotPasswordRequest,
  OrderDetail,
  OrderSummary,
  PaginatedResponse,
  Product,
  ProductRequest,
  Profile,
  PurchaseOrderDetail,
  PurchaseOrderStatus,
  PurchaseOrderSummary,
  ReceivePurchaseOrderRequest,
  ServiceResponse,
  ShippingAddress,
  ShippingAddressRequest,
  SignInRequest,
  SignInResult,
  SignUpRequest,
  SignUpResult,
  Stock,
  UpdateProfileRequest,
  Vendor,
  VendorRequest,
} from "@/lib/types";

const BASE_URL = process.env.API_URL ?? "http://localhost:5018";

/** Thrown for any non-2xx response. Carries the HTTP status and the backend's message, if any. */
export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

type Query = Record<string, string | number | boolean | undefined>;

function toQueryString(query?: Query): string {
  if (!query) return "";
  const params = new URLSearchParams();
  for (const [key, value] of Object.entries(query)) {
    if (value !== undefined) params.set(key, String(value));
  }
  const qs = params.toString();
  return qs ? `?${qs}` : "";
}

/**
 * A thin, typed wrapper around the ShoppingOnline backend API.
 * Server-only: runs inside Server Components / Server Actions and forwards the
 * caller's Paseto token (read from the httpOnly session cookie) as a Bearer header.
 */
export class ApiClient {
  constructor(private readonly token?: string) {}

  private async send(
    path: string,
    options: { method?: string; body?: unknown; query?: Query } = {},
  ): Promise<unknown> {
    const { method = "GET", body, query } = options;

    const headers: HeadersInit = { "Content-Type": "application/json" };
    if (this.token) headers["Authorization"] = `Bearer ${this.token}`;

    const url = `${BASE_URL}${path}${toQueryString(query)}`;
    const response = await fetch(url, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined,
      cache: "no-store",
    });

    const text = await response.text();
    let json: Record<string, unknown> | undefined;
    try {
      json = text ? JSON.parse(text) : undefined;
    } catch {
      json = undefined;
    }

    if (!response.ok) {
      // Logged server-side (this file only runs on the Next.js server, see "server-only"
      // above) so the real backend response is visible in the dev terminal, not just a
      // generic "Request failed with status N" in the browser.
      console.error(`[ApiClient] ${method} ${url} -> ${response.status}\n${text}`);

      const message =
        (json?.message as string | undefined) ??
        (json?.title as string | undefined) ??
        (text ? text.slice(0, 500) : `Request failed with status ${response.status}`);
      throw new ApiError(response.status, message);
    }

    return json;
  }

  /** Unwraps a single-item ServiceResponse<T> into T. */
  private async request<T>(path: string, options: { method?: string; body?: unknown; query?: Query } = {}) {
    const json = await this.send(path, options);
    return (json as ServiceResponse<T>).data;
  }

  /** Returns the raw paginated envelope (data + page metadata). */
  private async paginated<T>(path: string, query?: Query) {
    const json = await this.send(path, { query });
    return json as PaginatedResponse<T>;
  }

  // ----- Auth -----

  signUp(req: SignUpRequest) {
    return this.request<SignUpResult>("/api/scores/sign_up", { method: "POST", body: req });
  }

  signIn(req: SignInRequest) {
    return this.request<SignInResult>("/api/scores/sign_in", { method: "POST", body: req });
  }

  changePassword(req: ChangePasswordRequest) {
    return this.request<string>("/api/scores/change_password", { method: "POST", body: req });
  }

  forgotPassword(req: ForgotPasswordRequest) {
    return this.request<string>("/api/scores/forgot_password", { method: "POST", body: req });
  }

  // ----- Account -----

  getProfile() {
    return this.request<Profile>("/api/account/profile");
  }

  updateProfile(req: UpdateProfileRequest) {
    return this.request<Profile>("/api/account/profile", { method: "PUT", body: req });
  }

  deactivateAccount(req: DeactivateAccountRequest) {
    return this.request<string>("/api/account/deactivate", { method: "POST", body: req });
  }

  // ----- Categories -----

  getCategoriesPage(query?: {
    search?: string;
    include_inactive?: boolean;
    page_index?: number;
    page_limit?: number;
  }) {
    return this.paginated<Category>("/api/product_categories", {
      search: query?.search,
      includeInactive: query?.include_inactive,
      page_index: query?.page_index,
      page_limit: query?.page_limit,
    });
  }

  getCategory(id: number) {
    return this.request<Category>(`/api/product_categories/${id}`);
  }

  createCategory(req: CategoryRequest) {
    return this.request<Category>("/api/product_categories", { method: "POST", body: req });
  }

  updateCategory(id: number, req: CategoryRequest) {
    return this.request<Category>(`/api/product_categories/${id}`, { method: "PUT", body: req });
  }

  deactivateCategory(id: number) {
    return this.request<string>(`/api/product_categories/${id}`, { method: "DELETE" });
  }

  // ----- Vendors -----

  getVendorsPage(query?: {
    search?: string;
    include_inactive?: boolean;
    page_index?: number;
    page_limit?: number;
  }) {
    return this.paginated<Vendor>("/api/vendors", {
      search: query?.search,
      includeInactive: query?.include_inactive,
      page_index: query?.page_index,
      page_limit: query?.page_limit,
    });
  }

  getVendor(id: number) {
    return this.request<Vendor>(`/api/vendors/${id}`);
  }

  createVendor(req: VendorRequest) {
    return this.request<Vendor>("/api/vendors", { method: "POST", body: req });
  }

  updateVendor(id: number, req: VendorRequest) {
    return this.request<Vendor>(`/api/vendors/${id}`, { method: "PUT", body: req });
  }

  deactivateVendor(id: number) {
    return this.request<string>(`/api/vendors/${id}`, { method: "DELETE" });
  }

  // ----- Products -----

  getProductsPage(query?: {
    search?: string;
    product_category_id?: number;
    vendor_id?: number;
    include_inactive?: boolean;
    page_index?: number;
    page_limit?: number;
  }) {
    // NOTE: ASP.NET's default query-string binder matches bare property names
    // case-insensitively but does NOT understand snake_case. Only page_index /
    // page_limit / is_order_descending have an explicit [FromQuery(Name=...)]
    // on PagingRequest, so those stay snake_case; multi-word filters must be
    // sent without underscores (camelCase here) to actually bind server-side.
    return this.paginated<Product>("/api/products", {
      search: query?.search,
      productCategoryId: query?.product_category_id,
      vendorId: query?.vendor_id,
      includeInactive: query?.include_inactive,
      page_index: query?.page_index,
      page_limit: query?.page_limit,
    });
  }

  getProduct(id: number) {
    return this.request<Product>(`/api/products/${id}`);
  }

  createProduct(req: ProductRequest) {
    return this.request<Product>("/api/products", { method: "POST", body: req });
  }

  updateProduct(id: number, req: ProductRequest) {
    return this.request<Product>(`/api/products/${id}`, { method: "PUT", body: req });
  }

  deactivateProduct(id: number) {
    return this.request<string>(`/api/products/${id}`, { method: "DELETE" });
  }

  // ----- Stocks (read-only inventory view; lots are created only via received Purchase Orders) -----

  getStocksPage(query?: { product_id?: number; vendor_id?: number; page_index?: number; page_limit?: number }) {
    // See getProductsPage note above re: snake_case vs. bare-property query binding.
    return this.paginated<Stock>("/api/stocks", {
      productId: query?.product_id,
      vendorId: query?.vendor_id,
      page_index: query?.page_index,
      page_limit: query?.page_limit,
    });
  }

  // ----- Shipping addresses -----

  getAddressesPage(query?: { page_index?: number; page_limit?: number }) {
    return this.paginated<ShippingAddress>("/api/shipping_addresses", query);
  }

  createAddress(req: ShippingAddressRequest) {
    return this.request<ShippingAddress>("/api/shipping_addresses", { method: "POST", body: req });
  }

  updateAddress(id: number, req: ShippingAddressRequest) {
    return this.request<ShippingAddress>(`/api/shipping_addresses/${id}`, { method: "PUT", body: req });
  }

  deleteAddress(id: number) {
    return this.request<string>(`/api/shipping_addresses/${id}`, { method: "DELETE" });
  }

  // ----- Orders -----

  getOrdersPage(query?: { page_index?: number; page_limit?: number }) {
    return this.paginated<OrderSummary>("/api/orders", query);
  }

  getOrder(id: number) {
    return this.request<OrderDetail>(`/api/orders/${id}`);
  }

  createOrder(req: CreateOrderRequest) {
    return this.request<OrderDetail>("/api/orders", { method: "POST", body: req });
  }

  cancelOrder(id: number) {
    return this.request<string>(`/api/orders/${id}/cancel`, { method: "POST" });
  }

  // ----- Users (admin) -----

  getUsersPage(query?: { search?: string; page_index?: number; page_limit?: number }) {
    return this.paginated<AdminUser>("/api/users", query);
  }

  activateUser(id: number) {
    return this.request<string>(`/api/users/${id}/activate`, { method: "POST" });
  }

  deactivateUser(id: number) {
    return this.request<string>(`/api/users/${id}/deactivate`, { method: "POST" });
  }

  // ----- Purchase orders (admin) -----

  getPurchaseOrdersPage(query?: {
    vendor_id?: number;
    status?: PurchaseOrderStatus;
    page_index?: number;
    page_limit?: number;
  }) {
    return this.paginated<PurchaseOrderSummary>("/api/purchase_orders", {
      vendorId: query?.vendor_id,
      status: query?.status,
      page_index: query?.page_index,
      page_limit: query?.page_limit,
    });
  }

  getPurchaseOrder(id: number) {
    return this.request<PurchaseOrderDetail>(`/api/purchase_orders/${id}`);
  }

  createPurchaseOrder(req: CreatePurchaseOrderRequest) {
    return this.request<PurchaseOrderDetail>("/api/purchase_orders", { method: "POST", body: req });
  }

  sendPurchaseOrder(id: number) {
    return this.request<PurchaseOrderDetail>(`/api/purchase_orders/${id}/send`, { method: "POST" });
  }

  receivePurchaseOrder(id: number, req: ReceivePurchaseOrderRequest) {
    return this.request<PurchaseOrderDetail>(`/api/purchase_orders/${id}/receive`, { method: "POST", body: req });
  }
}

export function createApiClient(token?: string) {
  return new ApiClient(token);
}
