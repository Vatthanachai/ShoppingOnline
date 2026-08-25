import Link from "next/link";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { CreatePoDialog } from "@/components/admin/purchase-orders/create-po-dialog";
import { formatDate } from "@/lib/format";
import { getApiClient } from "@/lib/session";
import type { PurchaseOrderStatus } from "@/lib/types";

const PAGE_LIMIT = 20;

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

export default async function AdminPurchaseOrdersPage({
  searchParams,
}: {
  searchParams: Promise<{ page?: string }>;
}) {
  const { page } = await searchParams;
  const pageIndex = Math.max(1, Number(page) || 1);

  const client = await getApiClient();
  const [{ data: purchaseOrders, total_pages }, { data: vendors }, { data: products }] = await Promise.all([
    client.getPurchaseOrdersPage({ page_index: pageIndex, page_limit: PAGE_LIMIT }),
    client.getVendorsPage({ page_limit: 100 }),
    client.getProductsPage({ include_inactive: true, page_limit: 100 }),
  ]);

  function pageHref(targetPage: number) {
    const params = new URLSearchParams();
    params.set("page", String(targetPage));
    return `/admin/purchase-orders?${params.toString()}`;
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-end">
        <CreatePoDialog vendors={vendors} products={products} />
      </div>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>เลขที่</TableHead>
            <TableHead>ผู้ขาย</TableHead>
            <TableHead>จำนวนรายการ</TableHead>
            <TableHead>สร้างเมื่อ</TableHead>
            <TableHead>ส่งเมื่อ</TableHead>
            <TableHead>สถานะ</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {purchaseOrders.length === 0 ? (
            <TableRow>
              <TableCell colSpan={6} className="text-center text-muted-foreground">
                ยังไม่มีใบสั่งซื้อ
              </TableCell>
            </TableRow>
          ) : (
            purchaseOrders.map((po) => (
              <TableRow key={po.purchase_order_id} className="cursor-pointer">
                <TableCell className="font-medium">
                  <Link href={`/admin/purchase-orders/${po.purchase_order_id}`} className="hover:underline">
                    PO-{String(po.purchase_order_id).padStart(5, "0")}
                  </Link>
                </TableCell>
                <TableCell className="text-muted-foreground">{po.vendor_name}</TableCell>
                <TableCell>{po.item_count}</TableCell>
                <TableCell className="text-muted-foreground">{formatDate(po.created_on)}</TableCell>
                <TableCell className="text-muted-foreground">{po.sent_on ? formatDate(po.sent_on) : "-"}</TableCell>
                <TableCell>
                  <Badge variant={STATUS_VARIANT[po.status]}>{STATUS_LABEL[po.status]}</Badge>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>

      {total_pages > 1 && (
        <div className="flex items-center justify-center gap-2">
          {pageIndex > 1 ? (
            <Button variant="outline" nativeButton={false} render={<Link href={pageHref(pageIndex - 1)} />}>
              ก่อนหน้า
            </Button>
          ) : (
            <Button variant="outline" disabled>
              ก่อนหน้า
            </Button>
          )}
          <span className="text-sm text-muted-foreground">
            หน้า {pageIndex} จาก {total_pages}
          </span>
          {pageIndex < total_pages ? (
            <Button variant="outline" nativeButton={false} render={<Link href={pageHref(pageIndex + 1)} />}>
              ถัดไป
            </Button>
          ) : (
            <Button variant="outline" disabled>
              ถัดไป
            </Button>
          )}
        </div>
      )}
    </div>
  );
}
