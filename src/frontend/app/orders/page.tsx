import Link from "next/link";

import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
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

export default async function OrdersPage() {
  const { data: orders } = await withAuthRedirect((client) => client.getOrdersPage({ page_limit: 50 }));

  if (orders.length === 0) {
    return (
      <div className="flex flex-col items-center gap-4 py-16 text-center">
        <h1 className="text-2xl font-semibold">คุณยังไม่มีคำสั่งซื้อ</h1>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-semibold">คำสั่งซื้อของฉัน</h1>

      <div className="flex flex-col gap-3">
        {orders.map((order) => (
          <Link key={order.order_id} href={`/orders/${order.order_id}`}>
            <Card className="flex-row flex-wrap items-center justify-between gap-4 px-4 py-4 transition-shadow hover:shadow-md">
              <div className="flex flex-col gap-0.5">
                <span className="font-medium">คำสั่งซื้อ #{order.order_id}</span>
                <span className="text-sm text-muted-foreground">{formatDate(order.order_date)}</span>
              </div>
              <div className="flex items-center gap-4">
                <span className="font-medium">{formatCurrency(order.total_amount)}</span>
                <Badge variant={STATUS_BADGE_VARIANT[order.status]}>{formatOrderStatus(order.status)}</Badge>
              </div>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  );
}
