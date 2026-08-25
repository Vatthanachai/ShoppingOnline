import Link from "next/link";

import { Button } from "@/components/ui/button";
import { ProductCard } from "@/components/product-card";
import { getApiClient } from "@/lib/session";

export default async function Home() {
  const client = await getApiClient();
  const { data: products } = await client.getProductsPage({ page_limit: 8 });

  return (
    <div className="flex flex-col gap-10">
      <section className="flex flex-col items-center gap-4 rounded-xl bg-muted/50 px-6 py-14 text-center">
        <h1 className="text-3xl font-semibold tracking-tight">ยินดีต้อนรับสู่ ShoppingOnline</h1>
        <p className="max-w-md text-muted-foreground">
          ช้อปสินค้าหลากหลายจากผู้ขายหลายราย เปรียบเทียบราคาและเลือกสิ่งที่ใช่สำหรับคุณ
        </p>
        <Button size="lg" nativeButton={false} render={<Link href="/products" />}>
          ดูสินค้าทั้งหมด
        </Button>
      </section>

      <section className="flex flex-col gap-4">
        <div className="flex items-center justify-between">
          <h2 className="text-xl font-semibold">สินค้าแนะนำ</h2>
          <Link href="/products" className="text-sm font-medium text-primary hover:underline">
            ดูทั้งหมด
          </Link>
        </div>

        {products.length === 0 ? (
          <p className="text-sm text-muted-foreground">ยังไม่มีสินค้าในระบบ</p>
        ) : (
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {products.map((product) => (
              <ProductCard key={product.product_id} product={product} />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
