import Link from "next/link";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { UserStatusToggle } from "@/components/admin/users/user-status-toggle";
import { formatDate } from "@/lib/format";
import { getApiClient } from "@/lib/session";

const PAGE_LIMIT = 20;

export default async function AdminUsersPage({
  searchParams,
}: {
  searchParams: Promise<{ search?: string; page?: string }>;
}) {
  const { search, page } = await searchParams;
  const pageIndex = Math.max(1, Number(page) || 1);

  const client = await getApiClient();
  const { data: users, total_pages } = await client.getUsersPage({
    search: search || undefined,
    page_index: pageIndex,
    page_limit: PAGE_LIMIT,
  });

  function pageHref(targetPage: number) {
    const params = new URLSearchParams();
    if (search) params.set("search", search);
    params.set("page", String(targetPage));
    return `/admin/users?${params.toString()}`;
  }

  return (
    <div className="flex flex-col gap-4">
      <form method="get" className="flex gap-2">
        <Input name="search" defaultValue={search} placeholder="ค้นหาผู้ใช้..." />
        <Button type="submit" variant="outline">
          ค้นหา
        </Button>
      </form>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>ชื่อ</TableHead>
            <TableHead>อีเมล</TableHead>
            <TableHead>เบอร์โทรศัพท์</TableHead>
            <TableHead>บทบาท</TableHead>
            <TableHead>สมัครเมื่อ</TableHead>
            <TableHead className="text-right">เปิดใช้งาน</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {users.length === 0 ? (
            <TableRow>
              <TableCell colSpan={6} className="text-center text-muted-foreground">
                ไม่พบผู้ใช้
              </TableCell>
            </TableRow>
          ) : (
            users.map((user) => (
              <TableRow key={user.user_id}>
                <TableCell className="font-medium">{user.name}</TableCell>
                <TableCell className="text-muted-foreground">{user.email}</TableCell>
                <TableCell className="text-muted-foreground">{user.phone}</TableCell>
                <TableCell>
                  <Badge variant={user.role === "Admin" ? "secondary" : "outline"}>{user.role}</Badge>
                </TableCell>
                <TableCell className="text-muted-foreground">{formatDate(user.created_on)}</TableCell>
                <TableCell className="text-right">
                  <UserStatusToggle userId={user.user_id} isActive={user.is_active} />
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
