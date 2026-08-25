import Link from "next/link";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { ProductCard } from "@/components/product-card";
import { getApiClient } from "@/lib/session";

const PAGE_LIMIT = 12;

export default async function ProductsPage({
  searchParams,
}: {
  searchParams: Promise<{ search?: string; category?: string; page?: string }>;
}) {
  const { search, category, page } = await searchParams;
  const pageIndex = Math.max(1, Number(page) || 1);
  const categoryId = category ? Number(category) : undefined;

  const client = await getApiClient();
  const [{ data: products, total_pages }, { data: categories }] = await Promise.all([
    client.getProductsPage({
      search: search || undefined,
      product_category_id: categoryId,
      page_index: pageIndex,
      page_limit: PAGE_LIMIT,
    }),
    client.getCategoriesPage({ page_limit: 100 }),
  ]);

  function pageHref(targetPage: number) {
    const params = new URLSearchParams();
    if (search) params.set("search", search);
    if (category) params.set("category", category);
    params.set("page", String(targetPage));
    return `/products?${params.toString()}`;
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-semibold">สินค้าทั้งหมด</h1>

      <form method="get" className="flex flex-col gap-3 sm:flex-row sm:items-end">
        <div className="flex flex-1 flex-col gap-1.5">
          <label htmlFor="search" className="text-sm font-medium">
            ค้นหาสินค้า
          </label>
          <Input id="search" name="search" defaultValue={search} placeholder="ชื่อสินค้า..." />
        </div>

        <div className="flex flex-col gap-1.5">
          <label htmlFor="category" className="text-sm font-medium">
            หมวดหมู่
          </label>
          <select
            id="category"
            name="category"
            defaultValue={category ?? ""}
            className="h-8 rounded-lg border border-input bg-transparent px-2.5 text-sm outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 dark:bg-input/30"
          >
            <option value="">ทั้งหมด</option>
            {categories.map((c) => (
              <option key={c.product_category_id} value={c.product_category_id}>
                {c.category_name}
              </option>
            ))}
          </select>
        </div>

        <Button type="submit">ค้นหา</Button>
      </form>

      {products.length === 0 ? (
        <p className="text-sm text-muted-foreground">ไม่พบสินค้าที่ตรงกับเงื่อนไข</p>
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {products.map((product) => (
            <ProductCard key={product.product_id} product={product} />
          ))}
        </div>
      )}

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
