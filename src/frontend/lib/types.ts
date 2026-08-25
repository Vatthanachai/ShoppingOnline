// TypeScript mirrors of the backend's JSON wire format (snake_case, see
// ServiceExtension.cs -> AddNewtonsoftJson SnakeCaseNamingStrategy).

export interface ServiceResponse<T> {
  is_success: boolean;
  data: T;
}

export interface PaginatedResponse<T> extends ServiceResponse<T[]> {
  page_index: number;
  page_limit: number;
  total_records: number;
  total_pages: number;
}

export type UserRole = "Customer" | "Admin";
export type OrderStatus = "Pending" | "Confirmed" | "Shipped" | "Delivered" | "Cancelled";

// ----- Auth -----

export interface SignUpRequest {
  name: string;
  email: string;
  phone: string;
}

export interface SignUpResult {
  user_id: number;
  email: string;
}

export interface SignInRequest {
  email: string;
  password: string;
}

export interface SignInResult {
  token: string;
  expires_at: string;
  must_change_password: boolean;
}

export interface ChangePasswordRequest {
  current_password: string;
  new_password: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

// ----- Account -----

export interface Profile {
  user_id: number;
  name: string;
  email: string;
  phone: string;
  role: UserRole;
  is_active: boolean;
  must_change_password: boolean;
}

export interface UpdateProfileRequest {
  name: string;
  phone: string;
}

export interface DeactivateAccountRequest {
  current_password: string;
}

// ----- Product categories -----

export interface Category {
  product_category_id: number;
  category_name: string;
  description: string;
  is_active: boolean;
}

export interface CategoryRequest {
  category_name: string;
  description: string;
}

// ----- Vendors -----

export interface Vendor {
  vendor_id: number;
  vendor_name: string;
  contact_person: string;
  email: string;
  phone: string;
  is_active: boolean;
}

// ----- Purchase orders (admin) -----

export type PurchaseOrderStatus = "Draft" | "Sent" | "PartiallyReceived" | "Received" | "Cancelled";

export interface PurchaseOrderItem {
  purchase_order_item_id: number;
  product_id: number;
  product_name: string;
  product_code: string;
  quantity_ordered: number;
  quantity_received: number;
  unit_cost_quoted: number | null;
}

export interface PurchaseOrderSummary {
  purchase_order_id: number;
  vendor_id: number;
  vendor_name: string;
  status: PurchaseOrderStatus;
  created_on: string;
  sent_on: string | null;
  item_count: number;
}

export interface PurchaseOrderDetail {
  purchase_order_id: number;
  vendor_id: number;
  vendor_name: string;
  vendor_email: string;
  status: PurchaseOrderStatus;
  created_on: string;
  sent_on: string | null;
  items: PurchaseOrderItem[];
}

export interface CreatePurchaseOrderItemRequest {
  product_id: number;
  quantity: number;
  unit_cost_quoted?: number;
}

export interface CreatePurchaseOrderRequest {
  vendor_id: number;
  items: CreatePurchaseOrderItemRequest[];
}

export interface ReceivePurchaseOrderLineRequest {
  purchase_order_item_id: number;
  quantity_received: number;
  unit_cost: number;
}

export interface ReceivePurchaseOrderRequest {
  lines: ReceivePurchaseOrderLineRequest[];
}

export interface VendorRequest {
  vendor_name: string;
  contact_person: string;
  email: string;
  phone: string;
}

// ----- Products -----

export interface Product {
  product_id: number;
  product_category_id: number;
  product_category_name: string;
  vendor_id: number;
  vendor_name: string;
  product_code: string;
  product_name: string;
  description: string;
  is_active: boolean;
  sell_price: number;
  tax_rate_percent: number;
  price_with_tax: number;
  available_quantity: number;
  image_path: string | null;
}

export interface ProductRequest {
  product_category_id: number;
  vendor_id: number;
  product_code: string;
  product_name: string;
  description: string;
  image_path?: string;
  sell_price: number;
  tax_rate_percent: number;
}

// ----- Stocks (read-only inventory view) -----

export interface Stock {
  stock_id: number;
  product_id: number;
  product_name: string;
  vendor_id: number;
  vendor_name: string;
  quantity: number;
  cost: number;
  received_on: string;
  purchase_order_id: number | null;
}

// ----- Users (admin) -----

export interface AdminUser {
  user_id: number;
  name: string;
  email: string;
  phone: string;
  role: UserRole;
  is_active: boolean;
  created_on: string;
}

// ----- Shipping addresses -----

export interface ShippingAddress {
  shipping_address_id: number;
  address_line1: string;
  address_line2: string;
  city: string;
  state: string;
  postal_code: string;
  country: string;
  is_default: boolean;
}

export interface ShippingAddressRequest {
  address_line1: string;
  address_line2: string;
  city: string;
  state: string;
  postal_code: string;
  country: string;
}

// ----- Orders -----

export interface OrderSummary {
  order_id: number;
  order_date: string;
  total_amount: number;
  status: OrderStatus;
}

export interface OrderItem {
  order_item_id: number;
  product_id: number;
  product_name: string;
  quantity: number;
  unit_price: number;
  tax_rate_percent: number;
  line_total: number;
}

export interface OrderDetail extends OrderSummary {
  shipping_address_line1: string;
  shipping_address_line2: string;
  shipping_city: string;
  shipping_state: string;
  shipping_postal_code: string;
  shipping_country: string;
  items: OrderItem[];
}

export interface CreateOrderItemRequest {
  product_id: number;
  quantity: number;
}

export interface CreateOrderRequest {
  shipping_address_id: number;
  items: CreateOrderItemRequest[];
}
