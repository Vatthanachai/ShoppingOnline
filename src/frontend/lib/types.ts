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
  min_price: number | null;
  image_path: string | null;
}

export interface ProductRequest {
  product_category_id: number;
  vendor_id: number;
  product_code: string;
  product_name: string;
  description: string;
  image_path?: string;
}

// ----- Stocks -----

export interface Stock {
  stock_id: number;
  product_id: number;
  product_name: string;
  vendor_id: number;
  vendor_name: string;
  quantity: number;
  price: number;
}

export interface CreateStockRequest {
  product_id: number;
  vendor_id: number;
  quantity: number;
  price: number;
}

export interface UpdateStockRequest {
  quantity: number;
  price: number;
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
  vendor_id: number;
  vendor_name: string;
  quantity: number;
  price: number;
}

export interface OrderDetail extends OrderSummary {
  items: OrderItem[];
}

export interface CreateOrderItemRequest {
  product_id: number;
  vendor_id: number;
  quantity: number;
}

export interface CreateOrderRequest {
  items: CreateOrderItemRequest[];
}
