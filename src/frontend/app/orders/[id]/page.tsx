import { notFound } from "next/navigation";

import { Badge } from "@/components/ui/badge";
import { CancelOrderButton } from "@/components/cancel-order-button";
import { ApiError } from "@/lib/api-client";
import { withAuthRedirect } from "@/lib/session";
import { formatCurrency, formatDate, formatOrderStatus } from "@/lib/format";
import type { OrderStatus } from "@/lib/types";

const STATUS_BADGE_VARIANT: Record<OrderStatus, "default" | "secondary" | "destructive" | "outline"> = {
  Pending: "outline",
  Confirmed: "secondary",
  Shipped: "secondary",
  Delivered: "default",
  Cancelled: "destructive",
};

export default async function OrderDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const orderId = Number(id);

  let order;
  try {
    order = await withAuthRedirect((client) => client.getOrder(orderId));
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) {
      notFound();
    }
    throw error;
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-semibold">คำสั่งซื้อ #{order.order_id}</h1>
          <p className="text-sm text-muted-foreground">{formatDate(order.order_date)}</p>
        </div>
        <Badge variant={STATUS_BADGE_VARIANT[order.status]}>{formatOrderStatus(order.status)}</Badge>
      </div>

      <div className="rounded-xl border p-4">
        <h2 className="mb-2 text-sm font-semibold text-muted-foreground">ที่อยู่จัดส่ง</h2>
        <p className="text-sm">
          {order.shipping_address_line1}
          {order.shipping_address_line2 && `, ${order.shipping_address_line2}`}
        </p>
        <p className="text-sm text-muted-foreground">
          {[order.shipping_city, order.shipping_state, order.shipping_postal_code].filter(Boolean).join(" ")}
        </p>
        <p className="text-sm text-muted-foreground">{order.shipping_country}</p>
      </div>

      <div className="rounded-xl border px-4">
        {order.items.map((item) => (
          <div
            key={item.order_item_id}
            className="flex items-center justify-between gap-4 border-b py-4 last:border-b-0"
          >
            <div className="flex flex-col gap-0.5">
              <span className="font-medium">{item.product_name}</span>
              <span className="text-sm text-muted-foreground">
                {formatCurrency(item.unit_price)} x {item.quantity} (รวมภาษี {item.tax_rate_percent}%)
              </span>
            </div>
            <span className="font-medium">{formatCurrency(item.line_total)}</span>
          </div>
        ))}
      </div>

      <div className="flex items-center justify-between rounded-xl border p-4">
        <span className="text-lg font-semibold">ยอดรวมทั้งหมด</span>
        <span className="text-lg font-semibold">{formatCurrency(order.total_amount)}</span>
      </div>

      {order.status === "Pending" && (
        <div className="flex justify-end">
          <CancelOrderButton orderId={order.order_id} />
        </div>
      )}
    </div>
  );
}
