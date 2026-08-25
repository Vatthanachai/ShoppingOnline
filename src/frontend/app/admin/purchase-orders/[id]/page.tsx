import { notFound } from "next/navigation";

import { Badge } from "@/components/ui/badge";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { SendPoButton } from "@/components/admin/purchase-orders/send-po-button";
import { ReceivePoForm } from "@/components/admin/purchase-orders/receive-po-form";
import { ApiError } from "@/lib/api-client";
import { formatCurrency, formatDate } from "@/lib/format";
import { getApiClient } from "@/lib/session";
import type { PurchaseOrderStatus } from "@/lib/types";

const STATUS_LABEL: Record<PurchaseOrderStatus, string> = {
  Draft: "ฉบับร่าง",
  Sent: "ส่งแล้ว",
  PartiallyReceived: "รับบางส่วน",
  Received: "รับครบแล้ว",
  Cancelled: "ยกเลิก",
};

const STATUS_VARIANT: Record<PurchaseOrderStatus, "default" | "secondary" | "outline" | "destructive"> = {
  Draft: "outline",
  Sent: "secondary",
  PartiallyReceived: "secondary",
  Received: "default",
  Cancelled: "destructive",
};

export default async function AdminPurchaseOrderDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const purchaseOrderId = Number(id);
  const client = await getApiClient();

  let po;
  try {
    po = await client.getPurchaseOrder(purchaseOrderId);
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
          <h1 className="text-2xl font-semibold">PO-{String(po.purchase_order_id).padStart(5, "0")}</h1>
          <p className="text-sm text-muted-foreground">
            ผู้ขาย: {po.vendor_name} ({po.vendor_email})
          </p>
          <p className="text-sm text-muted-foreground">สร้างเมื่อ {formatDate(po.created_on)}</p>
          {po.sent_on && <p className="text-sm text-muted-foreground">ส่งเมื่อ {formatDate(po.sent_on)}</p>}
        </div>
        <div className="flex items-center gap-3">
          <Badge variant={STATUS_VARIANT[po.status]}>{STATUS_LABEL[po.status]}</Badge>
          {po.status === "Draft" && <SendPoButton purchaseOrderId={po.purchase_order_id} />}
        </div>
      </div>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>สินค้า</TableHead>
            <TableHead>จำนวนสั่ง</TableHead>
            <TableHead>จำนวนรับแล้ว</TableHead>
            <TableHead>ราคาอ้างอิง</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {po.items.map((item) => (
            <TableRow key={item.purchase_order_item_id}>
              <TableCell className="font-medium">
                {item.product_name}
                <span className="ml-2 text-xs text-muted-foreground">{item.product_code}</span>
              </TableCell>
              <TableCell>{item.quantity_ordered}</TableCell>
              <TableCell>{item.quantity_received}</TableCell>
              <TableCell className="text-muted-foreground">
                {item.unit_cost_quoted != null ? formatCurrency(item.unit_cost_quoted) : "-"}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>

      {(po.status === "Sent" || po.status === "PartiallyReceived") && (
        <ReceivePoForm purchaseOrderId={po.purchase_order_id} items={po.items} />
      )}
    </div>
  );
}
