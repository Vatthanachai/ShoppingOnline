import Link from "next/link";

import { Card, CardContent } from "@/components/ui/card";

const SECTIONS = [
  { href: "/admin/products", label: "สินค้า", description: "จัดการรายการสินค้า หมวดหมู่ และผู้ขาย" },
  { href: "/admin/vendors", label: "ผู้ขาย", description: "จัดการข้อมูลผู้ขาย" },
  { href: "/admin/categories", label: "หมวดหมู่", description: "จัดการหมวดหมู่สินค้า" },
  { href: "/admin/stocks", label: "สต็อก", description: "ดูล็อตสต็อกคงเหลือ (แบบดูอย่างเดียว)" },
  { href: "/admin/purchase-orders", label: "ใบสั่งซื้อ", description: "ออกใบสั่งซื้อไปยัง vendor และรับสินค้าเข้าสต็อก" },
  { href: "/admin/users", label: "ผู้ใช้งาน", description: "ดูรายชื่อและเปิด/ปิดการใช้งานบัญชีผู้ใช้" },
];

export default function AdminIndexPage() {
  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {SECTIONS.map((section) => (
        <Link key={section.href} href={section.href}>
          <Card className="h-full transition-colors hover:bg-muted/50">
            <CardContent className="flex flex-col gap-1">
              <p className="font-medium">{section.label}</p>
              <p className="text-sm text-muted-foreground">{section.description}</p>
            </CardContent>
          </Card>
        </Link>
      ))}
    </div>
  );
}
