import Link from "next/link";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { ProductDialog } from "@/components/admin/products/product-dialog";
import { DeactivateProductDialog } from "@/components/admin/products/deactivate-product-dialog";
import { formatCurrency } from "@/lib/format";
import { getApiClient } from "@/lib/session";

const PAGE_LIMIT = 20;

export default async function AdminProductsPage({
  searchParams,
}: {
  searchParams: Promise<{ search?: string; page?: string }>;
}) {
  const { search, page } = await searchParams;
  const pageIndex = Math.max(1, Number(page) || 1);

  const client = await getApiClient();
  const [{ data: products, total_pages }, { data: categories }, { data: vendors }] = await Promise.all([
    client.getProductsPage({
      search: search || undefined,
      include_inactive: true,
      page_index: pageIndex,
      page_limit: PAGE_LIMIT,
    }),
    client.getCategoriesPage({ include_inactive: true, page_limit: 100 }),
    client.getVendorsPage({ include_inactive: true, page_limit: 100 }),
  ]);

  function pageHref(targetPage: number) {
    const params = new URLSearchParams();
    if (search) params.set("search", search);
    params.set("page", String(targetPage));
    return `/admin/products?${params.toString()}`;
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between gap-4">
        <form method="get" className="flex flex-1 gap-2">
          <Input name="search" defaultValue={search} placeholder="ค้นหาสินค้า..." />
          <Button type="submit" variant="outline">
            ค้นหา
          </Button>
        </form>
        <ProductDialog mode="create" categories={categories} vendors={vendors} />
      </div>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>รหัสสินค้า</TableHead>
            <TableHead>ชื่อสินค้า</TableHead>
            <TableHead>หมวดหมู่</TableHead>
            <TableHead>ผู้ขาย</TableHead>
            <TableHead>ราคาขาย (รวมภาษี)</TableHead>
            <TableHead>คงเหลือ</TableHead>
            <TableHead>สถานะ</TableHead>
            <TableHead className="text-right">การจัดการ</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {products.length === 0 ? (
            <TableRow>
              <TableCell colSpan={8} className="text-center text-muted-foreground">
                ไม่พบสินค้า
              </TableCell>
            </TableRow>
          ) : (
            products.map((product) => (
              <TableRow key={product.product_id}>
                <TableCell className="text-muted-foreground">{product.product_code}</TableCell>
                <TableCell className="font-medium">{product.product_name}</TableCell>
                <TableCell className="text-muted-foreground">{product.product_category_name}</TableCell>
                <TableCell className="text-muted-foreground">{product.vendor_name}</TableCell>
                <TableCell className="text-muted-foreground">{formatCurrency(product.price_with_tax)}</TableCell>
                <TableCell className="text-muted-foreground">{product.available_quantity}</TableCell>
                <TableCell>
                  <Badge variant={product.is_active ? "default" : "outline"}>
                    {product.is_active ? "ใช้งาน" : "ปิดใช้งาน"}
                  </Badge>
                </TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    <ProductDialog mode="edit" product={product} categories={categories} vendors={vendors} />
                    {product.is_active && <DeactivateProductDialog productId={product.product_id} />}
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
