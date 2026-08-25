import Link from "next/link";

import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { StockDialog } from "@/components/admin/stocks/stock-dialog";
import { DeleteStockDialog } from "@/components/admin/stocks/delete-stock-dialog";
import { formatCurrency } from "@/lib/format";
import { getApiClient } from "@/lib/session";

const PAGE_LIMIT = 20;

const selectClassName =
  "h-8 rounded-lg border border-input bg-transparent px-2.5 text-sm outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 dark:bg-input/30";

export default async function AdminStocksPage({
  searchParams,
}: {
  searchParams: Promise<{ product?: string; vendor?: string; page?: string }>;
}) {
  const { product, vendor, page } = await searchParams;
  const pageIndex = Math.max(1, Number(page) || 1);
  const productId = product ? Number(product) : undefined;
  const vendorId = vendor ? Number(vendor) : undefined;

  const client = await getApiClient();
  const [{ data: stocks, total_pages }, { data: products }, { data: vendors }] = await Promise.all([
    client.getStocksPage({ product_id: productId, vendor_id: vendorId, page_index: pageIndex, page_limit: PAGE_LIMIT }),
    client.getProductsPage({ page_limit: 100 }),
    client.getVendorsPage({ page_limit: 100 }),
  ]);

  function pageHref(targetPage: number) {
    const params = new URLSearchParams();
    if (product) params.set("product", product);
    if (vendor) params.set("vendor", vendor);
    params.set("page", String(targetPage));
    return `/admin/stocks?${params.toString()}`;
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between gap-4">
        <form method="get" className="flex flex-1 flex-wrap items-end gap-2">
          <div className="flex flex-col gap-1.5">
            <label htmlFor="product" className="text-sm font-medium">
              สินค้า
            </label>
            <select id="product" name="product" defaultValue={product ?? ""} className={selectClassName}>
              <option value="">ทั้งหมด</option>
              {products.map((p) => (
                <option key={p.product_id} value={p.product_id}>
                  {p.product_name}
                </option>
              ))}
            </select>
          </div>

          <div className="flex flex-col gap-1.5">
            <label htmlFor="vendor" className="text-sm font-medium">
              ผู้ขาย
            </label>
            <select id="vendor" name="vendor" defaultValue={vendor ?? ""} className={selectClassName}>
              <option value="">ทั้งหมด</option>
              {vendors.map((v) => (
                <option key={v.vendor_id} value={v.vendor_id}>
                  {v.vendor_name}
                </option>
              ))}
            </select>
          </div>

          <Button type="submit" variant="outline">
            กรอง
          </Button>
        </form>
        <StockDialog mode="create" products={products} vendors={vendors} />
      </div>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>สินค้า</TableHead>
            <TableHead>ผู้ขาย</TableHead>
            <TableHead>จำนวน</TableHead>
            <TableHead>ราคา</TableHead>
            <TableHead className="text-right">การจัดการ</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {stocks.length === 0 ? (
            <TableRow>
              <TableCell colSpan={5} className="text-center text-muted-foreground">
                ไม่พบสต็อก
              </TableCell>
            </TableRow>
          ) : (
            stocks.map((stock) => (
              <TableRow key={stock.stock_id}>
                <TableCell className="font-medium">{stock.product_name}</TableCell>
                <TableCell className="text-muted-foreground">{stock.vendor_name}</TableCell>
                <TableCell>{stock.quantity}</TableCell>
                <TableCell>{formatCurrency(stock.price)}</TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    <StockDialog mode="edit" stock={stock} />
                    <DeleteStockDialog stockId={stock.stock_id} />
                  </div>
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
