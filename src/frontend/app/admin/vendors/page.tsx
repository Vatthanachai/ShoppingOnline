import Link from "next/link";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { VendorDialog } from "@/components/admin/vendors/vendor-dialog";
import { DeactivateVendorDialog } from "@/components/admin/vendors/deactivate-vendor-dialog";
import { getApiClient } from "@/lib/session";

const PAGE_LIMIT = 20;

export default async function AdminVendorsPage({
  searchParams,
}: {
  searchParams: Promise<{ search?: string; page?: string }>;
}) {
  const { search, page } = await searchParams;
  const pageIndex = Math.max(1, Number(page) || 1);

  const client = await getApiClient();
  const { data: vendors, total_pages } = await client.getVendorsPage({
    search: search || undefined,
    page_index: pageIndex,
    page_limit: PAGE_LIMIT,
  });

  function pageHref(targetPage: number) {
    const params = new URLSearchParams();
    if (search) params.set("search", search);
    params.set("page", String(targetPage));
    return `/admin/vendors?${params.toString()}`;
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between gap-4">
        <form method="get" className="flex flex-1 gap-2">
          <Input name="search" defaultValue={search} placeholder="ค้นหาผู้ขาย..." />
          <Button type="submit" variant="outline">
            ค้นหา
          </Button>
        </form>
        <VendorDialog mode="create" />
      </div>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>ชื่อผู้ขาย</TableHead>
            <TableHead>ผู้ติดต่อ</TableHead>
            <TableHead>อีเมล</TableHead>
            <TableHead>เบอร์โทรศัพท์</TableHead>
            <TableHead>สถานะ</TableHead>
            <TableHead className="text-right">การจัดการ</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {vendors.length === 0 ? (
            <TableRow>
              <TableCell colSpan={6} className="text-center text-muted-foreground">
                ไม่พบผู้ขาย
              </TableCell>
            </TableRow>
          ) : (
            vendors.map((vendor) => (
              <TableRow key={vendor.vendor_id}>
                <TableCell className="font-medium">{vendor.vendor_name}</TableCell>
                <TableCell className="text-muted-foreground">{vendor.contact_person}</TableCell>
                <TableCell className="text-muted-foreground">{vendor.email}</TableCell>
                <TableCell className="text-muted-foreground">{vendor.phone}</TableCell>
                <TableCell>
                  <Badge variant={vendor.is_active ? "default" : "outline"}>
                    {vendor.is_active ? "ใช้งาน" : "ปิดใช้งาน"}
                  </Badge>
                </TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    <VendorDialog mode="edit" vendor={vendor} />
                    {vendor.is_active && <DeactivateVendorDialog vendorId={vendor.vendor_id} />}
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
